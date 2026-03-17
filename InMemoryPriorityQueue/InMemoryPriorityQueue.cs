namespace InMemoryPriorityQueue
{
    public class InMemoryPriorityQueue<T> : IInMemoryPriorityQueue<T>
    {
        private readonly SemaphoreSlim _slim = new SemaphoreSlim(0, int.MaxValue);
        private readonly PriorityQueue<T, PriorityKey> _queue = new PriorityQueue<T, PriorityKey>();
        private readonly object _locker = new object();
        private long sequence = 0;

        public int Count
        {
            get
            {
                lock (_locker)
                {
                    return _queue.Count;
                }
            }
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            await _slim.WaitAsync(cancellationToken);
            T item;
            lock (_locker)
            {
                item = _queue.Dequeue();
            }
            return item;
        }

        public async Task<T> DequeueAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            T item = default;
            if (!await _slim.WaitAsync(timeout, cancellationToken))
                throw new TimeoutException("Waiting is canceled due to timeout");

            lock (_locker)
            {
                item = _queue.Dequeue();
            }
            return item;
        }

        public void Enqueue(T item, int priority)
        {
            lock (_locker)
            {
                var priorityKey = new PriorityKey(priority, ++sequence);
                _queue.Enqueue(item, priorityKey);
            }
            _slim.Release();
        }

        public void Enqueue(T item, int priority, TimeSpan delay)
        {
            if (delay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(delay));

            if (delay == TimeSpan.Zero)
            {
                Enqueue(item, priority);
                return;
            }

            EnqueueAsync(item, priority, delay);
        }

        private async Task EnqueueAsync(T item, int priority, TimeSpan delay)
        {
            await Task.Delay(delay);
            Enqueue(item, priority);
        }

        public bool TryDequeue(out T item)
        {
            item = default;

            if (!_slim.Wait(0))
                return false;

            lock (_locker)
            {
                item = _queue.Dequeue();
            }

            return true;
        }
    }
}
