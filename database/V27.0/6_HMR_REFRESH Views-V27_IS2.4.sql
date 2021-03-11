-- =============================================

-- Description:	
--  - refresh existing views
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

EXEC SP_REFRESHVIEW HMR_WORK_REPORT_VW;
EXEC SP_REFRESHVIEW HMR_WILDLIFE_REPORT_VW;
EXEC SP_REFRESHVIEW HMR_ROCKFALL_REPORT_VW; 

GO

