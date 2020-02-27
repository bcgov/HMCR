-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-01
-- Updates: 
-- 
-- Description:	Database snapshot modifications in support of IS5, to enable larger Hangfire batch jobs.
-- =============================================

USE master;
GO

ALTER DATABASE HMR_DEV SET SINGLE_USER WITH ROLLBACK IMMEDIATE
GO

ALTER DATABASE HMR_DEV  
SET ALLOW_SNAPSHOT_ISOLATION ON;
GO

ALTER DATABASE HMR_DEV  
SET READ_COMMITTED_SNAPSHOT ON;
GO

ALTER DATABASE HMR_DEV SET MULTI_USER
GO