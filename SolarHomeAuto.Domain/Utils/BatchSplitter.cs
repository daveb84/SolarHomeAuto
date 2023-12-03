namespace SolarHomeAuto.Domain.Utils
{
    public static class BatchSplitter<T>
        where T : IComparable<T>, IEquatable<T>
    {
        public static IEnumerable<(T from, T to)> GetBatches(T from, T to, int batchSize, Func<T, int, T> increment, bool inclusive)
        {
            var batch = NextBatch(from, to, batchSize, increment);

            yield return batch;

            while (!batch.to.Equals(to))
            {
                var nextFrom = inclusive
                    ? increment(batch.to, 1)
                    : batch.to;

                batch = NextBatch(nextFrom, to, batchSize, increment);

                yield return batch;
            }
        }

        private static (T from, T to) NextBatch(T from, T to, int batchSize, Func<T, int, T> increment)
        {
            var endOfBatch = increment(from, batchSize);
            if (endOfBatch.CompareTo(to) > 0)
            {
                endOfBatch = to;
            }

            return (from, endOfBatch);
        }
    }
}
