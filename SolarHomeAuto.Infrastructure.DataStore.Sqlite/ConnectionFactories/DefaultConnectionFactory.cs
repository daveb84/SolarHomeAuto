using Microsoft.EntityFrameworkCore;
using SolarHomeAuto.Domain.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite.ConnectionFactories
{
    public class DefaultConnectionFactory : ISqliteConnectionFactory
    {
        private MigrationsState migrations;

        public DefaultConnectionFactory()
        {
            migrations = new MigrationsState();
        }

        public DbContextOptions<SqliteDbContext> GetConnection(DataStoreSettings settings)
        {
            var options = new DbContextOptionsBuilder<SqliteDbContext>()
                .UseSqlite(settings.ConnectionString, x => x.MigrationsAssembly(typeof(SqliteDataStore).Assembly.FullName))
                .Options;

            return options;
        }

        public void EnsureMigrationsApplied(SqliteDbContext context) => migrations.EnsureMigrationsApplied(context);
    }
}
