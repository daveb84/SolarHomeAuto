using SolarHomeAuto.Domain.DataStore.Filtering;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.SolarUsage.Models;

namespace SolarHomeAuto.UI.Services.Default
{
    public class SolarPageService : ISolarPageService
    {
        private readonly IDataStoreFactory dataStoreFactory;

        public SolarPageService(IDataStoreFactory dataStoreFactory)
        {
            this.dataStoreFactory = dataStoreFactory;
        }

        public async Task<List<SolarRealTime>> GetSolarRealTime(PagingFilter filter)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                return await store.GetSolarRealTime(filter);
            }
        }
    }
}
