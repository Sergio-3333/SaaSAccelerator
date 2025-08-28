

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118203340_Baseline_v5'
)
BEGIN

    INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Hourly')
    INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Daily')
    INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Weekly')
    INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Monthly')
    INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Yearly')

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118203340_Baseline_v5'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221118203340_Baseline_v5', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118211554_Baseline_v6'
)
BEGIN
    ALTER TABLE [MeteredAuditLogs] ADD [RunBy] varchar(255) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118211554_Baseline_v6'
)
BEGIN

    IF NOT EXISTS (SELECT * FROM [dbo].[SchedulerFrequency] WHERE [Frequency] = 'OneTime')
    BEGIN
        INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('OneTime')
    END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118211554_Baseline_v6'
)
BEGIN

    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableHourlyMeterSchedules', N'False', N'This will enable to run Hourly meter scheduled items')
    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableDailyMeterSchedules', N'False', N'This will enable to run Daily meter scheduled items')
    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableWeeklyMeterSchedules', N'False', N'This will enable to run Weekly meter scheduled items')
    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableMonthlyMeterSchedules', N'False', N'This will enable to run Monthly meter scheduled items')
    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableYearlyMeterSchedules', N'False', N'This will enable to run Yearly meter scheduled items')
    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableOneTimeMeterSchedules', N'False', N'This will enable to run OneTime meter scheduled items')
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118211554_Baseline_v6'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221118211554_Baseline_v6', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230726232155_Baseline_v7'
)
BEGIN

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230726232155_Baseline_v7'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230726232155_Baseline_v7', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230912052848_SubscriptionDetails_v740'
)
BEGIN
    ALTER TABLE [Subscriptions] ADD [AmpOfferId] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230912052848_SubscriptionDetails_v740'
)
BEGIN
    ALTER TABLE [Subscriptions] ADD [EndDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230912052848_SubscriptionDetails_v740'
)
BEGIN
    ALTER TABLE [Subscriptions] ADD [StartDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230912052848_SubscriptionDetails_v740'
)
BEGIN
    ALTER TABLE [Subscriptions] ADD [Term] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230912052848_SubscriptionDetails_v740'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230912052848_SubscriptionDetails_v740', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20231115232155_Baseline_v741'
)
BEGIN

                        IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'IsMeteredBillingEnabled')
                        BEGIN
                            INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'IsMeteredBillingEnabled', N'true', N'Enable Metered Billing Feature')
                        END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20231115232155_Baseline_v741'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231115232155_Baseline_v741', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240312055030_baseline751'
)
BEGIN

                        IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'ValidateWebhookJwtToken')
                        BEGIN
                            INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'ValidateWebhookJwtToken', N'true', N'Validates JWT token when webhook event is recieved.')
                        END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240312055030_baseline751'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240312055030_baseline751', N'8.0.6');
END;
GO

COMMIT;
GO

