IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612215527_InitialCreate')
BEGIN
    CREATE TABLE [AuthToken] (
        [Id] int NOT NULL IDENTITY,
        [AccountId] nvarchar(450) NOT NULL,
        [Created] datetime2 NOT NULL,
        [Expires] datetime2 NOT NULL,
        [AccessToken] nvarchar(max) NULL,
        [RefreshToken] nvarchar(max) NULL,
        CONSTRAINT [PK_AuthToken] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612215527_InitialCreate')
BEGIN
    CREATE TABLE [DeviceHistory] (
        [Id] int NOT NULL IDENTITY,
        [DeviceId] varchar(100) NULL,
        [Time] datetime2 NOT NULL,
        [State] varchar(max) NULL,
        [Source] varchar(500) NULL,
        [Error] varchar(500) NULL,
        CONSTRAINT [PK_DeviceHistory] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612215527_InitialCreate')
BEGIN
    CREATE TABLE [Devices] (
        [Id] int NOT NULL IDENTITY,
        [DeviceId] varchar(100) NULL,
        [Name] nvarchar(500) NULL,
        [StateType] varchar(500) NULL,
        [Enabled] bit NOT NULL,
        CONSTRAINT [PK_Devices] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612215527_InitialCreate')
BEGIN
    CREATE TABLE [Log] (
        [Id] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [Level] nvarchar(20) NOT NULL,
        [IpAddress] nvarchar(50) NULL,
        [Url] nvarchar(2000) NULL,
        [Message] nvarchar(max) NULL,
        [Exception] nvarchar(max) NULL,
        [Logger] nvarchar(2000) NULL,
        CONSTRAINT [PK_Log] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612215527_InitialCreate')
BEGIN
    CREATE TABLE [UsageRealTime] (
        [Id] int NOT NULL IDENTITY,
        [Date] date NOT NULL,
        [Time] datetime2 NOT NULL,
        [Production] decimal(18,2) NULL,
        [GridPower] decimal(18,2) NULL,
        [GridPurchasing] bit NOT NULL,
        [BatteryCharging] bit NOT NULL,
        [BatteryPower] decimal(18,2) NULL,
        [Consumption] decimal(18,2) NULL,
        [BatteryCapacity] decimal(18,2) NULL,
        CONSTRAINT [PK_UsageRealTime] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612215527_InitialCreate')
BEGIN
    CREATE TABLE [UsageStats] (
        [Id] int NOT NULL IDENTITY,
        [Time] datetime2 NOT NULL,
        [Duration] nvarchar(max) NOT NULL,
        [Generation] decimal(18,2) NULL,
        [Consumption] decimal(18,2) NULL,
        [GridFeedIn] decimal(18,2) NULL,
        [GridPurchase] decimal(18,2) NULL,
        [ChargeCapacity] decimal(18,2) NULL,
        [DischargeCapacity] decimal(18,2) NULL,
        CONSTRAINT [PK_UsageStats] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612215527_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_AuthToken_AccountId] ON [AuthToken] ([AccountId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612215527_InitialCreate')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_DeviceHistory_DeviceId] ON [DeviceHistory] ([DeviceId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612215527_InitialCreate')
BEGIN
    EXEC(N'CREATE UNIQUE NONCLUSTERED INDEX [IX_Device_DeviceId] ON [Devices] ([DeviceId]) WHERE [DeviceId] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612215527_InitialCreate')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_UsageRealTime_Date] ON [UsageRealTime] ([Date]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612215527_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230612215527_InitialCreate', N'7.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230719133134_DateColumns')
BEGIN
    EXEC sp_rename N'[DeviceHistory].[Time]', N'Date', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230719133134_DateColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230719133134_DateColumns', N'7.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230727084012_RenameAndAddTables')
BEGIN
    DROP TABLE [UsageRealTime];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230727084012_RenameAndAddTables')
BEGIN
    DROP TABLE [UsageStats];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230727084012_RenameAndAddTables')
BEGIN
    CREATE TABLE [ApplicationState] (
        [Id] int NOT NULL,
        [IsBackgroundServiceRunning] bit NOT NULL,
        CONSTRAINT [PK_ApplicationState] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230727084012_RenameAndAddTables')
BEGIN
    CREATE TABLE [SolarExcessJobs] (
        [Id] int NOT NULL IDENTITY,
        [JobId] nvarchar(450) NULL,
        [DelaySeconds] int NOT NULL,
        [Order] int NOT NULL,
        [DeviceId] nvarchar(max) NULL,
        [TurnOnBatteryMinPercent] int NOT NULL,
        [TurnOnGridFeedInGreaterThan] int NOT NULL,
        [TurnOnGridFeedInDuration] int NOT NULL,
        [TurnOnProductionGreaterThan] int NOT NULL,
        [TurnOnProductionDuration] int NOT NULL,
        [TurnOnTurnedOffAtLeastDuration] int NOT NULL,
        [TurnOffGridPurchaseGreaterThan] int NOT NULL,
        [TurnOffGridPurchaseDuration] int NOT NULL,
        [TurnOffProductionLessThan] int NOT NULL,
        [TurnOffProductionDuration] int NOT NULL,
        [TurnOffTurnedOnAtLeastDuration] int NOT NULL,
        CONSTRAINT [PK_SolarExcessJobs] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230727084012_RenameAndAddTables')
BEGIN
    CREATE TABLE [SolarRealTime] (
        [Id] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [Production] decimal(18,2) NULL,
        [GridPower] decimal(18,2) NULL,
        [GridPurchasing] bit NOT NULL,
        [BatteryCharging] bit NOT NULL,
        [BatteryPower] decimal(18,2) NULL,
        [Consumption] decimal(18,2) NULL,
        [BatteryCapacity] decimal(18,2) NULL,
        CONSTRAINT [PK_SolarRealTime] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230727084012_RenameAndAddTables')
BEGIN
    CREATE TABLE [SolarStats] (
        [Id] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [Duration] nvarchar(max) NOT NULL,
        [Generation] decimal(18,2) NULL,
        [Consumption] decimal(18,2) NULL,
        [GridFeedIn] decimal(18,2) NULL,
        [GridPurchase] decimal(18,2) NULL,
        [ChargeCapacity] decimal(18,2) NULL,
        [DischargeCapacity] decimal(18,2) NULL,
        CONSTRAINT [PK_SolarStats] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230727084012_RenameAndAddTables')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_DeviceHistory_Date] ON [DeviceHistory] ([Date]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230727084012_RenameAndAddTables')
BEGIN
    EXEC(N'CREATE UNIQUE NONCLUSTERED INDEX [IX_SolarExcessJob_JobId] ON [SolarExcessJobs] ([JobId]) WHERE [JobId] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230727084012_RenameAndAddTables')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SolarRealTime_Date] ON [SolarRealTime] ([Date]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230727084012_RenameAndAddTables')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SolarStats_Date] ON [SolarStats] ([Date]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230727084012_RenameAndAddTables')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230727084012_RenameAndAddTables', N'7.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230731223205_StateChange')
BEGIN
    ALTER TABLE [DeviceHistory] ADD [IsStateChange] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230731223205_StateChange')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230731223205_StateChange', N'7.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230804123908_DevicesConfig')
BEGIN
    ALTER TABLE [Devices] ADD [Provider] varchar(100) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230804123908_DevicesConfig')
BEGIN
    ALTER TABLE [Devices] ADD [ProviderData] varchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230804123908_DevicesConfig')
BEGIN
    ALTER TABLE [Devices] ADD [SolarJobs] varchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230804123908_DevicesConfig')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230804123908_DevicesConfig', N'7.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230804164528_RemoveTables')
BEGIN
    DROP TABLE [SolarExcessJobs];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230804164528_RemoveTables')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230804164528_RemoveTables', N'7.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230805235202_Settings')
BEGIN
    ALTER TABLE [ApplicationState] ADD [AuthSettings] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230805235202_Settings')
BEGIN
    ALTER TABLE [ApplicationState] ADD [ShellySettings] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230805235202_Settings')
BEGIN
    ALTER TABLE [ApplicationState] ADD [SolarSettings] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230805235202_Settings')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230805235202_Settings', N'7.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230809141934_UpdateSettings')
BEGIN
    DROP INDEX [IX_AuthToken_AccountId] ON [AuthToken];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230809141934_UpdateSettings')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApplicationState]') AND [c].[name] = N'AuthSettings');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ApplicationState] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [ApplicationState] DROP COLUMN [AuthSettings];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230809141934_UpdateSettings')
BEGIN
    EXEC sp_rename N'[ApplicationState].[SolarSettings]', N'ServerApiAccount', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230809141934_UpdateSettings')
BEGIN
    EXEC sp_rename N'[ApplicationState].[ShellySettings]', N'AccountCredentials', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230809141934_UpdateSettings')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AuthToken]') AND [c].[name] = N'AccountId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AuthToken] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [AuthToken] ALTER COLUMN [AccountId] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230809141934_UpdateSettings')
BEGIN
    ALTER TABLE [AuthToken] ADD [ServiceId] varchar(500) NOT NULL DEFAULT '';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230809141934_UpdateSettings')
BEGIN
    CREATE UNIQUE INDEX [IX_AuthToken_ServiceId] ON [AuthToken] ([ServiceId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230809141934_UpdateSettings')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230809141934_UpdateSettings', N'7.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230811084437_SolarImportSettings')
BEGIN
    ALTER TABLE [ApplicationState] ADD [SolarRealTimeImportSchedule] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230811084437_SolarImportSettings')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230811084437_SolarImportSettings', N'7.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230830221606_RemoteCommands')
BEGIN
    ALTER TABLE [ApplicationState] ADD [IsMonitoringWorkerRunning] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230830221606_RemoteCommands')
BEGIN
    ALTER TABLE [ApplicationState] ADD [MonitoringServiceMode] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230830221606_RemoteCommands')
BEGIN
    CREATE TABLE [RemoteCommandMessages] (
        [Id] int NOT NULL IDENTITY,
        [MessageId] uniqueidentifier NOT NULL,
        [Date] datetime2 NOT NULL,
        [Type] varchar(100) NULL,
        [RelatedId] varchar(100) NULL,
        [Data] varchar(max) NULL,
        [Source] varchar(100) NULL,
        [Consumed] bit NOT NULL,
        [ConsumedResult] varchar(max) NULL,
        CONSTRAINT [PK_RemoteCommandMessages] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230830221606_RemoteCommands')
BEGIN
    CREATE UNIQUE INDEX [IX_RemoteCommandMessages_MessageId] ON [RemoteCommandMessages] ([MessageId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230830221606_RemoteCommands')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RemoteCommandMessages_Type] ON [RemoteCommandMessages] ([Type]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230830221606_RemoteCommands')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230830221606_RemoteCommands', N'7.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230927101038_JobState')
BEGIN
    ALTER TABLE [ApplicationState] ADD [HasLanAccess] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230927101038_JobState')
BEGIN
    CREATE TABLE [SystemData] (
        [Id] int NOT NULL IDENTITY,
        [Key] varchar(200) NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_SystemData] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230927101038_JobState')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_SystemData_Key] ON [SystemData] ([Key]) WHERE [Key] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230927101038_JobState')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230927101038_JobState', N'7.0.5');
END;
GO

COMMIT;
GO


