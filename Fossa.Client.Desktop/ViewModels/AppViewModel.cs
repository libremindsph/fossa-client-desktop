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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Fossa.Client.Desktop.Extensions;
using Fossa.Client.Desktop.Llama;
using Fossa.Client.Desktop.Llama.Entities;
using Fossa.Client.Desktop.Llama.Events;
using Fossa.Client.Desktop.Services;

namespace Fossa.Client.Desktop.ViewModels;

public partial class AppViewModel : ObservableObject
{
    private readonly DialogFactory _dialogFactory;
    private readonly ModelProvider _modelProvider;
    private readonly SettingsViewModel _settingsViewModel;
    private readonly ChatViewModel _chatViewModel;
    private readonly ModelManagerViewModel _modelManagerViewModel;

    [ObservableProperty] private ObservableObject _pageContext;
    [ObservableProperty] private ObservableCollection<LlamaModel> _models = new();
    [ObservableProperty] private bool _isOnWindows = Environment.OSVersion.Platform is PlatformID.Win32NT;

    public AppViewModel(
        DialogFactory dialogFactory,
        ModelProvider modelProvider,
        SettingsViewModel settingsViewModel,
        ChatViewModel chatViewModel,
        ModelManagerViewModel modelManagerViewModel)
    {
        _dialogFactory = dialogFactory;
        _modelProvider = modelProvider;
        _settingsViewModel = settingsViewModel;
        _chatViewModel = chatViewModel;
        _modelManagerViewModel = modelManagerViewModel;

        PageContext = _chatViewModel;

        Models = _modelProvider.GetDownloadableModels()
            .ToObservableCollection();
        
        WeakReferenceMessenger.Default
            .Send(new ModelChangedEvent(Models.First()));
        WeakReferenceMessenger.Default
            .Register<OpenModelManagerEvent>(this, OnOpenModelManager);
        WeakReferenceMessenger.Default
            .Register<ModelDownloadCompletedEvent>(this, OnModelDownloadCompleted);
        
        OpenChat();
    }
    
    [RelayCommand]
    private void OpenSettings()
    {
        PageContext = _settingsViewModel;
    }

    [RelayCommand]
    private void OpenModelManager()
    {
        PageContext = _modelManagerViewModel;
    }

    [RelayCommand]
    private void OpenChat()
    {
        PageContext = _chatViewModel;
    }
    
    private void OnModelDownloadCompleted(object recipient, ModelDownloadCompletedEvent message)
    {
        if (Models.Count(model => model.Installed) > 1) return;
        WeakReferenceMessenger.Default
            .Send(new ModelChangedEvent(Models.First()));
    }

    private void OnOpenModelManager(object recipient, OpenModelManagerEvent message)
    {
        OpenModelManager();
    }

}