using PriorityQueue;
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
    }
}