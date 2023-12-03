using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Infrastructure.DataStore.Sqlite.ConnectionFactories;

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite
{
    public class SqliteDataStore : EntityFrameworkDataStore
    {
        private readonly SqliteDataStoreFactory factory;

        public SqliteDataStore(DataStoreSettings settings, SqliteDataStoreFactory factory) : base(settings)
        {
            this.factory = factory;
        }

        protected override IDataStoreDbContext CreateContext(DataStoreSettings settings)
        {
            return CreateDbContext(settings, factory.ConnectionFactory);
        }

        protected override IDataStoreTransaction CreateTransaction(DataStoreSettings settings)
        {
            return new SqliteDataStoreTransaction(settings, factory);
        }

        public static SqliteDbContext CreateDbContext(DataStoreSettings settings, ISqliteConnectionFactory connectionFactory)
        {
            var options = connectionFactory.GetConnection(settings);

            var context = new SqliteDbContext(options);

            connectionFactory.EnsureMigrationsApplied(context);

            return context;
        }
    }
}
