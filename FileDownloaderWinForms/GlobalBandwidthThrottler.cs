using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloaderApp
{
    public class GlobalBandwidthThrottler
    {
        private readonly long _maxBytesPerSecond;
        private long _bytesConsumedInInterval;
        private DateTime _lastIntervalStartTime;
        private readonly object _lock = new object();
        private const int IntervalMilliseconds = 1000;

        public GlobalBandwidthThrottler(long maxBytesPerSecond)
        {
            _maxBytesPerSecond = maxBytesPerSecond;
            _lastIntervalStartTime = DateTime.UtcNow;
        }

        public async Task ThrottleAsync(long bytesToConsume)
        {
            if (_maxBytesPerSecond <= 0) return;

            lock (_lock)
            {
                if ((DateTime.UtcNow - _lastIntervalStartTime).TotalMilliseconds >= IntervalMilliseconds)
                {
                    _bytesConsumedInInterval = 0;
                    _lastIntervalStartTime = DateTime.UtcNow;
                }

                _bytesConsumedInInterval += bytesToConsume;
                if (_bytesConsumedInInterval > _maxBytesPerSecond)
                {
                    var timeElapsed = (DateTime.UtcNow - _lastIntervalStartTime).TotalMilliseconds;
                    var estimatedWaitTime = (long)((_bytesConsumedInInterval - _maxBytesPerSecond) / (_maxBytesPerSecond / (double)IntervalMilliseconds));
                    var actualWait = Math.Max(0, (long)(IntervalMilliseconds - timeElapsed));
                    actualWait = Math.Min(actualWait, estimatedWaitTime);

                    if (actualWait > 0)
                    {
                        Monitor.Wait(_lock, TimeSpan.FromMilliseconds(actualWait));
                    }
                }
            }
        }
    }
}
