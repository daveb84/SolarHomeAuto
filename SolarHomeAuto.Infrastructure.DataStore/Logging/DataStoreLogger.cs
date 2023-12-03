using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.Logging.Models;

namespace SolarHomeAuto.Infrastructure.DataStore.Logging
{
    public class DataStoreLogger : LogWriterBase
    {
        private readonly IDataStoreFactory dataStoreFactory;

        public DataStoreLogger(string loggerName, IDataStoreFactory dataStoreFactory)
            : base(loggerName)
        {
            this.dataStoreFactory = dataStoreFactory;
        }

        protected override void WriteLog(LogEntry log)
        {
            Task.Run(() => WriteLogAsync(new List<LogEntry> { log }));
        }

        private async Task WriteLogAsync(IEnumerable<LogEntry> logs)
        {
            try
            {
                using (var store = dataStoreFactory.CreateStore())
                {
                    using (var trans = store.CreateTransaction())
                    {
                        await trans.SaveLogs(logs);
                        await trans.Commit();
                    }
                }
            }
            // swallow exceptions if we can't log to avoid infinate loops
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write logs to database: {ex}");
            }
        }
    }
}
