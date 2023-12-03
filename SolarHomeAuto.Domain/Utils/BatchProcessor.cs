namespace SolarHomeAuto.Domain.Utils
{
    internal class BatchProcessor<T>
    {
        public static async Task Process(IAsyncEnumerable<T> fetch, int batchSize, Func<IEnumerable<T>, Task> process)
        {
            var results = new List<T>();

            await foreach(var item in fetch) 
            { 
                results.Add(item);

                if (results.Count == batchSize)
                {
                    await process(results);
                    results.Clear();
                }
            }

            if (results.Any())
            {
                await process(results);
            }
        }
    }
}
