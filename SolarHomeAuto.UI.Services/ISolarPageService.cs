using SolarHomeAuto.Domain.DataStore.Filtering;
using SolarHomeAuto.Domain.SolarUsage.Models;

namespace SolarHomeAuto.UI.Services
{
    public interface ISolarPageService
    {
        Task<List<SolarRealTime>> GetSolarRealTime(PagingFilter filter);
    }
}