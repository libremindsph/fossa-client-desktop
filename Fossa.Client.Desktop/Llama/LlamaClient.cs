﻿// MIT License
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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Fossa.Client.Desktop.Configuration;
using Fossa.Client.Desktop.Llama.Entities;
using Fossa.Client.Desktop.Llama.Events;
using Fossa.Client.Desktop.Services;
using LLama;
using LLama.Common;

namespace Fossa.Client.Desktop.Llama;

public class LlamaClient
{
    private readonly AppConfig _appConfig;
    private readonly DialogFactory _dialogFactory;
    private LlamaModel? _model;
    private ChatSession? _chatSession;
    private LLamaContext _context = null!;
    private InstructExecutor? _executor;
    private CancellationTokenSource? _stopResponseTokenSource;

    public LlamaClient(AppConfig appConfig, DialogFactory dialogFactory)
    {
        _appConfig = appConfig;
        _dialogFactory = dialogFactory;
    }

    public async Task LoadModelAsync(LlamaModel model, LlamaModel? oldModel)
    {
        _model = model;
        
        await Task.Run(() =>
        {
            var parameters = new ModelParams(_appConfig.ModelsDirectory + _model.FileName)
            {
                Encoding = Encoding.UTF8,
                GpuLayerCount = -1,
                Threads = _appConfig.Threads,
                ContextSize = _model.ContextSize
            };
            try
            {
                _context = LLamaWeights.LoadFromFile(parameters)
                    .CreateContext(parameters);
            }
            catch (Exception e)
            {
                Dispatcher.UIThread.InvokeAsync(() => _dialogFactory.CreateModelFailedToLoadDialog(model.Name));
                WeakReferenceMessenger.Default.Send(new ModelLoadFailedEvent(oldModel, model));
            }
        }, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
        
    }

    public async IAsyncEnumerable<string> ChatAsync(string prompt)
    {
        _stopResponseTokenSource = new();
        _executor ??= new (_context);
        _chatSession ??= new (_executor);

        await foreach (var text in _chatSession.ChatAsync(prompt,
                           new InferenceParams 
                           {
                               MirostatTau = 5.0f,
                               MirostatEta = 0.10f,
                               TopP = 0.95f,
                               MaxTokens = -1,
                               TopK = 40,
                               RepeatLastTokensCount = 64,
                               PresencePenalty = 0.0f,
                               Temperature = _model!.Temperature,
                               RepeatPenalty = _model.RepeatPenalty
                           }, _stopResponseTokenSource.Token))
        {
            yield return text.Replace(">","");
        }
    }

    public void StopResponse()
    {
        if (_stopResponseTokenSource is null
            || _stopResponseTokenSource.IsCancellationRequested) return;
        _stopResponseTokenSource.Cancel();
        _stopResponseTokenSource.Dispose();
    }
}