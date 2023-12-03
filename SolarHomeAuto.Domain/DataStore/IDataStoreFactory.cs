namespace SolarHomeAuto.Domain.DataStore
{
    public interface IDataStoreFactory
    {
        IDataStore CreateStore();
    }
}
