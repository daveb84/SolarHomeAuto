using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.DataStore;

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite
{
    public class SqlitePurgeDataStoreService : EntityFrameworkPurgeDataStoreService
    {
        public SqlitePurgeDataStoreService(DataStoreSettings dataStoreSettings, ILogger<SqlitePurgeDataStoreService> logger, IDataStoreFactory dataStoreFactory)
            : base(() => SqliteDataStore.CreateDbContext(dataStoreSettings, ((SqliteDataStoreFactory)dataStoreFactory).ConnectionFactory), logger)
        {
        }
    }
}
