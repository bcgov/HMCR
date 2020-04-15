/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          V19_APP_HMR_06042020.dez                        */
/* Project name:          Highway Maintenance Reporting                   */
/* Author:                Ayodeji Kuponiyi                                */
/* Script type:           Alter database script                           */
/* Created on:            2020-04-06 15:24                                */
/* ---------------------------------------------------------------------- */

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* Updates 
  1. added a new column: [HIGHWAY_UNIQUE_PREFIX]
  2. Inserted values
*/


/* ---------------------------------------------------------------------- */
/* Drop triggers                                                          */
/* ---------------------------------------------------------------------- */

GO


DROP TRIGGER [dbo].[HMR_SRV_ARA_A_S_IUD_TR]
GO


DROP TRIGGER [dbo].[HMR_SRV_ARA_I_S_I_TR]
GO


DROP TRIGGER [dbo].[HMR_SRV_ARA_I_S_U_TR]
GO


/* ---------------------------------------------------------------------- */
/* Drop foreign key constraints                                           */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA] DROP CONSTRAINT [HMR_SRV_AREA_DISTRICT_FK]
GO


ALTER TABLE [dbo].[HMR_CONTRACT_TERM] DROP CONSTRAINT [HMR_CNRT_TRM_SRV_ARA_FK]
GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA_USER] DROP CONSTRAINT [HMR_SRV_AREA_USR_SRV_AREA_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] DROP CONSTRAINT [HRM_SUBM_OBJ_SRV_AREA_FK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RPT_HMR_SRV_ARA_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SRV_ARA_FK]
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RCKFL_RPT_HMR_SRV_ARA_FK]
GO

/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_SERVICE_AREA"                         */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA] DROP CONSTRAINT [HMR_SERVICE_AREA_PK]
GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA] DROP CONSTRAINT [HMR_SRV_ARA_UK]
GO


CREATE TABLE [dbo].[HMR_SERVICE_AREA_TMP] (
    [SERVICE_AREA_ID] NUMERIC(9) DEFAULT NEXT VALUE FOR [HMR_SRV_ARA_ID_SEQ] NOT NULL,
    [SERVICE_AREA_NUMBER] NUMERIC(9) NOT NULL,
    [SERVICE_AREA_NAME] VARCHAR(60) NOT NULL,
    [DISTRICT_NUMBER] NUMERIC(2) NOT NULL,
    [HIGHWAY_UNIQUE_PREFIX] VARCHAR(30),
    [CONCURRENCY_CONTROL_NUMBER] BIGINT DEFAULT 1 NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) DEFAULT user_name() NOT NULL,
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME DEFAULT getutcdate() NOT NULL,
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) DEFAULT user_name() NOT NULL,
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME DEFAULT getutcdate() NOT NULL)
GO


INSERT INTO [dbo].[HMR_SERVICE_AREA_TMP]
    ([SERVICE_AREA_ID],[SERVICE_AREA_NUMBER],[SERVICE_AREA_NAME],[DISTRICT_NUMBER],[HIGHWAY_UNIQUE_PREFIX],[CONCURRENCY_CONTROL_NUMBER],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [SERVICE_AREA_ID],[SERVICE_AREA_NUMBER],[SERVICE_AREA_NAME],[DISTRICT_NUMBER],(SELECT '06,43,44' WHERE SERVICE_AREA_NUMBER = 6), [CONCURRENCY_CONTROL_NUMBER],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_SERVICE_AREA]

GO

DROP TABLE [dbo].[HMR_SERVICE_AREA]
GO


EXEC sp_rename '[dbo].[HMR_SERVICE_AREA_TMP]', 'HMR_SERVICE_AREA', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA] ADD CONSTRAINT [HMR_SERVICE_AREA_PK] 
    PRIMARY KEY CLUSTERED ([SERVICE_AREA_NUMBER])
GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA] ADD CONSTRAINT [HMR_SRV_ARA_UK] 
    UNIQUE ([SERVICE_AREA_NUMBER], [SERVICE_AREA_NAME])
GO


/* ---------------------------------------------------------------------- */
/* MODIFIED - Reinstate field description                                 */
/* ---------------------------------------------------------------------- */

EXECUTE sp_addextendedproperty N'MS_Description', N'Service Area lookup values', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SERVICE_AREA', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier for table records', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SERVICE_AREA', 'COLUMN', N'SERVICE_AREA_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Assigned number of the Service Area', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SERVICE_AREA', 'COLUMN', N'SERVICE_AREA_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Name of the service area', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SERVICE_AREA', 'COLUMN', N'SERVICE_AREA_NAME'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for DISTRICT.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SERVICE_AREA', 'COLUMN', N'DISTRICT_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SERVICE_AREA', 'COLUMN', N'CONCURRENCY_CONTROL_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SERVICE_AREA', 'COLUMN', N'DB_AUDIT_CREATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record created in the database', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SERVICE_AREA', 'COLUMN', N'DB_AUDIT_CREATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SERVICE_AREA', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record was last updated in the database.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SERVICE_AREA', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Determines the tolerated spatial variance allowed between submitted activity coordinates and the related Highway Unique road segment', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SERVICE_AREA', 'COLUMN', N'HIGHWAY_UNIQUE_PREFIX'
GO 


/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_SERVICE_AREA_HIST"                    */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA_HIST] DROP CONSTRAINT [HMR_SRV_A_H_PK]
GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA_HIST] DROP CONSTRAINT [HMR_SRV_A_H_UK]
GO


CREATE TABLE [dbo].[HMR_SERVICE_AREA_HIST_TMP] (
    [SERVICE_AREA_HIST_ID] BIGINT DEFAULT NEXT VALUE FOR [HMR_SERVICE_AREA_H_ID_SEQ] NOT NULL,
    [EFFECTIVE_DATE_HIST] DATETIME DEFAULT getutcdate() NOT NULL,
    [END_DATE_HIST] DATETIME,
    [SERVICE_AREA_ID] NUMERIC(18) NOT NULL,
    [SERVICE_AREA_NUMBER] NUMERIC(18) NOT NULL,
    [SERVICE_AREA_NAME] VARCHAR(60) NOT NULL,
    [DISTRICT_NUMBER] NUMERIC(18) NOT NULL,
    [HIGHWAY_UNIQUE_PREFIX] VARCHAR(30),
    [CONCURRENCY_CONTROL_NUMBER] BIGINT NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) NOT NULL,
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL)
GO


INSERT INTO [dbo].[HMR_SERVICE_AREA_HIST_TMP]
    ([SERVICE_AREA_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[SERVICE_AREA_ID],[SERVICE_AREA_NUMBER],[SERVICE_AREA_NAME],[DISTRICT_NUMBER],[CONCURRENCY_CONTROL_NUMBER],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [SERVICE_AREA_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[SERVICE_AREA_ID],[SERVICE_AREA_NUMBER],[SERVICE_AREA_NAME],[DISTRICT_NUMBER],[CONCURRENCY_CONTROL_NUMBER],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_SERVICE_AREA_HIST]
GO


DROP TABLE [dbo].[HMR_SERVICE_AREA_HIST]
GO


EXEC sp_rename '[dbo].[HMR_SERVICE_AREA_HIST_TMP]', 'HMR_SERVICE_AREA_HIST', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA_HIST] ADD CONSTRAINT [HMR_SRV_A_H_PK] 
    PRIMARY KEY CLUSTERED ([SERVICE_AREA_HIST_ID])
GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA_HIST] ADD CONSTRAINT [HMR_SRV_A_H_UK] 
    UNIQUE ([SERVICE_AREA_HIST_ID], [END_DATE_HIST])
GO


/* ---------------------------------------------------------------------- */
/* Add foreign key constraints                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA] ADD CONSTRAINT [HMR_SRV_AREA_DISTRICT_FK] 
    FOREIGN KEY ([DISTRICT_NUMBER]) REFERENCES [dbo].[HMR_DISTRICT] ([DISTRICT_NUMBER])
GO


ALTER TABLE [dbo].[HMR_CONTRACT_TERM] ADD CONSTRAINT [HMR_CNRT_TRM_SRV_ARA_FK] 
    FOREIGN KEY ([SERVICE_AREA_NUMBER]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA_USER] ADD CONSTRAINT [HMR_SRV_AREA_USR_SRV_AREA_FK] 
    FOREIGN KEY ([SERVICE_AREA_NUMBER]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] ADD CONSTRAINT [HRM_SUBM_OBJ_SRV_AREA_FK] 
    FOREIGN KEY ([SERVICE_AREA_NUMBER]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RPT_HMR_SRV_ARA_FK] 
    FOREIGN KEY ([SERVICE_AREA]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SRV_ARA_FK] 
    FOREIGN KEY ([SERVICE_AREA]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] ADD CONSTRAINT [HMR_RCKFL_RPT_HMR_SRV_ARA_FK] 
    FOREIGN KEY ([SERVICE_AREA]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


/* ---------------------------------------------------------------------- */
/* Repair/add triggers                                                    */
/* ---------------------------------------------------------------------- */

GO


CREATE TRIGGER [dbo].[HMR_SRV_ARA_A_S_IUD_TR] ON HMR_SERVICE_AREA FOR INSERT, UPDATE, DELETE AS
SET NOCOUNT ON
BEGIN TRY
DECLARE @curr_date datetime;
SET @curr_date = getutcdate();
  IF NOT EXISTS(SELECT * FROM inserted) AND NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- historical
  IF EXISTS(SELECT * FROM deleted)
    update HMR_SERVICE_AREA_HIST set END_DATE_HIST = @curr_date where SERVICE_AREA_NUMBER in (select SERVICE_AREA_NUMBER from deleted) and END_DATE_HIST is null;

  IF EXISTS(SELECT * FROM inserted)
    insert into HMR_SERVICE_AREA_HIST ([SERVICE_AREA_ID], [SERVICE_AREA_NUMBER], [SERVICE_AREA_NAME], [DISTRICT_NUMBER], [HIGHWAY_UNIQUE_PREFIX], [CONCURRENCY_CONTROL_NUMBER], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], SERVICE_AREA_HIST_ID, END_DATE_HIST, EFFECTIVE_DATE_HIST)
      select [SERVICE_AREA_ID], [SERVICE_AREA_NUMBER], [SERVICE_AREA_NAME], [DISTRICT_NUMBER], [HIGHWAY_UNIQUE_PREFIX], [CONCURRENCY_CONTROL_NUMBER], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_SERVICE_AREA_H_ID_SEQ]) as [SERVICE_AREA_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_SRV_ARA_I_S_I_TR] ON HMR_SERVICE_AREA INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted)
    RETURN;


  insert into HMR_SERVICE_AREA ("SERVICE_AREA_ID",
      "SERVICE_AREA_NUMBER",
      "SERVICE_AREA_NAME",
      "DISTRICT_NUMBER", 
      "HIGHWAY_UNIQUE_PREFIX",
      "CONCURRENCY_CONTROL_NUMBER")
    select "SERVICE_AREA_ID",
      "SERVICE_AREA_NUMBER",
      "SERVICE_AREA_NAME",
      "DISTRICT_NUMBER", 
      "HIGHWAY_UNIQUE_PREFIX",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_SRV_ARA_I_S_U_TR] ON HMR_SERVICE_AREA INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.SERVICE_AREA_NUMBER = deleted.SERVICE_AREA_NUMBER)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SERVICE_AREA
    set "SERVICE_AREA_ID" = inserted."SERVICE_AREA_ID",
      "SERVICE_AREA_NUMBER" = inserted."SERVICE_AREA_NUMBER",
      "SERVICE_AREA_NAME" = inserted."SERVICE_AREA_NAME",
      "DISTRICT_NUMBER" = inserted."DISTRICT_NUMBER",    
      "HIGHWAY_UNIQUE_PREFIX"  = inserted."HIGHWAY_UNIQUE_PREFIX",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SERVICE_AREA
    inner join inserted
    on (HMR_SERVICE_AREA.SERVICE_AREA_NUMBER = inserted.SERVICE_AREA_NUMBER);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO

