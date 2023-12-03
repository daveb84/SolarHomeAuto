using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite
{
    public class SqliteDesignFactory : IDesignTimeDbContextFactory<SqliteDbContext>
    {
        public SqliteDbContext CreateDbContext(string[] args)
        {
            var path = Path.Combine(Path.GetFullPath(Directory.GetCurrentDirectory()), "SolarHomeAutoSqlLite.db3");

            var builder = new DbContextOptionsBuilder<SqliteDbContext>();
            var connectionString = "Filename=" + path;

            builder.UseSqlite(connectionString, x => x.MigrationsAssembly(typeof(SqliteDesignFactory).Assembly.FullName));
            return new SqliteDbContext(builder.Options);
        }
    }
}
