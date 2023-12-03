using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite.ConnectionFactories
{
    public class MigrationsState
    {
        private bool migrated = false;
        private object migratedLock = new object();

        public void EnsureMigrationsApplied(SqliteDbContext context)
        {
            if (!migrated)
            {
                lock (migratedLock)
                {
                    if (!migrated)
                    {
                        context.Database.Migrate();
                        migrated = true;
                    }
                }
            }
        }

        public void Reset()
        {
            migrated = false;
        }
    }
}
