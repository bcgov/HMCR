/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:                                                          */
/* Author:                                                                */
/* Script type:           Alter database script                           */
/* Created on:            2020-02-13 10:59                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-13
-- Updates: 
--	
-- 
-- Description:	Incremnetal DML in support of sprint 5.
--  - Changes boolean fields to BIT data type for HMR_SUBMISSION_ROW.IS_RESUBMITTED, HMR_SYSTEM_VALIDATION.IS_REQUIRED
--  
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* ---------------------------------------------------------------------- */
/* Drop foreign key constraints                                           */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] DROP CONSTRAINT [HMR_SUBM_RW_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] DROP CONSTRAINT [HMR_SUBM_RW_HMR_SUBM_OBJ_FK]
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_SUBMISSION_ROW"                                   */
/* ---------------------------------------------------------------------- */

GO

-- MODIFIED: data conversion  
	DECLARE @utcdate DATETIME = (SELECT getutcdate() AS utcdate)  
	DECLARE @app_guid UNIQUEIDENTIFIER = (SELECT CASE WHEN SUSER_SID() IS NOT NULL THEN SUSER_SID() ELSE (SELECT CONVERT(uniqueidentifier,STUFF(STUFF(STUFF(STUFF('B00D00A0AC0A0D0C00DD00F0D0C00000',9,0,'-'),14,0,'-'),19,0,'-'),24,0,'-'))) END AS  app_guid)  
	DECLARE @app_user VARCHAR(30) = (SELECT CASE WHEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) IS NOT NULL THEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) ELSE CURRENT_USER END AS app_user)  
	DECLARE @app_user_dir VARCHAR(12) = (SELECT CASE WHEN LEN(SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)))>1 THEN SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)) ELSE 'WIN AUTH' END AS app_user_dir)    
	
	UPDATE [dbo].[HMR_SUBMISSION_ROW]   
	SET [IS_RESUBMITTED] = CASE IS_RESUBMITTED    
	WHEN 'Y' THEN 1    
	WHEN 'N' THEN 0    
	ELSE 0    
	END  
	,[CONCURRENCY_CONTROL_NUMBER] = [CONCURRENCY_CONTROL_NUMBER]+1
	GO

ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ALTER COLUMN [IS_RESUBMITTED] BIT
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_SUBMISSION_ROW_HIST"                              */
/* ---------------------------------------------------------------------- */

GO

-- MODIFIED: data conversion  
	DECLARE @utcdate DATETIME = (SELECT getutcdate() AS utcdate)  
	DECLARE @app_guid UNIQUEIDENTIFIER = (SELECT CASE WHEN SUSER_SID() IS NOT NULL THEN SUSER_SID() ELSE (SELECT CONVERT(uniqueidentifier,STUFF(STUFF(STUFF(STUFF('B00D00A0AC0A0D0C00DD00F0D0C00000',9,0,'-'),14,0,'-'),19,0,'-'),24,0,'-'))) END AS  app_guid)  
	DECLARE @app_user VARCHAR(30) = (SELECT CASE WHEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) IS NOT NULL THEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) ELSE CURRENT_USER END AS app_user)  
	DECLARE @app_user_dir VARCHAR(12) = (SELECT CASE WHEN LEN(SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)))>1 THEN SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)) ELSE 'WIN AUTH' END AS app_user_dir)    
	
	UPDATE [dbo].[HMR_SUBMISSION_ROW_HIST]   
	SET [IS_RESUBMITTED] = CASE IS_RESUBMITTED    
	WHEN 'Y' THEN 1    
	WHEN 'N' THEN 0    
	ELSE 0    
	END  
	,[CONCURRENCY_CONTROL_NUMBER] = [CONCURRENCY_CONTROL_NUMBER]+1
	GO

ALTER TABLE [dbo].[HMR_SUBMISSION_ROW_HIST] ALTER COLUMN [IS_RESUBMITTED] BIT
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_SYSTEM_VALIDATION"                                */
/* ---------------------------------------------------------------------- */

GO

-- MODIFIED: data conversion  
	DECLARE @utcdate DATETIME = (SELECT getutcdate() AS utcdate)  
	DECLARE @app_guid UNIQUEIDENTIFIER = (SELECT CASE WHEN SUSER_SID() IS NOT NULL THEN SUSER_SID() ELSE (SELECT CONVERT(uniqueidentifier,STUFF(STUFF(STUFF(STUFF('B00D00A0AC0A0D0C00DD00F0D0C00000',9,0,'-'),14,0,'-'),19,0,'-'),24,0,'-'))) END AS  app_guid)  
	DECLARE @app_user VARCHAR(30) = (SELECT CASE WHEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) IS NOT NULL THEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) ELSE CURRENT_USER END AS app_user)  
	DECLARE @app_user_dir VARCHAR(12) = (SELECT CASE WHEN LEN(SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)))>1 THEN SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)) ELSE 'WIN AUTH' END AS app_user_dir)    
	
	UPDATE [dbo].[HMR_SYSTEM_VALIDATION]   
	SET [IS_REQUIRED] = CASE IS_REQUIRED   
	WHEN 'Y' THEN 1    
	WHEN 'N' THEN 0    
	ELSE 0    
	END  
	,[CONCURRENCY_CONTROL_NUMBER] = [CONCURRENCY_CONTROL_NUMBER]+1
	GO

ALTER TABLE [dbo].[HMR_SYSTEM_VALIDATION] ALTER COLUMN [IS_REQUIRED] BIT
GO


/* ---------------------------------------------------------------------- */
/* Add foreign key constraints                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ADD CONSTRAINT [HMR_SUBM_RW_SUBM_STAT_FK] 
    FOREIGN KEY ([ROW_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ADD CONSTRAINT [HMR_SUBM_RW_HMR_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO

