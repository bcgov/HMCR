-- *****************************************************************************
-- Populate the HMR_SERVICE_AREA_ACTIVITY table by assigning all service areas 
-- to all activity codes by default.
-- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-- Author        Date         Comment
-- ------------  -----------  --------------------------------------------------
-- Doug Filteau  2020-Oct-14  Initial version.
-- *****************************************************************************

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

INSERT INTO [dbo].[HMR_SERVICE_AREA_ACTIVITY]
  (ACTIVITY_CODE_ID, SERVICE_AREA_NUMBER)
  SELECT ACT.ACTIVITY_CODE_ID
       , SVC.SERVICE_AREA_NUMBER
  FROM   [dbo].[HMR_ACTIVITY_CODE] ACT
       , (SELECT SERVICE_AREA_NUMBER 
          FROM [dbo].[HMR_SERVICE_AREA]) SVC;
GO
