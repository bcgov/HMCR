-- =============================================
-- Author:		Doug Filteau
-- Create date: 2021-02-22
-- Updates: 
-- - Added new tables to follow existing grants 
--   or to be run independently.
-- 
-- Description:	
--  - Role creation for APPLICATION_PROXY role
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
GRANT select ON dbo.HMR_ACTIVITY_CODE_RULE TO HMR_APPLICATION_PROXY;
GRANT select ON dbo.HMR_SERVICE_AREA_ACTIVITY TO HMR_APPLICATION_PROXY;
GRANT select ON dbo.HMR_SERVICE_AREA_ACTIVITY_HIST TO HMR_APPLICATION_PROXY;
GO

-- Table INSERT/UPDATE grants
PRINT N'Table INSERT/UPDATE grants'
GRANT insert, update ON dbo.HMR_ACTIVITY_CODE_RULE TO HMR_APPLICATION_PROXY;
GRANT insert, update ON dbo.HMR_SERVICE_AREA_ACTIVITY TO HMR_APPLICATION_PROXY;
GRANT insert, update ON dbo.HMR_SERVICE_AREA_ACTIVITY_HIST TO HMR_APPLICATION_PROXY;
GO

-- Table DELETE grants
PRINT N'Table DELETE grants'
GRANT delete ON dbo.HMR_ACTIVITY_CODE_RULE TO HMR_APPLICATION_PROXY;
GRANT delete ON dbo.HMR_SERVICE_AREA_ACTIVITY TO HMR_APPLICATION_PROXY;
GO

-- Sequence DELETE grants
PRINT N'Sequence UPDATE grants'
GRANT update ON dbo.HMR_ACTIVITY_CODE_RULE_ID_SEQ TO HMR_APPLICATION_PROXY;
GRANT update ON dbo.HMR_ACTIVITY_RULE_CODE_ID_SEQ TO HMR_APPLICATION_PROXY;
GRANT update ON dbo.HMR_ACTIVITY_RULE_ID_SEQ TO HMR_APPLICATION_PROXY;
GRANT update ON dbo.HMR_SERVICE_AREA_ACTIVITY_H_ID_SEQ TO HMR_APPLICATION_PROXY;
GRANT update ON dbo.HMR_SERVICE_AREA_ACTIVITY_ID_SEQ TO HMR_APPLICATION_PROXY;
GO
