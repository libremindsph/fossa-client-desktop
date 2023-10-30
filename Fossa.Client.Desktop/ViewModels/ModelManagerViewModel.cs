// MIT License
// 
// Copyright (c) 2023 Russell Camo (russkyc), Libre Minds
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.UI.Controls;
using Fossa.Client.Desktop.Configuration;
using Fossa.Client.Desktop.Downloader;
using Fossa.Client.Desktop.Extensions;
using Fossa.Client.Desktop.Llama;
using Fossa.Client.Desktop.Llama.Entities;
using Fossa.Client.Desktop.Llama.Events;
using Fossa.Client.Desktop.Services;

namespace Fossa.Client.Desktop.ViewModels;

public partial class ModelManagerViewModel : ObservableObject
{
    private readonly AppConfig _appConfig;
    private readonly ModelProvider _modelProvider;
    private readonly DialogFactory _dialogFactory;
    private readonly DownloadManager _downloadManager;

    [ObservableProperty] private double _downloadPercentage = 0;
    [ObservableProperty] private ObservableCollection<LlamaModel> _models = new();

    public ModelManagerViewModel(
        AppConfig appConfig,
        ModelProvider modelProvider,
        DownloadManager downloadManager,
        DialogFactory dialogFactory)
    {
        _appConfig = appConfig;
        _modelProvider = modelProvider;
        _downloadManager = downloadManager;
        _dialogFactory = dialogFactory;

        Models = _modelProvider.GetDownloadableModels()
            .Select(model =>
            {
                model.Installed = File.Exists(_appConfig.ModelsDirectory + model.FileName);
                return model;
            })
            .ToObservableCollection();
        
        WeakReferenceMessenger.Default
            .Register<ModelDownloadEvent>(this, OnModelDownload);
        WeakReferenceMessenger.Default
            .Register<ModelUninstallEvent>(this, OnModelUninstall);
    }

    private async void OnModelDownload(object recipient, ModelDownloadEvent message)
    {
        var continueInstall = await _dialogFactory.CreateModelInstallDialog(message.Value.Name, message.Value.FileSize);
        if (continueInstall is ContentDialogResult.None) return;
        
        WeakReferenceMessenger.Default.Send(new ModelDownloadInitializedEvent(message.Value.Name));
        await _downloadManager.DownloadAsync(message.Value.DownloadSource,
            message.Value.FileName, DownloadStarted, DownloadCompleted, DownloadProgressChanged);
    }

    private async void OnModelUninstall(object recipient, ModelUninstallEvent message)
    {

        var continueUninstall = await _dialogFactory.CreateModelUninstallDialog(message.Value.Name);
        if (continueUninstall is ContentDialogResult.None) return;
        
        var modelPath = _appConfig.ModelsDirectory + message.Value.FileName;
        if (!File.Exists(modelPath)) return;
        
        File.Delete(modelPath);
        if (File.Exists(modelPath)) return;
        
        WeakReferenceMessenger.Default.Send(new ModelRemovedEvent(message.Value.FileName));
    }

    private void DownloadProgressChanged((string, double) downloadData)
    {
        WeakReferenceMessenger.Default.Send(new ModelDownloadProgressChangedEvent(downloadData));
    }

    private void DownloadCompleted(string modelName)
    {
        WeakReferenceMessenger.Default.Send(new ModelDownloadCompletedEvent(modelName));
    }

    private void DownloadStarted(string modelName)
    {
        WeakReferenceMessenger.Default.Send(new ModelDownloadStartedEvent(modelName));
    }
}