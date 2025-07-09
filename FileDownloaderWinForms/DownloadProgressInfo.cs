using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloaderApp
{
    public class DownloadProgressInfo
    {
        public string FileName { get; }
        public long BytesReceived { get; }
        public long TotalBytesToReceive { get; }
        public double ProgressPercentage { get; }
        public TimeSpan ElapsedTime { get; }
        public long BytesPerSecond { get; }
        public string Status { get; }

        public DownloadProgressInfo(string fileName, long bytesReceived, long totalBytesToReceive, TimeSpan elapsedTime, string status = "Downloading")
        {
            FileName = fileName;
            BytesReceived = bytesReceived;
            TotalBytesToReceive = totalBytesToReceive;
            ElapsedTime = elapsedTime;
            Status = status;

            if (totalBytesToReceive > 0)
            {
                ProgressPercentage = (double)bytesReceived / totalBytesToReceive * 100.0;
            }
            else
            {
                ProgressPercentage = 0;
            }

            if (elapsedTime.TotalSeconds > 0)
            {
                BytesPerSecond = (long)(bytesReceived / elapsedTime.TotalSeconds);
            }
            else
            {
                BytesPerSecond = 0;
            }
        }

        public static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;
            double dblSByte = bytes;

            while (Math.Round(dblSByte / 1024) >= 1)
            {
                dblSByte /= 1024;
                i++;
            }

            return string.Format("{0:n1} {1}", dblSByte, Suffix[i]);
        }
    }
}
