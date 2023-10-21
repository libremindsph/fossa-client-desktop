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
using Fossa.Client.Desktop.Models;
using Fossa.Client.Desktop.Models.Entities;

namespace Fossa.Client.Desktop.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    private readonly LlamaClient _llamaClient;
    private readonly MessageFactory _messageFactory;

    [ObservableProperty] private string _prompt = "";
    [ObservableProperty] private bool _canSend = true;
    [ObservableProperty] private LlamaModel? _model;
    [ObservableProperty] private ObservableCollection<IConversationItem> _conversationItems = new();

    public ChatViewModel(
        MessageFactory messageFactory,
        LlamaClient llamaClient)
    {
        _messageFactory = messageFactory;
        _llamaClient = llamaClient;
    }

    partial void OnModelChanged(LlamaModel? oldValue, LlamaModel? newValue)
    {
        if (newValue is null || oldValue! == newValue)
        {
            return;
        }
        Task.Run(async () =>
        {
            CanSend = false;
            
            await _llamaClient.LoadModelAsync(newValue);
            await Task.Delay(1000);
            
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
    private async Task ChatAsync()
    {
        if (string.IsNullOrWhiteSpace(Prompt) || Prompt.Length < 2)
        {
            return;
        }
        
        var prompt = Prompt;
        var response = _messageFactory.CreateBotMessage();
        
        ConversationItems.Add(_messageFactory.CreateUserMessage(prompt));
        ConversationItems.Add(response);

        await Task.Run(async () =>
        {
            CanSend = false;
            Prompt = string.Empty;
            
            await foreach (var text in _llamaClient.ChatAsync(prompt))
            {
                response.Message += text;
                WeakReferenceMessenger.Default.Send(new ResponseChangedEvent(false));
            }
            
            response.IsCurrent = false;
            CanSend = true;
            WeakReferenceMessenger.Default.Send(new ResponseChangedEvent(true));
        });
    }
}