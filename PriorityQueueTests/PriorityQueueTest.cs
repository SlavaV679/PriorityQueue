using PriorityQueue;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriorityQueueTests
{
    public class PriorityQueueTest
    {
        [Fact]
        public void TryDequeue_ReturnsCorrectResult()
        {
            //arrange
            var queue = new InMemoryPriorityQueue<int>();
            queue.Enqueue(1, 1);
            queue.Enqueue(2, 1);

            //act
            var countBeforeDequeue = queue.Count;
            var isExistFirstItem = queue.TryDequeue(out int firstItem);
            var isExistSecondItem = queue.TryDequeue(out int secondItem);
            var isExistThirdItem = queue.TryDequeue(out int thirdItem);
            var countAfterDequeue = queue.Count;

            //assert
            Assert.True(isExistFirstItem);
            Assert.True(isExistSecondItem);
            Assert.False(isExistThirdItem);
            Assert.Equal(1, firstItem);
            Assert.Equal(2, secondItem);
            Assert.Equal(0, thirdItem);
            Assert.Equal(2, countBeforeDequeue);
            Assert.Equal(0, countAfterDequeue);
        }

        [Fact]
        public void TryDequeue_CheckCorrectSequence()
        {
            //arrange
            var queue = new InMemoryPriorityQueue<string>();
            queue.Enqueue("A", 1);
            queue.Enqueue("D", 5);
            queue.Enqueue("E", 5);
            queue.Enqueue("B", 1);
            queue.Enqueue("F", 5);
            queue.Enqueue("C", 1);

            //act
            var countBeforeDequeue = queue.Count;
            var isExistFirstItem = queue.TryDequeue(out string firstItem);
            var isExistSecondItem = queue.TryDequeue(out string secondItem);
            var isExistThirdItem = queue.TryDequeue(out string thirdItem);
            var isExistfourthItem = queue.TryDequeue(out string fourthItem);
            var isExistfifthItem = queue.TryDequeue(out string fifthItem);
            var isExistsixthItem = queue.TryDequeue(out string sixthItem);
            var countAfterDequeue = queue.Count;

            //assert
            Assert.True(isExistFirstItem);
            Assert.Equal("A", firstItem);
            Assert.Equal("B", secondItem);
            Assert.Equal("C", thirdItem);
            Assert.Equal("D", fourthItem);
            Assert.Equal("E", fifthItem);
            Assert.Equal("F", sixthItem);
            Assert.Equal(6, countBeforeDequeue);
            Assert.Equal(0, countAfterDequeue);
        }

        [Fact]
        public async Task EnqueueWithDelay_ReturnsCorrectResultAfterDelay()
        {
            //arrange
            var queue = new InMemoryPriorityQueue<string>();
            queue.Enqueue("A", 1, TimeSpan.FromMilliseconds(500));

            //act
            var isExistBeforeDelay = queue.TryDequeue(out string defaultValue);
            await Task.Delay(TimeSpan.FromMilliseconds(700));
            var isExistAfterDelay = queue.TryDequeue(out string actualValue);

            //assert
            Assert.False(isExistBeforeDelay);
            Assert.True(isExistAfterDelay);
            Assert.Equal("A", actualValue);
        }

        [Fact]
        public async Task ParalelEnqueueAndDequeue_CorrectResult()
        {
            //arrange
            var queue = new InMemoryPriorityQueue<int>();
            var produsersRang = Enumerable.Range(1, 10);
            var dequeueTasks = new List<Task>();
            var actualResultList = new List<int>();

            foreach (var threadId in produsersRang)
            {
                dequeueTasks.Add(queue.DequeueAsync());
            }

            var enqueueTasks = new List<Task>();

            foreach (var threadId in produsersRang)
            {
                var task = Task.Run(() =>
                {
                    queue.Enqueue(threadId, 1);
                });

                enqueueTasks.Add(task);
            }

            //act
            await Task.WhenAll(enqueueTasks);

            var countAfterEnqueue = queue.Count;

            var hashSet = new HashSet<int>();


            await Task.WhenAll(dequeueTasks);
            var countAfterDequeue = queue.Count;
            foreach (var dequeueTask in dequeueTasks)
            {
                // тут хочу вытащить значения из dequeueTask, но не понимаю как.
                //await dequeueTask;
                //actualResultList.Add(dequeueTask.);
            }
            //assert
            Assert.Equal(10, countAfterEnqueue);
            Assert.Equal(0, countAfterDequeue);
            Assert.Equal(10, dequeueTasks.Count);
            Assert.Equal(10, dequeueTasks.Capacity);
        }

        [Fact]
        public async Task ParallelEnqueueAndDequeue_CorrectResult_2()
        {
            //arrange
            var queue = new InMemoryPriorityQueue<int>();
            var totalMessages = 1000;
            var producerTasks = Enumerable.Range(0, 10);
            var consumerTasks = Enumerable.Range(1, totalMessages);
            var tasks = new List<Task>();
            var dictionary = new ConcurrentDictionary<int, byte>();

            foreach (var consumerId in consumerTasks)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var message = await queue.DequeueAsync(TimeSpan.FromSeconds(10));
                    if (!dictionary.TryAdd(message, 1))
                        throw new DuplicateWaitObjectException();
                }));
            }

            foreach (var threadId in producerTasks)
            {
                var messagesRange = Enumerable.Range(1, 100);

                foreach (var message in messagesRange)
                {
                    var task = Task.Run(() =>
                    {
                        queue.Enqueue(threadId * 100 + message, 1);
                    });

                    tasks.Add(task);
                }
            }

            //act
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                var v = ex.Message;
                throw;
            }

            //assert
            Assert.Equal(totalMessages, dictionary.Count);
            Assert.Equal(0, queue.Count);
        }

        [Fact]
        public async Task ParallelEnqueueAndDequeue_CorrectResult_3()
        {
            //arrange
            var queue = new InMemoryPriorityQueue<int>();
            var totalMessages = 1000;
            var producerTasks = Enumerable.Range(0, 10);
            var consumerTasks = Enumerable.Range(1, totalMessages);
            var tasks = new List<Task>();
            var dictionary = new ConcurrentDictionary<int, byte>();

            foreach (var consumerId in consumerTasks)
            {
                tasks.Add(ConsumeOneAsync(queue, dictionary));
            }

            foreach (var threadId in producerTasks)
            {
                var messagesRange = Enumerable.Range(1, 100);

                var task = Task.Run(() =>
                {
                    foreach (var message in messagesRange)
                    {
                        queue.Enqueue(threadId * 100 + message, 1);
                    }
                });

                tasks.Add(task);
            }

            //act
            await Task.WhenAll(tasks);


            //assert
            foreach (var item in consumerTasks)
            {
                Assert.True(dictionary.ContainsKey(item));
            }

            Assert.Equal(totalMessages, dictionary.Count);
            Assert.Equal(0, queue.Count);
        }

        private async Task ConsumeOneAsync(InMemoryPriorityQueue<int> queue, ConcurrentDictionary<int, byte> dictionary)
        {
            var message = await queue.DequeueAsync(TimeSpan.FromSeconds(30));
            if (!dictionary.TryAdd(message, 1))
                throw new InvalidOperationException($"Duplicate message received:{message}");
        }
    }
}