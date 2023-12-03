using Microsoft.EntityFrameworkCore;
using SolarHomeAuto.Infrastructure.DataStore.Entities;

namespace SolarHomeAuto.Infrastructure.DataStore
{
    public interface IDataStoreDbContext : IDisposable
    {
        DbSet<ApplicationStateEntity> ApplicationState { get; set; }
        DbSet<AuthTokenEntity> AuthTokens { get; set; }
        DbSet<DeviceHistoryEntity> DeviceHistory { get; set; }
        DbSet<DeviceEntity> Devices { get; set; }
        DbSet<LogEntity> Logs { get; set; }
        DbSet<SolarRealTimeEntity> SolarRealTime { get; set; }
        DbSet<SolarStatsEntity> SolarStats { get; set; }
        DbSet<RemoteCommandMessageEntity> RemoteCommandMessages { get; set; }
        DbSet<SystemDataEntity> SystemData { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}