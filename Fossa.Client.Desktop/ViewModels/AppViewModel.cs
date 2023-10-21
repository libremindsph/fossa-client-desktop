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
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fossa.Client.Desktop.Configuration;
using Fossa.Client.Desktop.Extensions;
using Fossa.Client.Desktop.Models;
using Fossa.Client.Desktop.Models.Entities;
using Fossa.Client.Desktop.Services;
using JsonFlatFileDataStore;

namespace Fossa.Client.Desktop.ViewModels;

public partial class AppViewModel : ObservableObject
{
    private readonly AppConfig _appConfig;
    private readonly ViewFactory _viewFactory;

    [ObservableProperty] private ChatViewModel _pageContext;
    [ObservableProperty] private ObservableCollection<LlamaModel> _models = new();
    [ObservableProperty] private bool _isOnWindows = Environment.OSVersion.Platform is PlatformID.Win32NT;

    public AppViewModel(
        ModelProvider modelProvider,
        ChatViewModel pageContext,
        ViewFactory viewFactory,
        AppConfig appConfig)
    {
        PageContext = pageContext;
        _viewFactory = viewFactory;
        _appConfig = appConfig;
        Models = modelProvider.GetDownloadableModels().ToObservableCollection();
        Task.Run(async () =>
        {
            await Task.Run(() => PageContext.Model = Models.FirstOrDefault());
        });
    }

    [RelayCommand]
    private async Task OpenModelManager()
    {
        await _viewFactory.CreateModelView(this);
    }
}