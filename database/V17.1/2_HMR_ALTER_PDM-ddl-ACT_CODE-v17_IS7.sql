/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting - v15             */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-02-24 07:06                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-24
-- Updates: 
--	
-- 
-- Description:	Incremnetal DML in support of sprint 7.
--  - Renamed HMR_ACTIVITY_CODE.POINT_LINE_FEATURE to FEATURE_TYPE
--  - Revised values from 'Either' to 'Point/Line'
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* ---------------------------------------------------------------------- */
/* Drop triggers                                                          */
/* ---------------------------------------------------------------------- */

GO


DROP TRIGGER [dbo].[HMR_ACT_CODE_A_S_IUD_TR]
GO


DROP TRIGGER [dbo].[HMR_ACT_CODE_I_S_I_TR]
GO


DROP TRIGGER [dbo].[HMR_ACT_CODE_I_S_U_TR]
GO


/* ---------------------------------------------------------------------- */
/* Drop foreign key constraints                                           */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_ACTIVITY_CODE] DROP CONSTRAINT [HMR_ACT_CODE_LOC_CODE_FK]
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_ACTIVITY_CODE"                                    */
/* ---------------------------------------------------------------------- */

GO


EXEC sp_rename '[dbo].[HMR_ACTIVITY_CODE].[POINT_LINE_FEATURE]', 'FEATURE_TYPE', 'COLUMN'
GO


EXECUTE sp_updateextendedproperty N'MS_Description', N'Indicator of spatial nature of the activity.  (ie:  point, line or either)   Point - a point location will be reported  Line - activity occurs in relation to a section of road  Either - may be spatially represented in either manner', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'FEATURE_TYPE'
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_ACTIVITY_CODE_HIST"                               */
/* ---------------------------------------------------------------------- */

GO


EXEC sp_rename '[dbo].[HMR_ACTIVITY_CODE_HIST].[POINT_LINE_FEATURE]', 'FEATURE_TYPE', 'COLUMN'
GO


/* ---------------------------------------------------------------------- */
/* Add foreign key constraints                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_ACTIVITY_CODE] ADD CONSTRAINT [HMR_ACT_CODE_LOC_CODE_FK] 
    FOREIGN KEY ([LOCATION_CODE_ID]) REFERENCES [dbo].[HMR_LOCATION_CODE] ([LOCATION_CODE_ID])
GO


/* ---------------------------------------------------------------------- */
/* Repair/add triggers                                                    */
/* ---------------------------------------------------------------------- */

GO


CREATE TRIGGER [dbo].[HMR_ACT_CODE_A_S_IUD_TR] ON HMR_ACTIVITY_CODE FOR INSERT, UPDATE, DELETE AS
SET NOCOUNT ON
BEGIN TRY
DECLARE @curr_date datetime;
SET @curr_date = getutcdate();
  IF NOT EXISTS(SELECT * FROM inserted) AND NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- historical
  IF EXISTS(SELECT * FROM deleted)
    update HMR_ACTIVITY_CODE_HIST set END_DATE_HIST = @curr_date where ACTIVITY_CODE_ID in (select ACTIVITY_CODE_ID from deleted) and END_DATE_HIST is null;

  IF EXISTS(SELECT * FROM inserted)
    insert into HMR_ACTIVITY_CODE_HIST ([ACTIVITY_CODE_ID], [ACTIVITY_NUMBER], [ACTIVITY_NAME], [UNIT_OF_MEASURE], [MAINTENANCE_TYPE], [LOCATION_CODE_ID], [FEATURE_TYPE], [IS_SITE_NUM_REQUIRED], [ACTIVITY_APPLICATION], [END_DATE], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], ACTIVITY_CODE_HIST_ID, END_DATE_HIST, EFFECTIVE_DATE_HIST)
      select [ACTIVITY_CODE_ID], [ACTIVITY_NUMBER], [ACTIVITY_NAME], [UNIT_OF_MEASURE], [MAINTENANCE_TYPE], [LOCATION_CODE_ID], [FEATURE_TYPE], [IS_SITE_NUM_REQUIRED], [ACTIVITY_APPLICATION], [END_DATE], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_ACTIVITY_CODE_H_ID_SEQ]) as [ACTIVITY_CODE_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_ACT_CODE_I_S_I_TR] ON HMR_ACTIVITY_CODE INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted)
    RETURN;


  insert into HMR_ACTIVITY_CODE ("ACTIVITY_CODE_ID",
      "ACTIVITY_NUMBER",
      "ACTIVITY_NAME",
      "UNIT_OF_MEASURE",
      "MAINTENANCE_TYPE",
      "LOCATION_CODE_ID",
      "FEATURE_TYPE",
      "IS_SITE_NUM_REQUIRED",
      "ACTIVITY_APPLICATION",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "ACTIVITY_CODE_ID",
      "ACTIVITY_NUMBER",
      "ACTIVITY_NAME",
      "UNIT_OF_MEASURE",
      "MAINTENANCE_TYPE",
      "LOCATION_CODE_ID",
      "FEATURE_TYPE",
      "IS_SITE_NUM_REQUIRED",
      "ACTIVITY_APPLICATION",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_ACT_CODE_I_S_U_TR] ON HMR_ACTIVITY_CODE INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.ACTIVITY_CODE_ID = deleted.ACTIVITY_CODE_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_ACTIVITY_CODE
    set "ACTIVITY_CODE_ID" = inserted."ACTIVITY_CODE_ID",
      "ACTIVITY_NUMBER" = inserted."ACTIVITY_NUMBER",
      "ACTIVITY_NAME" = inserted."ACTIVITY_NAME",
      "UNIT_OF_MEASURE" = inserted."UNIT_OF_MEASURE",
      "MAINTENANCE_TYPE" = inserted."MAINTENANCE_TYPE",
      "LOCATION_CODE_ID" = inserted."LOCATION_CODE_ID",
      "FEATURE_TYPE" = inserted."FEATURE_TYPE",
      "IS_SITE_NUM_REQUIRED" = inserted."IS_SITE_NUM_REQUIRED",
      "ACTIVITY_APPLICATION" = inserted."ACTIVITY_APPLICATION",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_ACTIVITY_CODE
    inner join inserted
    on (HMR_ACTIVITY_CODE.ACTIVITY_CODE_ID = inserted.ACTIVITY_CODE_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO

/* ---------------------------------------------------------------------- */
/* MODIFIED - update FEATURE_TYPE values                                                  */
/* ---------------------------------------------------------------------- */


DECLARE @utcdate DATETIME = (SELECT getutcdate() AS utcdate)
DECLARE @app_guid UNIQUEIDENTIFIER = (SELECT CASE WHEN SUSER_SID() IS NOT NULL THEN SUSER_SID() ELSE (SELECT CONVERT(uniqueidentifier,STUFF(STUFF(STUFF(STUFF('B00D00A0AC0A0D0C00DD00F0D0C00000',9,0,'-'),14,0,'-'),19,0,'-'),24,0,'-'))) END AS  app_guid)
DECLARE @app_user VARCHAR(30) = (SELECT CASE WHEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) IS NOT NULL THEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) ELSE CURRENT_USER END AS app_user)
DECLARE @app_user_dir VARCHAR(12) = (SELECT CASE WHEN LEN(SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)))>1 THEN SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)) ELSE 'WIN AUTH' END AS app_user_dir)

UPDATE [dbo].[HMR_ACTIVITY_CODE] SET [FEATURE_TYPE] = 'Point/Line', CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1, APP_LAST_UPDATE_USERID=@app_user,APP_LAST_UPDATE_TIMESTAMP=@utcdate,APP_LAST_UPDATE_USER_GUID=@app_guid,APP_LAST_UPDATE_USER_DIRECTORY=@app_user_dir WHERE [FEATURE_TYPE] =  'Either';

UPDATE [dbo].[HMR_ACTIVITY_CODE] SET [FEATURE_TYPE] = 'Point/Line', CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1, APP_LAST_UPDATE_USERID=@app_user,APP_LAST_UPDATE_TIMESTAMP=@utcdate,APP_LAST_UPDATE_USER_GUID=@app_guid,APP_LAST_UPDATE_USER_DIRECTORY=@app_user_dir WHERE [LOCATION_CODE_ID] = 1 AND [FEATURE_TYPE] IS NULL;

UPDATE [dbo].[HMR_CODE_LOOKUP] SET [CODE_SET] = 'FEATURE_TYPE', CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1  WHERE [CODE_SET] = 'POINT_LINE_FEATURE';

UPDATE [dbo].[HMR_CODE_LOOKUP] SET [CODE_VALUE_TEXT] = 'Point/Line', CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1  WHERE [CODE_SET] = 'FEATURE_TYPE' AND [CODE_VALUE_TEXT] = 'Either';

UPDATE [dbo].[HMR_CODE_LOOKUP] SET [CODE_NAME] = 'Point/Line', CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1  WHERE [CODE_SET] = 'FEATURE_TYPE' AND [CODE_NAME] = 'Either';

GO

