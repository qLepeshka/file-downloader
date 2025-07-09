using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloaderApp
{
    public class DownloadProgressReporter : IProgress<DownloadProgressInfo>
    {
        private readonly int _lineNumber;
        private readonly object _consoleLock;
        private readonly int _maxFileNameLength;

        public DownloadProgressReporter(int lineNumber, object consoleLock, int maxFileNameLength)
        {
            _lineNumber = lineNumber;
            _consoleLock = consoleLock;
            _maxFileNameLength = maxFileNameLength;
        }

        public void Report(DownloadProgressInfo value)
        {
            lock (_consoleLock)
            {
                Console.SetCursorPosition(0, _lineNumber);

                int barLength = 20;
                int filledChars = (int)Math.Round(value.ProgressPercentage / 100.0 * barLength);
                string progressBar = $"[{new string('#', filledChars)}{new string('-', barLength - filledChars)}]";
                string speed = DownloadProgressInfo.FormatBytes(value.BytesPerSecond);
                string received = DownloadProgressInfo.FormatBytes(value.BytesReceived);
                string total = value.TotalBytesToReceive > 0 ? DownloadProgressInfo.FormatBytes(value.TotalBytesToReceive) : "Unknown";
                string elapsed = value.ElapsedTime.ToString(@"hh\:mm\:ss");
                string output = string.Format(
                    "{0,-" + _maxFileNameLength + "} {1,6:N2}% {2} / {3} {4} {5,10}/s {6,-10} {7}",
                    value.FileName,
                    value.ProgressPercentage,
                    received,
                    total,
                    progressBar,
                    speed,
                    value.Status,
                    elapsed
                );

                Console.Write(output.PadRight(Console.WindowWidth - 1));
            }
        }
    }
}
