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
  (ACTIVITY_CODE_ID, SERVICE_AREA_NUMBER, APP_CREATE_USERID, APP_CREATE_TIMESTAMP, APP_CREATE_USER_GUID, APP_CREATE_USER_DIRECTORY, APP_LAST_UPDATE_USERID, APP_LAST_UPDATE_TIMESTAMP, APP_LAST_UPDATE_USER_GUID, APP_LAST_UPDATE_USER_DIRECTORY)
  SELECT ACT.ACTIVITY_CODE_ID
       , SVC.SERVICE_AREA_NUMBER
       , 'sa'
       , getutcdate()
       , '00000001-0000-0000-0000-000000000000'
       , 'WIN AUTH'
       , 'sa'
       , getutcdate()
       , '00000001-0000-0000-0000-000000000000'
       , 'WIN AUTH'
  FROM   [dbo].[HMR_ACTIVITY_CODE] ACT
       , (SELECT SERVICE_AREA_NUMBER 
          FROM [dbo].[HMR_SERVICE_AREA]) SVC;
GO
