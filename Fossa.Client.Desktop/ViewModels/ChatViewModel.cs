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
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Fossa.Client.Desktop.Conversation.Events;
using Fossa.Client.Desktop.Conversation.Factory;
using Fossa.Client.Desktop.Conversation.Interfaces;
using Fossa.Client.Desktop.Llama;
using Fossa.Client.Desktop.Llama.Entities;
using Fossa.Client.Desktop.Llama.Events;
using Fossa.Client.Desktop.Services;

namespace Fossa.Client.Desktop.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    private readonly LlamaClient _llamaClient;
    private readonly DialogFactory _dialogFactory;
    private readonly MessageFactory _messageFactory;

    [ObservableProperty] private string _prompt = "";
    [ObservableProperty] private bool _canSend = true;
    [ObservableProperty] private ObservableCollection<IConversationItem> _conversationItems = new();
    [ObservableProperty] private LlamaModel? _model;

    public ChatViewModel(
        DialogFactory dialogFactory,
        MessageFactory messageFactory,
        LlamaClient llamaClient)
    {
        _messageFactory = messageFactory;
        _llamaClient = llamaClient;
        _dialogFactory = dialogFactory;

        WeakReferenceMessenger.Default
            .Register<ModelChangedEvent>(this, OnLlamaModelChanged);
        WeakReferenceMessenger.Default
            .Register<ModelLoadFailedEvent>(this, OnLlamaLoadFailed);
    }

    private void OnLlamaLoadFailed(object recipient, ModelLoadFailedEvent message)
    {
        Model = message.OldValue;
    }

    private void OnLlamaModelChanged(object recipient, ModelChangedEvent message)
    {
        Model = message.Value;
    }

    partial void OnModelChanged(LlamaModel? oldValue, LlamaModel? newValue)
    {
        if (newValue is null || oldValue! == newValue) return;
        
        Task.Run(async () =>
        {
            CanSend = false;

            await _llamaClient.LoadModelAsync(newValue, oldValue);

            CanSend = true;
            WeakReferenceMessenger.Default.Send(new ResponseChangedEvent(true));
        });
    }
    
    [RelayCommand]
    private void StopGenerating()
    {
        _llamaClient.StopResponse();
    }

    [RelayCommand]
    private Task ChatAsync()
    {
        if (string.IsNullOrWhiteSpace(Prompt) || Prompt.Length < 2) return Task.CompletedTask;
        
        var prompt = Prompt;
        var response = _messageFactory.CreateBotMessage();
        
        ConversationItems.Add(_messageFactory.CreateUserMessage(prompt));

        return Task.Run(async () =>
        {
            CanSend = false;
            Prompt = string.Empty;
            
            // Simulate response delay
            await Task.Delay(500);
            
            ConversationItems.Add(response);
            WeakReferenceMessenger.Default.Send(new ResponseChangedEvent(false));
            
            await foreach (var text in _llamaClient.ChatAsync(prompt))
            {
                response.Message += text;
                WeakReferenceMessenger.Default.Send(new ResponseChangedEvent(false));
            }
            
            CanSend = true;
            response.IsCurrent = false;
            WeakReferenceMessenger.Default.Send(new ResponseChangedEvent(true));
        });
    }
}