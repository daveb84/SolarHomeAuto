namespace SolarHomeAuto.Infrastructure.DataStore.Filtering
{
    internal interface IDatePageableEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }
}
