using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PriorityQueue
{
    internal struct PriorityKey : IComparable<PriorityKey>
    {
        public int Priority { get; }
        public long Sequence { get; }

        private const int MAX_PRIORITY = 255;
        private const int MIN_PRIORITY = 0;

        public PriorityKey(int priority, long sequence)
        {
            if (priority < MIN_PRIORITY || priority > MAX_PRIORITY)
                throw new ArgumentOutOfRangeException(nameof(priority), $"Not valid value of 'priority' = {priority}");

            Priority = priority;
            Sequence = sequence;
        }

        public int CompareTo(PriorityKey priorityKey)
        {
            var comparer = this.Priority.CompareTo(priorityKey.Priority);
            if (comparer != 0)
                return comparer;

            return this.Sequence.CompareTo(priorityKey.Sequence);
        }
    }

    public class InMemoryPriorityQueue<T> : IInMemoryPriorityQueue<T>//  where T : Message //
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
            // queue.Enqueue(new Message() { Name ="example 1"}, 1);
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
