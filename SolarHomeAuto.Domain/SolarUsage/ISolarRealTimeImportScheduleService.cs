using SolarHomeAuto.Domain.SolarUsage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.SolarUsage
{
    public interface ISolarRealTimeImportScheduleService
    {
        Task<List<SolarRealTimeSchedulePeriod>> GetSolarRealTimeImportSchedule();
    }
}
