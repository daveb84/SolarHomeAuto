using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SolarHomeAuto.Domain.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite.ConnectionFactories
{
    public class InMemoryConnectionFactory : ISqliteConnectionFactory, IDisposable
    {
        private MigrationsState migrations;

        private SqliteConnection inMemoryConnection = null;
        private object inMemoryConnectionLock = new object();

        public InMemoryConnectionFactory()
        {
            migrations = new MigrationsState();
        }

        public void EnsureMigrationsApplied(SqliteDbContext context) => migrations.EnsureMigrationsApplied(context);

        public DbContextOptions<SqliteDbContext> GetConnection(DataStoreSettings settings)
        {
            if (inMemoryConnection == null)
            {
                lock (inMemoryConnectionLock)
                {
                    if (inMemoryConnection == null)
                    {
                        inMemoryConnection = new SqliteConnection("Filename=:memory:");
                        inMemoryConnection.Open();
                    }
                }
            }

            var options = new DbContextOptionsBuilder<SqliteDbContext>()
                .UseSqlite(inMemoryConnection, x => x.MigrationsAssembly(typeof(SqliteDataStore).Assembly.FullName))
                .Options;

            return options;
        }

        public void CloseConnection()
        {
            if (inMemoryConnection != null)
            {
                lock (inMemoryConnectionLock)
                {
                    if (inMemoryConnection != null)
                    {
                        try
                        {
                            migrations.Reset();
                            inMemoryConnection.Close();
                            inMemoryConnection.Dispose();
                        }
                        finally
                        {
                            inMemoryConnection = null;
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            inMemoryConnection?.Dispose();
        }
    }
}
