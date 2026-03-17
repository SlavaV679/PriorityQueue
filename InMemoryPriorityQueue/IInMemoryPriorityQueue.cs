namespace InMemoryPriorityQueue
{
    public interface IInMemoryPriorityQueue<T>
    {
        /// <summary>
        /// Adds an item to the queue with the specified priority.
        /// </summary>
        /// <param name="item">The item to add to the queue.</param>
        /// <param name="priority">The priority of the item. Lower values indicate higher priority. Items with the same priority are processed
        /// in the order they were enqueued.</param>
        void Enqueue(T item, int priority); // Чем меньше число, тем выше приоритет

        /// <summary>
        /// Adds an item to the queue with the specified priority and an optional delay before it becomes available for
        /// processing.
        /// </summary>
        /// <param name="item">The item to enqueue. Cannot be <c>null</c> if <typeparamref name="T"/> is a reference type.</param>
        /// <param name="priority">The priority of the item. Higher values indicate higher priority.</param>
        /// <param name="delay">The amount of time to wait before the item becomes available for dequeuing. Specify <see cref="TimeSpan.Zero"/> for
        /// no delay.</param>
        void Enqueue(T item, int priority, TimeSpan delay);

        /// <summary>
        /// Asynchronously removes and returns the item at the front of the queue.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the dequeue operation.</param>
        /// <returns>A task that represents the asynchronous dequeue operation. The task result contains the item removed from
        /// the front of the queue.</returns>
        Task<T> DequeueAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously removes and returns the next item from the queue, waiting up to the specified timeout if the
        /// queue is empty.
        /// </summary>
        /// <param name="timeout">The maximum duration to wait for an item to become available in the queue. Must be non-negative.</param>
        /// <param name="cancellationToken">A token to observe while waiting for an item to become available. The operation is canceled if the token is
        /// signaled.</param>
        /// <returns>A task that represents the asynchronous dequeue operation. The task result contains the next item in the
        /// queue if one becomes available within the specified timeout.</returns>
        Task<T> DequeueAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempts to remove and return the object at the beginning of the collection.    
        /// </summary>
        /// <param name="item">When this method returns, contains the object removed from the beginning of the collection, if the operation
        /// was successful; otherwise, the default value of <typeparamref name="T"/>.</param>
        /// <returns><see langword="true"/> if an object was successfully removed and returned; otherwise, <see
        /// langword="false"/> if the collection was empty.</returns>
        bool TryDequeue(out T item);

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        int Count { get; }
    }
}
