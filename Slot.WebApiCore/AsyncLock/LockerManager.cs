using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Slot.WebApiCore.AsyncLock
{
    public class LockerManager
    {
        private readonly TimeSpan idleForRemove = TimeSpan.FromMinutes(10);
        private readonly TimeSpan scanFrequency = TimeSpan.FromMinutes(10);
        private readonly ConcurrentDictionary<string, Lazy<Locker>> lockers = new ConcurrentDictionary<string, Lazy<Locker>>();
        private readonly CancellationTokenSource cleanerTokenSource;
        private readonly ILogger<LockerManager> logger;

        public LockerManager(ILoggerFactory loggerFactory)
        {
            cleanerTokenSource = new CancellationTokenSource();
            StartScanForIdle();
            logger = loggerFactory.CreateLogger<LockerManager>();
        }

        public Locker Acquire(string key)
        {
            var locker = lockers.GetOrAdd(key, x => new Lazy<Locker>(() => new Locker(idleForRemove))).Value;
            locker.UpdateLastActiveTime();
            return locker;
        }

        private void StartScanForIdle()
        {
            Task.Run(async () => await ScanForIdle());
        }

        private async Task ScanForIdle()
        {
            while (!cleanerTokenSource.Token.IsCancellationRequested)
            {
                logger.LogInformation("Start scan for idle lockers");
                var now = DateTimeOffset.UtcNow;
                var total = 0;
                var removed = 0;
                foreach (var locker in lockers)
                {
                    total++;
                    if (locker.Value.Value.CheckForIdle(now))
                    {
                        Remove(locker.Key);
                        removed++;
                    }
                }
                logger.LogInformation("Done clean-up for idle lockers, total {0} removed {1}", total, removed);
                await Task.Delay(scanFrequency);
            }
        }

        private void Remove(string key)
        {
            lockers.TryRemove(key, out Lazy<Locker> locker);
        }
    }
}
