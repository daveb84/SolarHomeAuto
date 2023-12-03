using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.ServerApi.Models
{
    public class ServerApiAccount
    {
        [Required]
        public string ApiKey { get; set; }

        [RegularExpression("https?:\\/\\/(.*)", ErrorMessage = "Invalid URL")]
        public string BaseUrl { get; set; }

        public bool Enabled => !string.IsNullOrEmpty(BaseUrl);
    }
}
