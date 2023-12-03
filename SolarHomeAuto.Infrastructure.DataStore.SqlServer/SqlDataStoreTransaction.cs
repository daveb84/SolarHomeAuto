using SolarHomeAuto.Domain.DataStore;

namespace SolarHomeAuto.Infrastructure.DataStore.SqlServer
{
    public class SqlDataStoreTransaction : EntityFrameworkDataStoreTransaction
    {
        public SqlDataStoreTransaction(DataStoreSettings settings) : base(settings)
        {
        }

        protected override EntityFrameworkDataStore CreateStore(DataStoreSettings settings)
        {
            return new SqlDataStore(settings);
        }
    }
}
