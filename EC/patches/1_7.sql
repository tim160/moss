USE [EC]
GO

ALTER TABLE [dbo].[report] DROP COLUMN last_update_dt
GO

ALTER TABLE [dbo].[report] ADD last_update_dt datetime2 DEFAULT'1900-01-01 00:00:00' NOT NULL
GO


ALTER TABLE [dbo].[report] DROP COLUMN reported_dt
GO
ALTER TABLE [dbo].[report] DROP COLUMN incident_dt
GO

ALTER TABLE [dbo].[report] ADD reported_dt datetime2 DEFAULT'1900-07-14 00:00:00' NOT NULL
GO

ALTER TABLE [dbo].[report] ADD incident_dt datetime2 DEFAULT'2015-07-14 00:00:00' NOT NULL
GO