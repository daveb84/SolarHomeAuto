using Microsoft.EntityFrameworkCore;
using SolarHomeAuto.Domain.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite.ConnectionFactories
{
    public interface ISqliteConnectionFactory
    {
        void EnsureMigrationsApplied(SqliteDbContext context);
        DbContextOptions<SqliteDbContext> GetConnection(DataStoreSettings settings);
    }
}
