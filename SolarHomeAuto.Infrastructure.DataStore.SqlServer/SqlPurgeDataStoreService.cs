using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.DataStore;

namespace SolarHomeAuto.Infrastructure.DataStore.SqlServer
{
    public class SqlPurgeDataStoreService : EntityFrameworkPurgeDataStoreService
    {
        public SqlPurgeDataStoreService(DataStoreSettings dataStoreSettings, ILogger<SqlPurgeDataStoreService> logger)
            : base(() => SqlDataStore.CreateDbContext(dataStoreSettings), logger)
        {
        }
    }
}
