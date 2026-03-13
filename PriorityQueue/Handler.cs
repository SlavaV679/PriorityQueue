namespace PriorityQueue
{
    public class Handler: IHandler//<T>  where T : Message//<T>
    {
        private readonly IInMemoryPriorityQueue<Message> _queue;

        public Handler(IInMemoryPriorityQueue<Message> queue)
        {
            _queue = queue;
        }

        public void EnqueueHandle(Message message, int priority)
        {
            _queue.Enqueue(message, priority);
        }

        public Task<Message> DequeueAsync(CancellationToken cancellationToken = default)
        {
            //var message = _queue.DequeueAsync(cancellationToken);            
            return null;
        }
    }
}
