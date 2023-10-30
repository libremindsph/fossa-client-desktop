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

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.UI.Controls;
using Fossa.Client.Desktop.Llama.Events;

namespace Fossa.Client.Desktop.Services;

public class DialogFactory
{
    public Task<ContentDialogResult> CreateModelUninstallDialog(string modelName)
    {
        return new ContentDialog
        {
            Title = $"Uninstalling {modelName}",
            Content = "Downloading the model again is necessary after uninstalling.",
            PrimaryButtonText = "Yes",
            CloseButtonText = "Cancel",
            IsSecondaryButtonEnabled = false
        }.ShowAsync();
    }
    
    public Task<ContentDialogResult> CreateModelFailedToLoadDialog(string modelName)
    {
        return new ContentDialog
        {
            Title = $"{modelName} Load Failed",
            Content = "The model is either not installed or corrupted, please open the model manager",
            PrimaryButtonText = "Open Model Manager",
            PrimaryButtonCommand = new RelayCommand(() => WeakReferenceMessenger.Default.Send(new OpenModelManagerEvent())),
            CloseButtonText = "Cancel",
            IsSecondaryButtonEnabled = false
        }.ShowAsync();
    }
    
    public Task<ContentDialogResult> CreateModelInstallDialog(string modelName, string fileSize)
    {
        return new ContentDialog
        {
            Title = $"Installing {modelName}",
            Content = $"The model will take up {fileSize} after install, would you like to proceed?",
            PrimaryButtonText = "Yes",
            CloseButtonText = "Cancel",
            IsSecondaryButtonEnabled = false
        }.ShowAsync();
    }
}