namespace PriorityQueue
{
    public interface IInMemoryPriorityQueue<T>
    {
        void Enqueue(T item, int priority); // Чем меньше число, тем выше приоритет

        void Enqueue(T item, int priority, TimeSpan delay);

        Task<T> DequeueAsync(CancellationToken cancellationToken = default);

        Task<T> DequeueAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

        bool TryDequeue(out T item);

        int Count { get; }
    }
}
