using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.PurgeData;
using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.DataStore.Entities;
using System.Linq.Expressions;

namespace SolarHomeAuto.Infrastructure.DataStore
{
    public abstract class EntityFrameworkPurgeDataStoreService : IPurgeDataStore
    {
        private readonly Func<DbContext> dbContextFactory;
        private readonly ILogger logger;

        public EntityFrameworkPurgeDataStoreService(Func<DbContext> dbContextFactory, ILogger logger) 
        {
            this.dbContextFactory = dbContextFactory;
            this.logger = logger;
        }

        public Task PurgeDeviceHistory(Func<List<DeviceHistory>, Task<bool>> process)
        {
            return ProcessBatch<DeviceHistoryEntity, DeviceHistory, DateTime>(
                batchSize: 100,
                orderBy: x => x.Date,
                mapToDomain: x => x.ToDomain(),
                processBatch: process);
        }

        public Task PurgeLogs(Func<List<LogEntry>, Task<bool>> processLogs)
        {
            return ProcessBatch<LogEntity, LogEntry, int>(
                batchSize: 100,
                orderBy: x => x.Id,
                mapToDomain: x => x.ToDomain(),
                processBatch: processLogs);
        }

        public Task PurgeSolarRealTime(Func<List<SolarRealTime>, Task<bool>> process)
        {
            return ProcessBatch<SolarRealTimeEntity, SolarRealTime, DateTime>(
                batchSize: 100,
                orderBy: x => x.Date,
                mapToDomain: x => x.ToDomain(),
                processBatch: process);
        }

        public Task PurgeRemoteCommandMessages(Func<List<RemoteCommandMessage>, Task<bool>> process)
        {
            return ProcessBatch<RemoteCommandMessageEntity, RemoteCommandMessage, DateTime>(
                batchSize: 100,
                orderBy: x => x.Date,
                mapToDomain: x => x.ToDomain(),
                processBatch: process);
        }

        private async Task ProcessBatch<TEntity, TDomain, TOrder>(
            int batchSize, 
            Expression<Func<TEntity, TOrder>> orderBy,
            Func<TEntity, TDomain> mapToDomain,
            Func<List<TDomain>, Task<bool>> processBatch)
            where TEntity : class
        {
            var errors = new List<Exception>();

            using (var dbContext = dbContextFactory())
            {
                var allData = await dbContext.Set<TEntity>()
                    .OrderBy(orderBy)
                    .ToListAsync();

                var skip = 0;

                while (true)
                {
                    var batchEntities = allData
                        .Skip(skip)
                        .Take(batchSize)
                        .ToList();

                    if (!batchEntities.Any())
                    {
                        break;
                    }

                    var batchDomain = batchEntities
                        .Select(mapToDomain)
                        .ToList();

                    var success = false;

                    try
                    {
                        success = await processBatch(batchDomain);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        errors.Add(ex);
                    }
                    finally
                    {
                        skip += batchSize;
                    }

                    if (success)
                    {
                        try
                        {
                            dbContext.Set<TEntity>().RemoveRange(batchEntities);
                            await dbContext.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex);
                        }
                    }
                }
            }

            foreach (var error in errors)
            {
                logger.LogError(error, $"Error occurred whilst purging {typeof(TEntity).Name}");
            }
        }
    }
}
