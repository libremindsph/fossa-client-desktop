// MIT License
// 
// Copyright (c) 2023 Libre Minds
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
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Fossa.Client.Desktop.Configuration;
using Fossa.Client.Desktop.Models.Entities;
using LLama;
using LLama.Common;

namespace Fossa.Client.Desktop.Models;

public class LlamaClient
{
    private readonly AppConfig _appConfig;
    private LlamaModel? _model;
    private ChatSession? _chatSession;
    private LLamaContext _context = null!;
    private InteractiveExecutor? _executor;
    private CancellationTokenSource? _stopResponseTokenSource;

    public LlamaClient(AppConfig appConfig)
    {
        _appConfig = appConfig;
    }

    public async Task LoadModelAsync(LlamaModel model)
    {
        _model = model;
        await Task.Run(() =>
        {
            var parameters = new ModelParams(_appConfig.ModelsDirectory + _model.FileName)
            {
                Threads = _appConfig.Threads,
                ContextSize = _model.ContextSize,
                Perplexity = _model.Perplexity
            };
            _context = LLamaWeights.LoadFromFile(parameters)
                .CreateContext(parameters);
        });
    }

    public async IAsyncEnumerable<string> ChatAsync(string prompt)
    {
        _stopResponseTokenSource = new();
        _executor ??= new (_context);
        _chatSession ??= new (_executor);

        await foreach (var text in _chatSession.ChatAsync(prompt,
                           new InferenceParams 
                           { 
                               Temperature = _model!.Temperature,
                               TfsZ = _model!.TfsZ,
                               TopK = _model.TopK,
                               RepeatPenalty = _model.RepeatPenalty,
                               TopP = _model.TopP,
                               TypicalP = _model.TypicalP,
                               FrequencyPenalty = _model.FrequencyPenalty,
                               PresencePenalty = _model.PresencePenalty,
                               AntiPrompts = new List<string> { "User:" } 
                           }, _stopResponseTokenSource.Token))
        {
            yield return text;
        }
        _chatSession.SaveSession($"{AppDomain.CurrentDomain.BaseDirectory}/sessions");
    }

    public void StopResponse()
    {
        if (_stopResponseTokenSource is null || _stopResponseTokenSource.IsCancellationRequested)
        {
            return;
        }
        _stopResponseTokenSource.Cancel();
        _stopResponseTokenSource.Dispose();
    }
}