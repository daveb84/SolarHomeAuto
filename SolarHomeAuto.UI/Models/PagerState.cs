namespace SolarHomeAuto.UI.Models
{
    public class PagerState
    {
        public int ItemsOnPage { get; private set; }
        public int Skip { get; private set; }
        public int Take { get; private set; }

        public int PageSize => Take;

        public bool HasPrevious => Skip > 0;
        public bool HasNext => ItemsOnPage >= Take;

        public int ShowingFrom => Skip + 1;
        public int ShowingTo => Skip + ItemsOnPage;

        public void Update(PagerFetchParams fetch, int itemsOnPage)
        {
            Skip = fetch.Skip;
            Take = fetch.Take;
            ItemsOnPage = itemsOnPage;
        }

        public PagerFetchParams NextPage()
        {
            return new PagerFetchParams
            {
                Skip = Skip + PageSize,
                Take = Take,
            };
        }

        public PagerFetchParams PreviousPage()
        {
            var skip = Skip - PageSize;
            if (skip < 0)
            {
                skip = 0;
            }

            return new PagerFetchParams
            {
                Skip = skip,
                Take = Take,
            };
        }
    }
}
