-- =============================================
-- Author:		Doug Filteau
-- Create date: 2021-02-22
-- Updates: 
-- - Added new tables to follow existing grants 
--   or to be run independently.
-- 
-- Description:	
--  - Read-only role creation for APPLICATION_PROXY role
-- =============================================

-- Uncomment appropriate instance
USE HMR_DEV; 
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

-- Create HMR_APPLICATION_PROXY role
PRINT N'Create HMR_APPLICATION_PROXY role'

if not exists (select 1 from sys.database_principals where name='HMR_APPLICATION_PROXY' and Type = 'R')
	begin
		CREATE ROLE HMR_APPLICATION_PROXY AUTHORIZATION db_securityadmin;
	end
GO

-- Table SELECT grants
PRINT N'Table SELECT grants'
GRANT select ON dbo.HMR_ACTIVITY_CODE_RULE TO HMR_READ_ONLY;
GRANT select ON dbo.HMR_SERVICE_AREA_ACTIVITY TO HMR_READ_ONLY;
GO
