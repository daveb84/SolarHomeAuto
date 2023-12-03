using SolarHomeAuto.Domain.DataStore.Filtering;

namespace SolarHomeAuto.Infrastructure.DataStore.Filtering
{
    public static class PageFilterExtensions
    {
        internal static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, PagingFilter filter)
            where T : IDatePageableEntity
        {
            var paged = query;

            if (filter.From.HasValue)
            {
                paged = paged.Where(x => x.Date >= filter.From.Value);

                if (filter.IncludeEarlierThanFrom > 0)
                {
                    var earlier = query
                        .Where(x => x.Date < filter.From.Value)
                        .OrderByDescending(x => x.Date)
                        .Take(filter.IncludeEarlierThanFrom);

                    paged = paged.Union(earlier);
                }
            }

            if (filter.To.HasValue)
            {
                paged = paged.Where(x => x.Date <= filter.To.Value);
            }

            paged = paged.ApplySort(filter);

            if (filter.Skip.HasValue)
            {
                paged = paged.Skip(filter.Skip.Value);
            }

            if (filter.Take.HasValue)
            {
                paged = paged.Take(filter.Take.Value);
            }

            return paged;
        }

        internal static IQueryable<T> ApplySort<T>(this IQueryable<T> query, PagingFilter filter)
            where T : IDatePageableEntity
        {
            if (filter.NewestFirst)
            {
                query = query.OrderByDescending(x => x.Date)
                    .ThenByDescending(x => x.Id);
            }
            else
            {
                query = query.OrderBy(x => x.Date)
                    .ThenBy(x => x.Id);
            }

            return query;
        }
    }
}
