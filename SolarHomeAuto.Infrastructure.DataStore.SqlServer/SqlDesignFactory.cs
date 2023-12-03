using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SolarHomeAuto.Infrastructure.DataStore.SqlServer
{
    public class SqlDesignFactory : IDesignTimeDbContextFactory<SqlDbContext>
    {
        public SqlDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Directory.GetCurrentDirectory() + "/../SolarHomeAuto.WebServer/appsettings.json")
                .AddJsonFile(Directory.GetCurrentDirectory() + "/../SolarHomeAuto.WebServer/appsettings.Development.json", true)
                .Build();

            var builder = new DbContextOptionsBuilder<SqlDbContext>();
            var connectionString = configuration.GetSection("SolarHomeAutoWebServer:DataStore:ConnectionString").Value;

            builder.UseSqlServer(connectionString, x => x.MigrationsAssembly(typeof(SqlDesignFactory).Assembly.FullName));

            return new SqlDbContext(builder.Options);
        }
    }
}
