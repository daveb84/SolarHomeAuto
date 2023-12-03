using System.ComponentModel.DataAnnotations;

namespace SolarHomeAuto.Domain.Monitoring
{
    public enum MonitoringServiceMode
    {
        [Display(Name = "Host")]
        Host,

        [Display(Name = "Remote")]
        Remote,
    }
}
