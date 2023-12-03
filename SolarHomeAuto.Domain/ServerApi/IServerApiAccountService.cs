using SolarHomeAuto.Domain.ServerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.ServerApi
{
    public interface IServerApiAccountService
    {
        Task<ServerApiAccount> GetServerApiAccount();
    }
}
