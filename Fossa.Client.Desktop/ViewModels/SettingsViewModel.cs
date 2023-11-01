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
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Fossa.Client.Desktop.Services;
using Fossa.Client.Desktop.Services.Updater;

namespace Fossa.Client.Desktop.ViewModels;

// TODO: move update functionality to about page
public partial class SettingsViewModel : ObservableObject
{
    private readonly GithubUpdater _githubUpdater;
    private readonly DialogFactory _dialogFactory;
    [ObservableProperty] private double _downloadProgress;
    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private string _version;
    [ObservableProperty] private bool _isCheckingForUpdates;

    public SettingsViewModel(GithubUpdater githubUpdater, DialogFactory dialogFactory)
    {
        _githubUpdater = githubUpdater;
        _dialogFactory = dialogFactory;
        Version = GithubUpdater.ReadCurrentVersion()?.ToString() ?? "Cannot read version";
    }

    [RelayCommand]
    private async Task CheckForUpdates()
    {
        IsProcessing = true;
        IsCheckingForUpdates = true;
        try
        {
            var updateStatus = await _githubUpdater.CheckForUpdates();
            IsCheckingForUpdates = false;
            if (updateStatus != UpdateStatus.Available)
            {
                await _dialogFactory.CreateInfoDialog("No Update", "No updates available");
                return;
            }

            var installUpdateDialog = await _dialogFactory.CreateInstallUpdateDialog("Update Available",
                "An update is available, would you like to install it?");
            if (installUpdateDialog is not ContentDialogResult.Primary) return;
            await _githubUpdater.PerformUpdate(new Progress<double>(val => DownloadProgress = val * 100),
                async () => await _dialogFactory.CreateInstallUpdateDialog("Update Downloaded",
                    "Update has been downloaded, restart the application?") is ContentDialogResult.Primary
            );
        }
        finally
        {
            IsProcessing = false;
            IsCheckingForUpdates = false;
        }
    }
}