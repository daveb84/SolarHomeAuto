using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Infrastructure.DataStore.SqlServer;

namespace SolarHomeAuto.Infrastructure.DataStore
{
    public class SqlDataStoreFactory : IDataStoreFactory
    {
        private readonly DataStoreSettings settings;

        public SqlDataStoreFactory(DataStoreSettings settings)
        {
            this.settings = settings;
        }

        public IDataStore CreateStore()
        {
            return CreateStore(settings);
        }

        internal static SqlDataStore CreateStore(DataStoreSettings settings)
        {
            return new SqlDataStore(settings);
        }
    }
}
