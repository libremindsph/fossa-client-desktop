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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Fossa.Client.Desktop.Llama.Events;

namespace Fossa.Client.Desktop.Llama.Entities;

public partial class LlamaModel : ObservableObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string DownloadSource { get; set; }
    public string FileName { get; set; }
    public string FileSize { get; set; }
    public string ParameterSize { get; set; }
    public string Prompt { get; set; }
    public int ContextSize { get; set; }
    public float RepeatPenalty { get; set; }
    public float Temperature { get; set; }
    public string SessionsDirectory { get; set; }
    public bool Heavy { get; set; }

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsInitializedOrStarted))] private bool _downloadStarting;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsInitializedOrStarted))] private bool _downloadStarted;
    [ObservableProperty] private double _downloadProgress;
    [ObservableProperty] private bool _installed;

    public bool IsInitializedOrStarted => DownloadStarted || DownloadStarting;

    public LlamaModel()
    {
        WeakReferenceMessenger.Default
            .Register<ModelDownloadStartedEvent>(this, OnModelDownloadStarted);
        WeakReferenceMessenger.Default
            .Register<ModelDownloadProgressChangedEvent>(this, OnModelDownloadProgressChanged);
        WeakReferenceMessenger.Default
            .Register<ModelDownloadCompletedEvent>(this, OnModelDownloadCompleted);
        WeakReferenceMessenger.Default
            .Register<ModelDownloadInitializedEvent>(this, OnModelDownloadInitialized);
        WeakReferenceMessenger.Default
            .Register<ModelRemovedEvent>(this, OnModelRemoved);
    }

    private void OnModelDownloadInitialized(object recipient, ModelDownloadInitializedEvent message)
    {
        if (!message.Value.Equals(Name)) return;
        DownloadStarting = true;
    }

    private void OnModelDownloadCompleted(object recipient, ModelDownloadCompletedEvent message)
    {
        if (!message.Value.Equals(FileName)) return;
        DownloadStarted = false;
        Installed = true;
    }

    private void OnModelDownloadProgressChanged(object recipient, ModelDownloadProgressChangedEvent message)
    {
        if (!DownloadStarted || !message.Value.Item1.Equals(FileName)) return;
        DownloadProgress = Math.Round(message.Value.Item2,2);
    }

    private void OnModelDownloadStarted(object recipient, ModelDownloadStartedEvent message)
    {
        if (!message.Value.Equals(FileName)) return;
        DownloadStarting = false;
        DownloadStarted = true;
    }

    private void OnModelRemoved(object recipient, ModelRemovedEvent message)
    {
        if (!message.Value.Equals(FileName)) return;
        Installed = false;
    }

    [RelayCommand]
    void Download()
    {
        WeakReferenceMessenger.Default.Send(new ModelDownloadEvent(this));
    }

    [RelayCommand]
    void Uninstall()
    {
        WeakReferenceMessenger.Default.Send(new ModelUninstallEvent(this));
    }
}