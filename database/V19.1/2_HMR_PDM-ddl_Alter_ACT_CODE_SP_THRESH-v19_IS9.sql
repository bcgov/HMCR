/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting                   */
/* Author:                Ayodeji Kuponiyi                                */
/* Script type:           Alter database script                           */
/* Created on:            2020-04-02 16:29                                */
/* ---------------------------------------------------------------------- */


/* Updates
1. Added a new column SP_THRESHOLD_LEVEL to ACTIVITY_CODE and ACTIVITY_CODE_HIST tables
*/


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
/* Drop and recreate table "dbo.HMR_ACTIVITY_CODE"                        */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_ACTIVITY_CODE] DROP CONSTRAINT [HMR_ACT_CODE_PK]
GO


ALTER TABLE [dbo].[HMR_ACTIVITY_CODE] DROP CONSTRAINT [HMR_ACTIVITY_CODE_UC]
GO


CREATE TABLE [dbo].[HMR_ACTIVITY_CODE_TMP] (
    [ACTIVITY_CODE_ID] NUMERIC(9) DEFAULT NEXT VALUE FOR [HMR_ACT_CODE_ID_SEQ] NOT NULL,
    [ACTIVITY_NUMBER] VARCHAR(6) NOT NULL,
    [ACTIVITY_NAME] VARCHAR(150) NOT NULL,
    [UNIT_OF_MEASURE] VARCHAR(12) NOT NULL,
    [MAINTENANCE_TYPE] VARCHAR(12) NOT NULL,
    [LOCATION_CODE_ID] NUMERIC(9) NOT NULL,
    [FEATURE_TYPE] VARCHAR(12),
    [SP_THRESHOLD_LEVEL] VARCHAR(30),
    [IS_SITE_NUM_REQUIRED] BIT,
    [ACTIVITY_APPLICATION] VARCHAR(30),
    [END_DATE] DATETIME,
    [CONCURRENCY_CONTROL_NUMBER] BIGINT DEFAULT 1 NOT NULL,
    [APP_CREATE_USERID] VARCHAR(30) NOT NULL,
    [APP_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_CREATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_CREATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [APP_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [APP_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_LAST_UPDATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_LAST_UPDATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) DEFAULT user_name() NOT NULL,
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME DEFAULT getutcdate() NOT NULL,
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) DEFAULT user_name() NOT NULL,
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME DEFAULT getutcdate() NOT NULL)
GO


INSERT INTO [dbo].[HMR_ACTIVITY_CODE_TMP]
    ([ACTIVITY_CODE_ID],[ACTIVITY_NUMBER],[ACTIVITY_NAME],[UNIT_OF_MEASURE],[MAINTENANCE_TYPE],[LOCATION_CODE_ID],[FEATURE_TYPE],[SP_THRESHOLD_LEVEL],[IS_SITE_NUM_REQUIRED],[ACTIVITY_APPLICATION],[END_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [ACTIVITY_CODE_ID],[ACTIVITY_NUMBER],[ACTIVITY_NAME],[UNIT_OF_MEASURE],[MAINTENANCE_TYPE],[LOCATION_CODE_ID],[FEATURE_TYPE],null,[IS_SITE_NUM_REQUIRED],[ACTIVITY_APPLICATION],[END_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_ACTIVITY_CODE]
GO


DROP INDEX [dbo].[HMR_ACTIVITY_CODE].[HMR_ACT_CODE_FK_I]
GO


DROP TABLE [dbo].[HMR_ACTIVITY_CODE]
GO


EXEC sp_rename '[dbo].[HMR_ACTIVITY_CODE_TMP]', 'HMR_ACTIVITY_CODE', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_ACTIVITY_CODE] ADD CONSTRAINT [HMR_ACT_CODE_PK] 
    PRIMARY KEY CLUSTERED ([ACTIVITY_CODE_ID])
GO


ALTER TABLE [dbo].[HMR_ACTIVITY_CODE] ADD CONSTRAINT [HMR_ACTIVITY_CODE_UC] 
    UNIQUE ([ACTIVITY_NUMBER])
GO


CREATE NONCLUSTERED INDEX [HMR_ACT_CODE_FK_I] ON [dbo].[HMR_ACTIVITY_CODE] ([LOCATION_CODE_ID] ASC)
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Indicator of spatial nature of the activity.  (ie:  point, line or either)   Point - a point location will be reported  Line - activity occurs in relation to a section of road  Either - may be spatially represented in either manner', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'FEATURE_TYPE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Determines the tolerated spatial variance allowed when comparing submitted activity coordinates vs the related Highway Unique road segment. Each level is defined within the CODE_LOOKUP table under the THRSHLD_SP_VAR code', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'SP_THRESHOLD_LEVEL'
GO

/* -------------------------------------------------------------------------------------------- */
/* MODIFIED - Reinstating field descriptions for HMR_ACTIVITY_CODE     */
/* -------------------------------------------------------------------------------------------- */


EXECUTE sp_addextendedproperty N'MS_Description', N'A tracking number for maintenance activities undertaken by the Contractor. This number is required for the specific reporting of each activity. The numbers are provided by the Province.  Reporting criteria varies based on location requirements, record frequency and reporting frequency.  Local Area Specification activities vary by Service Area, and therefore many of these activities do not apply to each Service Area.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', NULL, NULL
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for a record.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'ACTIVITY_CODE_ID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Code which uniquely identifies the activity performed.  The number reflects a a classificaiton hierarchy comprised of three levels: ABBCCC  A - the first digit represents Specification Category (eg:2 - Drainage ) BB - the second two digits represent Activity Category (eg: 02 - Drainage Appliance Maintenance) CCC - the last three digits represent Activity Type and Detail (eg: 310 - Boring, Augering.  300 series reflects Quantified value, which would be linear meters in this case.)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'ACTIVITY_NUMBER'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'N', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'ACTIVITY_NAME'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'The code which represents the unit of measure for the specified activity. ', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'UNIT_OF_MEASURE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N' Classification of maintenance activities which specifies detail of submission or reporting requirements (ie: Routine, Quantified, Additional).   Routine - reoccuring maintenace activities that require less detailed reporting  Quantified - maintenance activities that require more detailed reporting  Additional - activities that exceed agreement threasholds', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'MAINTENANCE_TYPE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for a record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'LOCATION_CODE_ID'
GO


EXECUTE sp_updateextendedproperty N'MS_Description', N'Indicator of spatial nature of the activity.  (ie:  point, line or either)   Point - a point location will be reported  Line - activity occurs in relation to a section of road  Either - may be spatially represented in either manner', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'FEATURE_TYPE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Indicates if a site number must be submitted for the activity', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'IS_SITE_NUM_REQUIRED'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Indicates if activity is conducted in all service areas or is specified for some service areas. ', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'ACTIVITY_APPLICATION'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'The latest date submissions will be accepted.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'END_DATE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'CONCURRENCY_CONTROL_NUMBER'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'APP_CREATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of record creation', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'APP_CREATE_TIMESTAMP'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'APP_CREATE_USER_GUID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'APP_CREATE_USER_DIRECTORY'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'APP_LAST_UPDATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of last record update', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'APP_LAST_UPDATE_TIMESTAMP'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'APP_LAST_UPDATE_USER_GUID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'APP_LAST_UPDATE_USER_DIRECTORY'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'DB_AUDIT_CREATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record created in the database', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'DB_AUDIT_CREATE_TIMESTAMP'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record was last updated in the database.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ACTIVITY_CODE', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_TIMESTAMP'
GO






/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_ACTIVITY_CODE_HIST"                   */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_ACTIVITY_CODE_HIST] DROP CONSTRAINT [HMR_ACT_C_H_PK]
GO


ALTER TABLE [dbo].[HMR_ACTIVITY_CODE_HIST] DROP CONSTRAINT [HMR_ACT_C_H_UK]
GO


CREATE TABLE [dbo].[HMR_ACTIVITY_CODE_HIST_TMP] (
    [ACTIVITY_CODE_HIST_ID] BIGINT DEFAULT NEXT VALUE FOR [HMR_ACTIVITY_CODE_H_ID_SEQ] NOT NULL,
    [EFFECTIVE_DATE_HIST] DATETIME DEFAULT getutcdate() NOT NULL,
    [END_DATE_HIST] DATETIME,
    [ACTIVITY_CODE_ID] NUMERIC(18) NOT NULL,
    [ACTIVITY_NUMBER] VARCHAR(6) NOT NULL,
    [ACTIVITY_NAME] VARCHAR(150) NOT NULL,
    [UNIT_OF_MEASURE] VARCHAR(12) NOT NULL,
    [MAINTENANCE_TYPE] VARCHAR(12) NOT NULL,
    [LOCATION_CODE_ID] NUMERIC(18) NOT NULL,
    [FEATURE_TYPE] VARCHAR(12),
    [SP_THRESHOLD_LEVEL] VARCHAR(30),
    [IS_SITE_NUM_REQUIRED] BIT,
    [ACTIVITY_APPLICATION] VARCHAR(30),
    [END_DATE] DATETIME,
    [CONCURRENCY_CONTROL_NUMBER] BIGINT NOT NULL,
    [APP_CREATE_USERID] VARCHAR(30) NOT NULL,
    [APP_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_CREATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_CREATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [APP_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [APP_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_LAST_UPDATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_LAST_UPDATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) NOT NULL,
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL)
GO


INSERT INTO [dbo].[HMR_ACTIVITY_CODE_HIST_TMP]
    ([ACTIVITY_CODE_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[ACTIVITY_CODE_ID],[ACTIVITY_NUMBER],[ACTIVITY_NAME],[UNIT_OF_MEASURE],[MAINTENANCE_TYPE],[LOCATION_CODE_ID],[FEATURE_TYPE],[SP_THRESHOLD_LEVEL],[IS_SITE_NUM_REQUIRED],[ACTIVITY_APPLICATION],[END_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [ACTIVITY_CODE_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[ACTIVITY_CODE_ID],[ACTIVITY_NUMBER],[ACTIVITY_NAME],[UNIT_OF_MEASURE],[MAINTENANCE_TYPE],[LOCATION_CODE_ID],[FEATURE_TYPE],null,[IS_SITE_NUM_REQUIRED],[ACTIVITY_APPLICATION],[END_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_ACTIVITY_CODE_HIST]
GO


DROP TABLE [dbo].[HMR_ACTIVITY_CODE_HIST]
GO


EXEC sp_rename '[dbo].[HMR_ACTIVITY_CODE_HIST_TMP]', 'HMR_ACTIVITY_CODE_HIST', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_ACTIVITY_CODE_HIST] ADD CONSTRAINT [HMR_ACT_C_H_PK] 
    PRIMARY KEY CLUSTERED ([ACTIVITY_CODE_HIST_ID])
GO


ALTER TABLE [dbo].[HMR_ACTIVITY_CODE_HIST] ADD CONSTRAINT [HMR_ACT_C_H_UK] 
    UNIQUE ([ACTIVITY_CODE_HIST_ID], [END_DATE_HIST])
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
    insert into HMR_ACTIVITY_CODE_HIST ([ACTIVITY_CODE_ID], [ACTIVITY_NUMBER], [ACTIVITY_NAME], [UNIT_OF_MEASURE], [MAINTENANCE_TYPE], [LOCATION_CODE_ID], [FEATURE_TYPE], [SP_THRESHOLD_LEVEL], [IS_SITE_NUM_REQUIRED], [ACTIVITY_APPLICATION], [END_DATE], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], ACTIVITY_CODE_HIST_ID, END_DATE_HIST, EFFECTIVE_DATE_HIST)
      select [ACTIVITY_CODE_ID], [ACTIVITY_NUMBER], [ACTIVITY_NAME], [UNIT_OF_MEASURE], [MAINTENANCE_TYPE], [LOCATION_CODE_ID], [FEATURE_TYPE], [SP_THRESHOLD_LEVEL], [IS_SITE_NUM_REQUIRED], [ACTIVITY_APPLICATION], [END_DATE], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_ACTIVITY_CODE_H_ID_SEQ]) as [ACTIVITY_CODE_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

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
      "SP_THRESHOLD_LEVEL",
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
      "SP_THRESHOLD_LEVEL",
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
      "SP_THRESHOLD_LEVEL" = inserted."SP_THRESHOLD_LEVEL",
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

