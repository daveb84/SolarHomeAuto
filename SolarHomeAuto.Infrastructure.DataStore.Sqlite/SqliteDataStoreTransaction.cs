using SolarHomeAuto.Domain.DataStore;

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite
{
    public class SqliteDataStoreTransaction : EntityFrameworkDataStoreTransaction
    {
        private readonly SqliteDataStoreFactory factory;

        public SqliteDataStoreTransaction(DataStoreSettings settings, SqliteDataStoreFactory factory) : base(settings)
        {
            this.factory = factory;
        }

        protected override EntityFrameworkDataStore CreateStore(DataStoreSettings settings)
        {
            return new SqliteDataStore(settings, factory);
        }
    }
}
