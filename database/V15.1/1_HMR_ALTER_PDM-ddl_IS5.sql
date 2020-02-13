
-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-01
-- Updates: 
-- 
-- Description:	v0.15 Lookup value modifications and change to BIT data type for HMR_ROLE.IS_INTERNAL in support of IS5.
-- =============================================


USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

-- Revert to non-superscript units of measure
UPDATE [dbo].[HMR_CODE_LOOKUP]
   SET CODE_VALUE_TEXT = REPLACE([CODE_VALUE_TEXT], NCHAR(0xb3), 3)
	,CODE_NAME = REPLACE([CODE_NAME], NCHAR(0xb3), 3)
	,[CONCURRENCY_CONTROL_NUMBER] = [CONCURRENCY_CONTROL_NUMBER]+1
WHERE [CODE_SET] ='VOLUME_RANGE1'
GO

/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_ROLE"                                             */
/* ---------------------------------------------------------------------- */

GO

DECLARE @ConstraintName nvarchar(200);
DECLARE @dcTbl nvarchar(200) = 'HMR_ROLE';
DECLARE @dcCln nvarchar(200) = 'IS_INTERNAL';

SELECT @ConstraintName = con.[name] 
from sys.check_constraints con
    left outer join sys.objects t
        on con.parent_object_id = t.object_id
    left outer join sys.all_columns col
        on con.parent_column_id = col.column_id
        and con.parent_object_id = col.object_id
WHERE schema_name(t.schema_id) = 'dbo' 
	AND t.[name] = @dcTbl
	AND col.[name] = @dcCln;

PRINT ('Deleting ' + @ConstraintName + ' ...');

Exec('ALTER TABLE [' + @dcTbl + N'] DROP CONSTRAINT [' + @ConstraintName + ']');

PRINT (@ConstraintName + ' Deleted.');

-- MODIFIED: data conversion  
	DECLARE @utcdate DATETIME = (SELECT getutcdate() AS utcdate)  
	DECLARE @app_guid UNIQUEIDENTIFIER = (SELECT CASE WHEN SUSER_SID() IS NOT NULL THEN SUSER_SID() ELSE (SELECT CONVERT(uniqueidentifier,STUFF(STUFF(STUFF(STUFF('B00D00A0AC0A0D0C00DD00F0D0C00000',9,0,'-'),14,0,'-'),19,0,'-'),24,0,'-'))) END AS  app_guid)  
	DECLARE @app_user VARCHAR(30) = (SELECT CASE WHEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) IS NOT NULL THEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) ELSE CURRENT_USER END AS app_user)  
	DECLARE @app_user_dir VARCHAR(12) = (SELECT CASE WHEN LEN(SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)))>1 THEN SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)) ELSE 'WIN AUTH' END AS app_user_dir)    
	
	UPDATE [dbo].[HMR_ROLE]   
	SET [IS_INTERNAL] = CASE IS_INTERNAL    
	WHEN 'Y' THEN 1    
	WHEN 'N' THEN 0    
	ELSE 0    
	END  
	,[CONCURRENCY_CONTROL_NUMBER] = [CONCURRENCY_CONTROL_NUMBER]+1
	,[APP_LAST_UPDATE_USERID] = @app_user  
	,[APP_LAST_UPDATE_TIMESTAMP] = @utcdate  
	,[APP_LAST_UPDATE_USER_GUID] = @app_guid  
	,[APP_LAST_UPDATE_USER_DIRECTORY] = @app_user_dir 
	GO

ALTER TABLE [dbo].[HMR_ROLE] ALTER COLUMN [IS_INTERNAL] BIT NOT NULL;
GO


ALTER TABLE [dbo].[HMR_ROLE] ADD
    DEFAULT (0) FOR [IS_INTERNAL];
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_ROLE_HIST"                                        */
/* ---------------------------------------------------------------------- */


-- MODIFIED: data conversion  
	DECLARE @utcdate DATETIME = (SELECT getutcdate() AS utcdate)  
	DECLARE @app_guid UNIQUEIDENTIFIER = (SELECT CASE WHEN SUSER_SID() IS NOT NULL THEN SUSER_SID() ELSE (SELECT CONVERT(uniqueidentifier,STUFF(STUFF(STUFF(STUFF('B00D00A0AC0A0D0C00DD00F0D0C00000',9,0,'-'),14,0,'-'),19,0,'-'),24,0,'-'))) END AS  app_guid)  
	DECLARE @app_user VARCHAR(30) = (SELECT CASE WHEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) IS NOT NULL THEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) ELSE CURRENT_USER END AS app_user)  
	DECLARE @app_user_dir VARCHAR(12) = (SELECT CASE WHEN LEN(SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)))>1 THEN SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)) ELSE 'WIN AUTH' END AS app_user_dir)    
	
	UPDATE [dbo].[HMR_ROLE_HIST]   
	SET [IS_INTERNAL] = CASE IS_INTERNAL    
	WHEN 'Y' THEN 1   
	WHEN 'N' THEN 0    
	ELSE 0    
	END  
	,[CONCURRENCY_CONTROL_NUMBER] = [CONCURRENCY_CONTROL_NUMBER]+1
	,[APP_LAST_UPDATE_USERID] = @app_user  
	,[APP_LAST_UPDATE_TIMESTAMP] = @utcdate  
	,[APP_LAST_UPDATE_USER_GUID] = @app_guid  
	,[APP_LAST_UPDATE_USER_DIRECTORY] = @app_user_dir 
	GO

ALTER TABLE [dbo].[HMR_ROLE_HIST] ALTER COLUMN [IS_INTERNAL] BIT
GO







