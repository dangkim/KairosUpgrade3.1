using System;
using System.Threading;

namespace Slot.WebApiCore.AsyncLock
{
    public class Locker
    {
        private readonly TimeSpan idleForRemove;

        public Locker(TimeSpan idleForRemove)
        {
            this.idleForRemove = idleForRemove;
            SemaphoreSlim = new SemaphoreSlim(1, 1);
            UpdateLastActiveTime();
        }

        public DateTimeOffset LastActiveTime { get; private set; }
        public SemaphoreSlim SemaphoreSlim { get; private set; }

        public bool CheckForIdle(DateTimeOffset now)
        {
            return (now - LastActiveTime) >= idleForRemove;
        }

        public void UpdateLastActiveTime()
        {
            LastActiveTime = DateTimeOffset.UtcNow;
        }
    }
}
