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
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Fossa.Client.Desktop.Conversation.Entities;
using Fossa.Client.Desktop.Conversation.Interfaces;

namespace Fossa.Client.Desktop.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<IConversationItem> _conversationItems = new();

    public ChatViewModel()
    {
        int t = 0;
        while (t < 20)
        {
            var rand = new Random().Next(3);
            ConversationItems.Add(
                rand switch
                {
                    0 => new BotMessage
                    {
                        Message = "Bot Message"
                    },
                    1 => new ClearContextMessage
                    {
                        Message = "End of Context"
                    },
                    2 => new UserMessage
                    {
                        Message = "User Message"
                    },
                    _ => new UserMessage
                    {
                        Message = "User Message"
                    }
                });
            t++;
        }
    }
}