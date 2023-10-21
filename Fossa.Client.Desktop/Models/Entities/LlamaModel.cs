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


using Fossa.Client.Desktop.Configuration;

namespace Fossa.Client.Desktop.Models.Entities;

public class LlamaModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string DownloadSource { get; set; }
    public string FileName { get; set; }
    public string FileSize { get; set; }
    public string ParameterSize { get; set; }

    public string Prompt { get; set; }
    public int ContextSize { get; set; }
    public bool Perplexity { get; set; }
    public int TopK { get; set; }
    public float TopP { get; set; }
    public float TypicalP { get; set; }
    public float TfsZ { get; set; }
    public float RepeatPenalty { get; set; }
    public float Temperature { get; set; }
    public float FrequencyPenalty { get; set; }
    public float PresencePenalty { get; set; }
    public string SessionsDirectory { get; set; }
}