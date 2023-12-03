namespace SolarHomeAuto.Domain.DataStore
{
    public class DataStoreSettings
    {
        public string ConnectionString { get; set; }
        public bool EnableLogging { get; set; }
        public bool InMemoryDatabase { get; set; }
    }
}
