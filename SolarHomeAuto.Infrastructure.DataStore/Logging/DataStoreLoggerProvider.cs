using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.DataStore;

namespace SolarHomeAuto.Infrastructure.DataStore.Logging
{
    public class DataStoreLoggerProvider : ILoggerProvider
    {
        private readonly IDataStoreFactory dataStoreFactory;

        public DataStoreLoggerProvider(IDataStoreFactory dataStoreFactory)
        {
            this.dataStoreFactory = dataStoreFactory;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DataStoreLogger(categoryName, dataStoreFactory);
        }

        public void Dispose()
        {
        }
    }
}
