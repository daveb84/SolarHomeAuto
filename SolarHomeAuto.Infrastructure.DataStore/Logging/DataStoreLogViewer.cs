using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.Logging;

namespace SolarHomeAuto.Infrastructure.DataStore.Logging
{
    public class DataStoreLogViewer : ILogViewer
    {
        private readonly IDataStoreFactory dataStoreFactory;

        public DataStoreLogViewer(IDataStoreFactory dataStoreFactory)
        {
            this.dataStoreFactory = dataStoreFactory;
        }

        public async Task<List<LogEntry>> Get(LogFilter filter)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var logs = await store.GetLogs(filter);

                return logs;
            }
        }
    }
}
