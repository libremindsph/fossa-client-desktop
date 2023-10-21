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

using Russkyc.Configuration;

namespace Fossa.Client.Desktop.Configuration;

public class ModelConfig : ConfigProvider
{
    public ModelConfig(string path) : base(path)
    {
        
    }

    public string Path
    {
        get => GetValue<string>(nameof(Path));
        set => SetValue(nameof(Path), value);
    }

    public string Prompt
    {
        get => GetValue<string>(nameof(Prompt));
        set => SetValue(nameof(Prompt), value);
    }
    
    public int Threads
    {
        get => GetValue<int>(nameof(Threads));
        set => SetValue(nameof(Threads), value);
    }

    public int ContextSize
    {
        get => GetValue<int>(nameof(ContextSize));
        set => SetValue(nameof(ContextSize), value);
    }

    public bool Perplexity
    {
        get => GetValue<bool>(nameof(Perplexity));
        set => SetValue(nameof(Perplexity), value);
    }

    public int TopK
    {
        get => GetValue<int>(nameof(TopK));
        set => SetValue(nameof(TopK), value);
    }

    public float TopP
    {
        get => GetValue<float>(nameof(TopP));
        set => SetValue(nameof(TopP), value);
    }

    public float TypicalP
    {
        get => GetValue<float>(nameof(TypicalP));
        set => SetValue(nameof(TypicalP), value);
    }

    public float Temperature
    {
        get => GetValue<float>(nameof(Temperature));
        set => SetValue(nameof(Temperature), value);
    }

    public float FrequencyPenalty
    {
        get => GetValue<float>(nameof(FrequencyPenalty));
        set => SetValue(nameof(FrequencyPenalty), value);
    }

    public float PresencePenalty
    {
        get => GetValue<float>(nameof(PresencePenalty));
        set => SetValue(nameof(PresencePenalty), value);
    }

    public float RopeFrequencyBase
    {
        get => GetValue<float>(nameof(RopeFrequencyBase));
        set => SetValue(nameof(RopeFrequencyBase), value);
    }

    public float RopeFrequencyScale
    {
        get => GetValue<float>(nameof(RopeFrequencyScale));
        set => SetValue(nameof(RopeFrequencyScale), value);
    }
    
    public string SessionsDirectory
    {
        get => GetValue<string>(nameof(SessionsDirectory));
        set => SetValue(nameof(SessionsDirectory), value);
    }

}