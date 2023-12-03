using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.DataStore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SolarHomeAuto.Domain.Monitoring;

namespace SolarHomeAuto.Infrastructure.DataStore.SqlServer
{
    public class SqlDbContext : DbContext, IDataStoreDbContext
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

        public SqlDbContext(DbContextOptions<SqlDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationStateEntity>().ToTable("ApplicationState").HasKey(x => x.Id);
            builder.Entity<ApplicationStateEntity>().Property(x => x.Id).ValueGeneratedNever();
            builder.Entity<ApplicationStateEntity>().Property(x => x.ServerApiAccount).HasColumnType("nvarchar(max)");
            builder.Entity<ApplicationStateEntity>().Property(x => x.AccountCredentials).HasColumnType("nvarchar(max)");
            builder.Entity<ApplicationStateEntity>().Property(d => d.MonitoringServiceMode)
                .HasConversion(new EnumToStringConverter<MonitoringServiceMode>());

            builder.Entity<SolarStatsEntity>().ToTable("SolarStats").HasKey(x => x.Id);
            builder.Entity<SolarStatsEntity>().Property(d => d.Duration)
                .HasConversion(new EnumToStringConverter<SolarStatsDuration>());
            builder.Entity<SolarStatsEntity>().HasIndex(d => d.Date, "IX_SolarStats_Date").IsUnique(false).IsClustered(false);

            builder.Entity<SolarStatsEntity>().Property(d => d.Generation).HasColumnType("decimal(18,2)");
            builder.Entity<SolarStatsEntity>().Property(d => d.Consumption).HasColumnType("decimal(18,2)");
            builder.Entity<SolarStatsEntity>().Property(d => d.GridFeedIn).HasColumnType("decimal(18,2)");
            builder.Entity<SolarStatsEntity>().Property(d => d.GridPurchase).HasColumnType("decimal(18,2)");
            builder.Entity<SolarStatsEntity>().Property(d => d.ChargeCapacity).HasColumnType("decimal(18,2)");
            builder.Entity<SolarStatsEntity>().Property(d => d.DischargeCapacity).HasColumnType("decimal(18,2)");

            builder.Entity<SolarRealTimeEntity>().ToTable("SolarRealTime").HasKey(x => x.Id);
            builder.Entity<SolarRealTimeEntity>().Property(d => d.Production).HasColumnType("decimal(18,2)");
            builder.Entity<SolarRealTimeEntity>().Property(d => d.GridPower).HasColumnType("decimal(18,2)");
            builder.Entity<SolarRealTimeEntity>().Property(d => d.BatteryPower).HasColumnType("decimal(18,2)");
            builder.Entity<SolarRealTimeEntity>().Property(d => d.Consumption).HasColumnType("decimal(18,2)");
            builder.Entity<SolarRealTimeEntity>().Property(d => d.BatteryCapacity).HasColumnType("decimal(18,2)");
            builder.Entity<SolarRealTimeEntity>().HasIndex(d => d.Date, "IX_SolarRealTime_Date").IsUnique(false).IsClustered(false);

            builder.Entity<AuthTokenEntity>().ToTable("AuthToken").HasKey(x => x.Id);
            builder.Entity<AuthTokenEntity>().HasIndex(x => x.ServiceId).IsUnique();
            builder.Entity<AuthTokenEntity>().Property(x => x.ServiceId).IsRequired().HasColumnType("varchar(500)");

            builder.Entity<LogEntity>().ToTable("Log").HasKey(x => x.Id);
            builder.Entity<LogEntity>().Property(x => x.Level).HasColumnType("nvarchar(20)");
            builder.Entity<LogEntity>().Property(x => x.IpAddress).HasColumnType("nvarchar(50)");
            builder.Entity<LogEntity>().Property(x => x.Url).HasColumnType("nvarchar(2000)");
            builder.Entity<LogEntity>().Property(x => x.Message).HasColumnType("nvarchar(max)");
            builder.Entity<LogEntity>().Property(x => x.Exception).HasColumnType("nvarchar(max)");
            builder.Entity<LogEntity>().Property(x => x.Logger).HasColumnType("nvarchar(2000)");

            builder.Entity<DeviceHistoryEntity>().ToTable("DeviceHistory").HasKey(x => x.Id);
            builder.Entity<DeviceHistoryEntity>().Property(x => x.DeviceId).HasColumnType("varchar(100)");
            builder.Entity<DeviceHistoryEntity>().Property(x => x.Source).HasColumnType("varchar(500)");
            builder.Entity<DeviceHistoryEntity>().Property(x => x.Error).HasColumnType("varchar(500)");
            builder.Entity<DeviceHistoryEntity>().Property(x => x.State).HasColumnType("varchar(max)");
            builder.Entity<DeviceHistoryEntity>().HasIndex(d => d.DeviceId, "IX_DeviceHistory_DeviceId").IsUnique(false).IsClustered(false);
            builder.Entity<DeviceHistoryEntity>().HasIndex(d => d.Date, "IX_DeviceHistory_Date").IsUnique(false).IsClustered(false);

            builder.Entity<DeviceEntity>().ToTable("Devices").HasKey(x => x.Id);
            builder.Entity<DeviceEntity>().Property(x => x.DeviceId).HasColumnType("varchar(100)");
            builder.Entity<DeviceEntity>().Property(x => x.Name).HasColumnType("nvarchar(500)");
            builder.Entity<DeviceEntity>().Property(x => x.StateType).HasColumnType("varchar(500)");
            builder.Entity<DeviceEntity>().Property(x => x.Provider).HasColumnType("varchar(100)");
            builder.Entity<DeviceEntity>().Property(x => x.ProviderData).HasColumnType("varchar(max)");
            builder.Entity<DeviceEntity>().Property(x => x.SolarJobs).HasColumnType("varchar(max)");
            builder.Entity<DeviceEntity>().HasIndex(d => d.DeviceId, "IX_Device_DeviceId").IsUnique(true).IsClustered(false);

            builder.Entity<RemoteCommandMessageEntity>().ToTable("RemoteCommandMessages").HasKey(x => x.Id);
            builder.Entity<RemoteCommandMessageEntity>().Property(x => x.Type).HasColumnType("varchar(100)");
            builder.Entity<RemoteCommandMessageEntity>().Property(x => x.Source).HasColumnType("varchar(100)");
            builder.Entity<RemoteCommandMessageEntity>().Property(x => x.RelatedId).HasColumnType("varchar(100)");
            builder.Entity<RemoteCommandMessageEntity>().Property(x => x.Data).HasColumnType("varchar(max)");
            builder.Entity<RemoteCommandMessageEntity>().Property(x => x.ConsumedResult).HasColumnType("varchar(max)");
            builder.Entity<RemoteCommandMessageEntity>().HasIndex(d => d.Type, "IX_RemoteCommandMessages_Type").IsUnique(false).IsClustered(false);
            builder.Entity<RemoteCommandMessageEntity>().HasIndex(d => d.MessageId, "IX_RemoteCommandMessages_MessageId").IsUnique(true);

            builder.Entity<SystemDataEntity>().ToTable("SystemData").HasKey(x => x.Id);
            builder.Entity<SystemDataEntity>().Property(x => x.Key).HasColumnType("varchar(200)");
            builder.Entity<SystemDataEntity>().Property(x => x.Value).HasColumnType("nvarchar(max)");
            builder.Entity<SystemDataEntity>().HasIndex(d => d.Key, "IX_SystemData_Key").IsUnique(true);
        }
    }
}
