namespace InMemoryPriorityQueue
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
}
