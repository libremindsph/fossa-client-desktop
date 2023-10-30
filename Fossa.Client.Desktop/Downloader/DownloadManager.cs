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
using Downloader;
using Fossa.Client.Desktop.Configuration;

namespace Fossa.Client.Desktop.Downloader;

public class DownloadManager
{
    private readonly AppConfig _appConfig;
    private readonly DownloadConfiguration _configuration;

    public DownloadManager(AppConfig appConfig)
    {
        _appConfig = appConfig;
        _configuration = new DownloadConfiguration
        {
            ChunkCount = 8,
            ParallelDownload = true,
            ClearPackageOnCompletionWithFailure = true,
            ReserveStorageSpaceBeforeStartingDownload = false
        };
    }

    public async Task DownloadAsync(string url, string filename, Action<string> started, Action<string> completed, Action<(string, double)> progressChanged)
    {
        using var downloadService = new DownloadService(_configuration);
        
        downloadService.DownloadStarted += (_,_) => started(filename);
        downloadService.DownloadFileCompleted += (_, _) => completed(filename);
        downloadService.DownloadProgressChanged += (_, args) => progressChanged((filename,args.ProgressPercentage));
        
        await downloadService.DownloadFileTaskAsync(url, _appConfig.ModelsDirectory + filename);
    }
}