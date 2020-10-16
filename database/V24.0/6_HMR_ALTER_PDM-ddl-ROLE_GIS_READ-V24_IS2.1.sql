-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-03-02
-- Updates: 
--	
-- 
-- Description:	
--  - Role creation for GIS_READ role
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

-- Create ROLE

if not exists (select 1 from sys.database_principals where name='HMR_GIS_READ' and Type = 'R')
	begin
		CREATE ROLE HMR_GIS_READ AUTHORIZATION db_securityadmin;
	end

-- GRANT select on views

GRANT select ON dbo.HMR_ROCKFALL_REPORT_VW TO HMR_GIS_READ;
GRANT select ON dbo.HMR_WILDLIFE_REPORT_VW TO HMR_GIS_READ;
GRANT select ON dbo.HMR_WORK_REPORT_VW TO HMR_GIS_READ;

GO

