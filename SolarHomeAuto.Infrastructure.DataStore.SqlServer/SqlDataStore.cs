using Microsoft.EntityFrameworkCore;
using SolarHomeAuto.Domain.DataStore;

namespace SolarHomeAuto.Infrastructure.DataStore.SqlServer
{
    public class SqlDataStore : EntityFrameworkDataStore
    {
        public SqlDataStore(DataStoreSettings settings) : base(settings)
        {
        }

        protected override SqlDbContext CreateContext(DataStoreSettings settings) => CreateDbContext(settings);

        public static SqlDbContext CreateDbContext(DataStoreSettings settings)
        {
            var options = new DbContextOptionsBuilder<SqlDbContext>().UseSqlServer(settings.ConnectionString).Options;
            return new SqlDbContext(options);
        }

        protected override IDataStoreTransaction CreateTransaction(DataStoreSettings settings)
        {
            return new SqlDataStoreTransaction(settings);
        }
    }
}
