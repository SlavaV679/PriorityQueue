namespace PriorityQueue
{
    public interface IHandler//<T>// where T:class
    {
        void EnqueueHandle(Message message, int priority);

        public Task<Message> DequeueAsync(CancellationToken cancellationToken = default);
    }
}