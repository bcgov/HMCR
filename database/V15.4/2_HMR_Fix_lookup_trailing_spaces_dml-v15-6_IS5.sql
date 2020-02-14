
-- =============================================
-- Author:		Neal Li
-- Create date: 2020-02-13
-- Updates: 
-- 
-- Description:	v0.15.6 Fix Code Lookup and Activity Code table column trailing spaces, and inconsistent Activity Code column values
-- =============================================


USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* Load user context variables.  Redeclared for subsequent executions */

DECLARE @utcdate DATETIME = (SELECT getutcdate() AS utcdate)
DECLARE @app_guid UNIQUEIDENTIFIER = (SELECT CASE WHEN SUSER_SID() IS NOT NULL THEN SUSER_SID() ELSE (SELECT CONVERT(uniqueidentifier,STUFF(STUFF(STUFF(STUFF('B00D00A0AC0A0D0C00DD00F0D0C00000',9,0,'-'),14,0,'-'),19,0,'-'),24,0,'-'))) END AS  app_guid)
DECLARE @app_user VARCHAR(30) = (SELECT CASE WHEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) IS NOT NULL THEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) ELSE CURRENT_USER END AS app_user)
DECLARE @app_user_dir VARCHAR(12) = (SELECT CASE WHEN LEN(SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)))>1 THEN SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)) ELSE 'WIN AUTH' END AS app_user_dir)


/* ---------------------------------------------------------------------- */
/* Fix trailing spaces and inconsistent upper/lower case          	      */
/* ---------------------------------------------------------------------- */

UPDATE [dbo].[HMR_ACTIVITY_CODE]
   SET UNIT_OF_MEASURE = LOWER(RTRIM(UNIT_OF_MEASURE)), CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1, APP_LAST_UPDATE_USERID=@app_user,APP_LAST_UPDATE_TIMESTAMP=@utcdate,APP_LAST_UPDATE_USER_GUID=@app_guid,APP_LAST_UPDATE_USER_DIRECTORY=@app_user_dir

UPDATE [dbo].[HMR_CODE_LOOKUP]
   SET CODE_NAME = RTRIM(CODE_NAME), CODE_VALUE_TEXT = RTRIM(CODE_VALUE_TEXT), CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1
   WHERE CODE_SET = 'UOM'

/* ---------------------------------------------------------------------- */
/* Fix Activity Code table Maintenance Type values                        */
/* ---------------------------------------------------------------------- */

UPDATE [dbo].[HMR_ACTIVITY_CODE]
   SET MAINTENANCE_TYPE = (SELECT CODE_VALUE_TEXT FROM [dbo].[HMR_CODE_LOOKUP] WHERE CODE_SET = 'WRK_RPT_MAINT_TYPE' AND CODE_NAME = MAINTENANCE_TYPE), CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1, APP_LAST_UPDATE_USERID=@app_user,APP_LAST_UPDATE_TIMESTAMP=@utcdate,APP_LAST_UPDATE_USER_GUID=@app_guid,APP_LAST_UPDATE_USER_DIRECTORY=@app_user_dir

GO