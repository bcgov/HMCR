
-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-24 
-- Updates: 
--	
-- 
-- Description:	Incremnetal DML in support of sprint 8.
-- - Reduce Spatial tolerance thresholds   
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
/* Update Spatial tolerance thresholds                                    */
/* ---------------------------------------------------------------------- */

UPDATE HMR_CODE_LOOKUP SET CODE_VALUE_NUM = 50 ,CONCURRENCY_CONTROL_NUMBER = CONCURRENCY_CONTROL_NUMBER+1, DB_AUDIT_LAST_UPDATE_USERID = @app_user ,DB_AUDIT_LAST_UPDATE_TIMESTAMP = @utcdate WHERE CODE_SET = 'THRSHLD_SP_VAR_WARN' AND CODE_NAME = 'Geo Variance Warning';

UPDATE HMR_CODE_LOOKUP SET CODE_VALUE_NUM = 100 ,CONCURRENCY_CONTROL_NUMBER = CONCURRENCY_CONTROL_NUMBER+1, DB_AUDIT_LAST_UPDATE_USERID = @app_user	
,DB_AUDIT_LAST_UPDATE_TIMESTAMP = @utcdate WHERE CODE_SET = 'THRSHLD_SP_VAR_ERROR' AND CODE_NAME = 'Geo Variance Error';
GO

