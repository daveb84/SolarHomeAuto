using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SolarHomeAuto.Domain.Monitoring;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.DataStore.Entities;

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite
{
    public class SqliteDbContext : DbContext, IDataStoreDbContext
    {
        public DbSet<ApplicationStateEntity> ApplicationState { get; set; }
        public DbSet<SolarStatsEntity> SolarStats { get; set; }
        public DbSet<SolarRealTimeEntity> SolarRealTime { get; set; }
        public DbSet<AuthTokenEntity> AuthTokens { get; set; }
        public DbSet<LogEntity> Logs { get; set; }
        public DbSet<DeviceEntity> Devices { get; set; }
        public DbSet<DeviceHistoryEntity> DeviceHistory { get; set; }
        public DbSet<RemoteCommandMessageEntity> RemoteCommandMessages { get; set; }
        public DbSet<SystemDataEntity> SystemData { get; set; }

        public SqliteDbContext(DbContextOptions<SqliteDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationStateEntity>().ToTable("ApplicationState").HasKey(x => x.Id);
            builder.Entity<ApplicationStateEntity>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<ApplicationStateEntity>().Property(d => d.MonitoringServiceMode)
                .HasConversion(new EnumToStringConverter<MonitoringServiceMode>());

            builder.Entity<SolarStatsEntity>().ToTable("SolarStats").HasKey(x => x.Id);
            builder.Entity<SolarStatsEntity>().Property(d => d.Duration)
                .HasConversion(new EnumToStringConverter<SolarStatsDuration>());
            builder.Entity<SolarStatsEntity>().HasIndex(d => d.Date, "IX_SolarStats_Date").IsUnique(false);

            builder.Entity<SolarRealTimeEntity>().ToTable("SolarRealTime").HasKey(x => x.Id);
            builder.Entity<SolarRealTimeEntity>().HasIndex(d => d.Date, "IX_SolarRealTime_Date").IsUnique(false);

            builder.Entity<AuthTokenEntity>().ToTable("AuthToken").HasKey(x => x.Id);
            builder.Entity<AuthTokenEntity>().HasIndex(x => x.ServiceId).IsUnique();
            builder.Entity<AuthTokenEntity>().Property(x => x.ServiceId).IsRequired();

            builder.Entity<LogEntity>().ToTable("Log").HasKey(x => x.Id);

            builder.Entity<DeviceHistoryEntity>().ToTable("DeviceHistory").HasKey(x => x.Id);
            builder.Entity<DeviceHistoryEntity>().HasIndex(d => d.DeviceId, "IX_DeviceHistory_DeviceId").IsUnique(false);
            builder.Entity<DeviceHistoryEntity>().HasIndex(d => d.Date, "IX_DeviceHistory_Date").IsUnique(false);

            builder.Entity<DeviceEntity>().ToTable("Devices").HasKey(x => x.Id);
            builder.Entity<DeviceEntity>().HasIndex(d => d.DeviceId, "IX_Device_DeviceId").IsUnique(false);

            builder.Entity<RemoteCommandMessageEntity>().ToTable("RemoteCommandMessages").HasKey(x => x.Id);
            builder.Entity<RemoteCommandMessageEntity>().HasIndex(d => d.Type, "IX_RemoteCommandMessages_Type").IsUnique(false);
            builder.Entity<RemoteCommandMessageEntity>().HasIndex(d => d.MessageId, "IX_RemoteCommandMessages_MessageId").IsUnique(true);

            builder.Entity<SystemDataEntity>().ToTable("SystemData").HasKey(x => x.Id);
            builder.Entity<SystemDataEntity>().HasIndex(d => d.Key, "IX_SystemData_Key").IsUnique(true);
        }
    }
}
