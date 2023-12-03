using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Messaging;
using SolarHomeAuto.Domain.Solar;
using SolarHomeAuto.Domain.SolarUsage.Models;

namespace SolarHomeAuto.Domain.SolarUsage
{
    public class SolarRealTimeImporter
    {
        private readonly ISolarApi api;
        private readonly IDataStoreFactory dataStoreFactory;
        private readonly IMessageServiceProvider messageProvider;

        public SolarRealTimeImporter(ISolarApi api, IDataStoreFactory dataStoreFactory, IMessageServiceProvider messageProvider)
        {
            this.api = api;
            this.dataStoreFactory = dataStoreFactory;
            this.messageProvider = messageProvider;
        }

        public async Task RunImport()
        {
            var enabled = await api.IsEnabled();

            if (!enabled) return;

            SolarRealTime data = null;
            bool hasChanged = false;

            try
            {
                data = await api.GetSolarRealTime();

                if (data != null)
                {
                    using (var db = dataStoreFactory.CreateStore())
                    {
                        var latest = await db.GetLatestSolarRealTime();

                        if (latest == null || !data.IsSame(latest))
                        {
                            hasChanged = true;

                            using (var trans = db.CreateTransaction())
                            {
                                await trans.SaveSolarRealTime(data);
                                await trans.Commit();
                            }
                        }
                    }
                }
            }
            finally 
            {
                if (data != null && hasChanged)
                {
                    await messageProvider.Publish(MessageTypes.SolarRealTime, data);
                }
            }
        }
    }
}
