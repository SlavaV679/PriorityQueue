using Microsoft.AspNetCore.Mvc;

namespace PriorityQueue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHandler _handler;
        private readonly IInMemoryPriorityQueue<Message> _queue;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHandler handler, IInMemoryPriorityQueue<Message> queue)
        {
            _logger = logger;
            _handler = handler;
            _queue = queue;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        [Route("enqueue")]
        public IActionResult Enqueue()
        {
            var rnd = new Random();
            var message = new Message() { Name = $"nameOfMessage {DateTime.UtcNow}" };
            var priority = rnd.Next(0, 255);
            _queue.Enqueue(message, priority);
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
