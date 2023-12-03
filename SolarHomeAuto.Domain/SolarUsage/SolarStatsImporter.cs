using SolarHomeAuto.Domain.Solar;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Domain.Utils;

namespace SolarHomeAuto.Domain.SolarUsage
{
    public class SolarStatsImporter
    {
        private readonly ISolarApi api;
        private readonly IDataStoreFactory dataStoreFactory;

        public SolarStatsImporter(ISolarApi api, IDataStoreFactory dataStoreFactory)
        {
            this.api = api;
            this.dataStoreFactory = dataStoreFactory;
        }

        public Task RunImport(SolarStatsDuration duration, DateTime from, DateTime to)
        {
            var fetch = api.GetSolarStats(duration, from, to);

            return BatchProcessor<SolarStats>.Process(fetch, 10, SaveData);
        }

        private async Task SaveData(IEnumerable<SolarStats> data)
        {
            using (var db = dataStoreFactory.CreateStore())
            using (var trans = db.CreateTransaction())
            {
                await trans.SaveSolarStats(data);
                await trans.Commit();
            }
        }
    }
}
