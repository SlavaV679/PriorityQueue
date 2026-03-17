using InMemoryPriorityQueue;
using Microsoft.AspNetCore.Mvc;

namespace PriorityQueue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly ILogger<QueueController> _logger;
        private readonly IInMemoryPriorityQueue<Message> _queue;

        public QueueController(ILogger<QueueController> logger, IInMemoryPriorityQueue<Message> queue)
        {
            _logger = logger;
            _queue = queue;
        }

        [HttpPost]
        [Route("enqueue")]
        public IActionResult Enqueue()
        {
            var rnd = new Random();
            var message = new Message() { Name = $"nameOfMessage {DateTime.UtcNow}" };
            var priority = rnd.Next(0, 255);
            _queue.Enqueue(message, priority);
            _logger.LogInformation($"В очереди {_queue.Count} сообщения(й)");
            return Ok(message);
        }

        [HttpPost]
        [Route("enqueuedelay")]
        public IActionResult EnqueueWithDelay()
        {
            var rnd = new Random();
            var message = new Message() { Name = $"nameOfMessage {DateTime.UtcNow}" };
            var priority = rnd.Next(0, 255);
            _queue.Enqueue(message, priority, TimeSpan.FromSeconds(3));
            return Ok(message);
        }

        [HttpGet]
        [Route("dequeueasync")]
        public async Task<IActionResult> DequeueAsync()
        {
            var message = await _queue.DequeueAsync();
            return Ok(message);
        }

        [HttpGet]
        [Route("dequeuetimeout")]
        public async Task<IActionResult> DequeueAsyncWithTimeout()
        {
            var message = await _queue.DequeueAsync(TimeSpan.FromSeconds(3));
            return Ok(message);
        }

        [HttpGet]
        [Route("trydequeue")]
        public IActionResult TryDequeue()
        {
            _queue.TryDequeue(out Message message);
            return Ok(message);
        }
    }
}
