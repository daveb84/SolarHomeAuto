namespace SolarHomeAuto.Domain.DataStore.Filtering
{
    public class PagingFilter
    {
        public bool NewestFirst { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public int IncludeEarlierThanFrom { get; set; }

        public static PagingFilter Default
        {
            get
            {
                return new PagingFilter
                {
                    NewestFirst = true,
                    Skip = 0,
                    Take = 20
                };
            }
        }
    }
}
