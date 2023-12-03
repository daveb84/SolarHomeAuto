using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Infrastructure.DataStore.Sqlite.ConnectionFactories;

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite
{
    public class SqliteDataStoreFactory : IDataStoreFactory
    {
        private readonly DataStoreSettings settings;

        public SqliteDataStoreFactory(DataStoreSettings settings, ISqliteConnectionFactory connectionFactory)
        {
            this.settings = settings;

            ConnectionFactory = connectionFactory;
        }

        public ISqliteConnectionFactory ConnectionFactory { get; }

        public IDataStore CreateStore()
        {
            return new SqliteDataStore(settings, this);
        }
    }
}
