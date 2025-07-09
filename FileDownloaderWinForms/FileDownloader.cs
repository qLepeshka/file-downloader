using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloaderApp
{
    public class FileDownloader
    {
        private readonly HttpClient _httpClient;
        private readonly GlobalBandwidthThrottler _throttler;
        private readonly DownloadProgressReporter _progressReporter;

        public Action<object, int> ProgressChanged { get; internal set; }

        public FileDownloader(HttpClient httpClient, GlobalBandwidthThrottler throttler, DownloadProgressReporter progressReporter)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _throttler = throttler ?? throw new ArgumentNullException(nameof(throttler));
            _progressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));
        }
        public FileDownloader()
        {
        }

        public FileDownloader(GlobalBandwidthThrottler throttler)
        {
            _httpClient = new HttpClient();
            _throttler = throttler;
        }

        public async Task DownloadFileAsync(string url, string destinationPath, IProgress<DownloadProgressInfo> progress, CancellationToken cancellationToken = default)
        {
            string fileName = Path.GetFileName(destinationPath);
            long totalBytesToReceive = 0;
            long bytesReceived = 0;
            DateTime startTime = DateTime.UtcNow;

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Head, url))
                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    if (response.Content.Headers.ContentLength.HasValue)
                    {
                        totalBytesToReceive = response.Content.Headers.ContentLength.Value;
                    }
                }

                using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    if (totalBytesToReceive == 0 && response.Content.Headers.ContentLength.HasValue)
                    {
                        totalBytesToReceive = response.Content.Headers.ContentLength.Value;
                    }

                    string directory = Path.GetDirectoryName(destinationPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await _throttler.ThrottleAsync(bytesRead);

                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                            bytesReceived += bytesRead;
                            progress?.Report(new DownloadProgressInfo(fileName, bytesReceived, totalBytesToReceive, DateTime.UtcNow - startTime, "Downloading"));
                        }
                    }
                }
                progress?.Report(new DownloadProgressInfo(fileName, bytesReceived, totalBytesToReceive, DateTime.UtcNow - startTime, "Completed"));
            }
            catch (OperationCanceledException)
            {
                progress?.Report(new DownloadProgressInfo(fileName, bytesReceived, totalBytesToReceive, DateTime.UtcNow - startTime, "Canceled"));
                File.Delete(destinationPath);
            }
            catch (Exception ex)
            {
                progress?.Report(new DownloadProgressInfo(fileName, bytesReceived, totalBytesToReceive, DateTime.UtcNow - startTime, $"Error: {ex.Message}"));
                File.Delete(destinationPath);
            }
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        internal async Task DownloadFileAsync(string urlToDownload, string fullPath, Progress<int> progressReporter)
        {
            throw new NotImplementedException();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
