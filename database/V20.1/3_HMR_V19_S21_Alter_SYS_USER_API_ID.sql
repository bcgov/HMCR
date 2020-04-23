/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR_updated_SYS_USER_API_ID_23042020.dez    */
/* Project name:          Highway Maintenance Reporting                   */
/* Author:                Ayodeji Kuponiyi                                */
/* Script type:           Alter database script                           */
/* Created on:            2020-04-23 13:05                                */
/* ---------------------------------------------------------------------- */

USE HMR_DEV;

/* Update 23/04/2020

i) Added API_CLIENT_ID to HMR_SYSTEM_USER table - to track Keycloak client ID created for the users

*/



/* ---------------------------------------------------------------------- */
/* Drop triggers                                                          */
/* ---------------------------------------------------------------------- */

GO


DROP TRIGGER [dbo].[HMR_SYS_USR_A_S_IUD_TR]
GO


DROP TRIGGER [dbo].[HMR_SYS_USR_I_S_I_TR]
GO


DROP TRIGGER [dbo].[HMR_SYS_USR_I_S_U_TR]
GO


/* ---------------------------------------------------------------------- */
/* Drop foreign key constraints                                           */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SYSTEM_USER] DROP CONSTRAINT [HMR_SYS_USR_PRTY_FK]
GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA_USER] DROP CONSTRAINT [HMR_SRV_AREA_USR_SYS_USR_FK]
GO


ALTER TABLE [dbo].[HMR_USER_ROLE] DROP CONSTRAINT [HMR_USR_RL_SYS_USR_FK]
GO


/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_SYSTEM_USER"                          */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SYSTEM_USER] DROP CONSTRAINT [HMR_SYSTEM_USER_PK]
GO


CREATE TABLE [dbo].[HMR_SYSTEM_USER_TMP] (
    [SYSTEM_USER_ID] NUMERIC(9) DEFAULT NEXT VALUE FOR [SYS_USR_ID_SEQ] NOT NULL,
    [PARTY_ID] NUMERIC(9),
    [API_CLIENT_ID] VARCHAR(40),
    [USER_GUID] UNIQUEIDENTIFIER,
    [USERNAME] VARCHAR(32) NOT NULL,
    [USER_DIRECTORY] VARCHAR(30),
    [USER_TYPE] VARCHAR(30),
    [FIRST_NAME] VARCHAR(150),
    [LAST_NAME] VARCHAR(150),
    [EMAIL] VARCHAR(100),
    [BUSINESS_GUID] UNIQUEIDENTIFIER,
    [BUSINESS_LEGAL_NAME] VARCHAR(200),
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


INSERT INTO [dbo].[HMR_SYSTEM_USER_TMP]
    ([SYSTEM_USER_ID],[PARTY_ID],[USER_GUID],[USERNAME],[USER_DIRECTORY],[USER_TYPE],[FIRST_NAME],[LAST_NAME],[EMAIL],[BUSINESS_GUID],[BUSINESS_LEGAL_NAME],[END_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [SYSTEM_USER_ID],[PARTY_ID],[USER_GUID],[USERNAME],[USER_DIRECTORY],[USER_TYPE],[FIRST_NAME],[LAST_NAME],[EMAIL],[BUSINESS_GUID],[BUSINESS_LEGAL_NAME],[END_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_SYSTEM_USER]
GO


DROP INDEX [dbo].[HMR_SYSTEM_USER].[HMR_SYSTEM_USER_FK_I]
GO


DROP TABLE [dbo].[HMR_SYSTEM_USER]
GO


EXEC sp_rename '[dbo].[HMR_SYSTEM_USER_TMP]', 'HMR_SYSTEM_USER', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_SYSTEM_USER] ADD CONSTRAINT [HMR_SYSTEM_USER_PK] 
    PRIMARY KEY CLUSTERED ([SYSTEM_USER_ID])
GO


CREATE NONCLUSTERED INDEX [HMR_SYSTEM_USER_FK_I] ON [dbo].[HMR_SYSTEM_USER] ([PARTY_ID] ASC)
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Defines users and their attributes as found in IDIR or BCeID.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'A system generated unique identifier.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'SYSTEM_USER_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'A system generated unique identifier.  Reflects the party record for the individual who has been assigned any roles.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'PARTY_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'This ID is used to track Keycloak client ID created for the users', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'API_CLIENT_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'A system generated unique identifier.  Reflects the active directory unique idenifier for the user.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'USER_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'IDIR or BCeID Active Directory defined universal identifier (SM_UNIVERSALID or userID) attributed to a user.  This value can change over time, while USER_GUID will remain consistant.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'USERNAME'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Directory (IDIR / BCeID/Oracle) in which the userid is defined.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'USER_DIRECTORY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Defined attribute within IDIR Active directory (UserType = SMGOV_USERTYPE)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'USER_TYPE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'First Name of the user', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'FIRST_NAME'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Last Name of the user', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'LAST_NAME'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Contact email address within Active Directory (Email = SMGOV_EMAIL)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'EMAIL'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'A system generated unique identifier.  Reflects the active directory unique idenifier for the business associated with the user.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'BUSINESS_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Lega lName assigned to the business and derived from BC Registry via BCeID (SMGOV_BUSINESSLEGALNAME)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'BUSINESS_LEGAL_NAME'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date a user can no longer access the system or invoke data submissions.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'END_DATE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'CONCURRENCY_CONTROL_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'APP_CREATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of record creation', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'APP_CREATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'APP_CREATE_USER_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'APP_CREATE_USER_DIRECTORY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'APP_LAST_UPDATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of last record update', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'APP_LAST_UPDATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'APP_LAST_UPDATE_USER_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'APP_LAST_UPDATE_USER_DIRECTORY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'DB_AUDIT_CREATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record created in the database', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'DB_AUDIT_CREATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record was last updated in the database.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SYSTEM_USER', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_TIMESTAMP'
GO



/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_SYSTEM_USER_HIST"                     */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SYSTEM_USER_HIST] DROP CONSTRAINT [HMR_SYS_U_H_PK]
GO


ALTER TABLE [dbo].[HMR_SYSTEM_USER_HIST] DROP CONSTRAINT [HMR_SYS_U_H_UK]
GO


CREATE TABLE [dbo].[HMR_SYSTEM_USER_HIST_TMP] (
    [SYSTEM_USER_HIST_ID] BIGINT DEFAULT NEXT VALUE FOR [HMR_SYSTEM_USER_H_ID_SEQ] NOT NULL,
    [EFFECTIVE_DATE_HIST] DATETIME DEFAULT getutcdate() NOT NULL,
    [END_DATE_HIST] DATETIME,
    [SYSTEM_USER_ID] NUMERIC(18) NOT NULL,
    [PARTY_ID] NUMERIC(18),
    [API_CLIENT_ID] VARCHAR(40),
    [USER_GUID] UNIQUEIDENTIFIER,
    [USERNAME] VARCHAR(32) NOT NULL,
    [USER_DIRECTORY] VARCHAR(30),
    [USER_TYPE] VARCHAR(30),
    [FIRST_NAME] VARCHAR(150),
    [LAST_NAME] VARCHAR(150),
    [EMAIL] VARCHAR(100),
    [BUSINESS_GUID] UNIQUEIDENTIFIER,
    [BUSINESS_LEGAL_NAME] VARCHAR(200),
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


INSERT INTO [dbo].[HMR_SYSTEM_USER_HIST_TMP]
    ([SYSTEM_USER_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[SYSTEM_USER_ID],[PARTY_ID],[USER_GUID],[USERNAME],[USER_DIRECTORY],[USER_TYPE],[FIRST_NAME],[LAST_NAME],[EMAIL],[BUSINESS_GUID],[BUSINESS_LEGAL_NAME],[END_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [SYSTEM_USER_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[SYSTEM_USER_ID],[PARTY_ID],[USER_GUID],[USERNAME],[USER_DIRECTORY],[USER_TYPE],[FIRST_NAME],[LAST_NAME],[EMAIL],[BUSINESS_GUID],[BUSINESS_LEGAL_NAME],[END_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_SYSTEM_USER_HIST]
GO


DROP TABLE [dbo].[HMR_SYSTEM_USER_HIST]
GO


EXEC sp_rename '[dbo].[HMR_SYSTEM_USER_HIST_TMP]', 'HMR_SYSTEM_USER_HIST', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_SYSTEM_USER_HIST] ADD CONSTRAINT [HMR_SYS_U_H_PK] 
    PRIMARY KEY CLUSTERED ([SYSTEM_USER_HIST_ID])
GO


ALTER TABLE [dbo].[HMR_SYSTEM_USER_HIST] ADD CONSTRAINT [HMR_SYS_U_H_UK] 
    UNIQUE ([SYSTEM_USER_HIST_ID], [END_DATE_HIST])
GO


/* ---------------------------------------------------------------------- */
/* Add foreign key constraints                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SYSTEM_USER] ADD CONSTRAINT [HMR_SYS_USR_PRTY_FK] 
    FOREIGN KEY ([PARTY_ID]) REFERENCES [dbo].[HMR_PARTY] ([PARTY_ID])
GO


ALTER TABLE [dbo].[HMR_SERVICE_AREA_USER] ADD CONSTRAINT [HMR_SRV_AREA_USR_SYS_USR_FK] 
    FOREIGN KEY ([SYSTEM_USER_ID]) REFERENCES [dbo].[HMR_SYSTEM_USER] ([SYSTEM_USER_ID])
GO


ALTER TABLE [dbo].[HMR_USER_ROLE] ADD CONSTRAINT [HMR_USR_RL_SYS_USR_FK] 
    FOREIGN KEY ([SYSTEM_USER_ID]) REFERENCES [dbo].[HMR_SYSTEM_USER] ([SYSTEM_USER_ID])
GO


/* ---------------------------------------------------------------------- */
/* Repair/add triggers                                                    */
/* ---------------------------------------------------------------------- */

GO


CREATE TRIGGER [dbo].[HMR_SYS_USR_A_S_IUD_TR] ON HMR_SYSTEM_USER FOR INSERT, UPDATE, DELETE AS
SET NOCOUNT ON
BEGIN TRY
DECLARE @curr_date datetime;
SET @curr_date = getutcdate();
  IF NOT EXISTS(SELECT * FROM inserted) AND NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- historical
  IF EXISTS(SELECT * FROM deleted)
    update HMR_SYSTEM_USER_HIST set END_DATE_HIST = @curr_date where SYSTEM_USER_ID in (select SYSTEM_USER_ID from deleted) and END_DATE_HIST is null;

  IF EXISTS(SELECT * FROM inserted)
    insert into HMR_SYSTEM_USER_HIST ([SYSTEM_USER_ID], [PARTY_ID], [API_CLIENT_ID], [USER_GUID], [USERNAME], [USER_DIRECTORY], [USER_TYPE], [FIRST_NAME], [LAST_NAME], [EMAIL], [BUSINESS_GUID], [BUSINESS_LEGAL_NAME], [END_DATE], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], SYSTEM_USER_HIST_ID, END_DATE_HIST, EFFECTIVE_DATE_HIST)
      select [SYSTEM_USER_ID], [PARTY_ID], [API_CLIENT_ID],[USER_GUID], [USERNAME], [USER_DIRECTORY], [USER_TYPE], [FIRST_NAME], [LAST_NAME], [EMAIL], [BUSINESS_GUID], [BUSINESS_LEGAL_NAME], [END_DATE], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_SYSTEM_USER_H_ID_SEQ]) as [SYSTEM_USER_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_SYS_USR_I_S_I_TR] ON HMR_SYSTEM_USER INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted)
    RETURN;


  insert into HMR_SYSTEM_USER ("SYSTEM_USER_ID",
      "PARTY_ID",  
      "API_CLIENT_ID",
      "USER_GUID",
      "USERNAME",
      "USER_DIRECTORY",
      "USER_TYPE",
      "FIRST_NAME",
      "LAST_NAME",
      "EMAIL",
      "BUSINESS_GUID",
      "BUSINESS_LEGAL_NAME",
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
    select "SYSTEM_USER_ID",
      "PARTY_ID",
      "API_CLIENT_ID",
      "USER_GUID",
      "USERNAME",
      "USER_DIRECTORY",
      "USER_TYPE",
      "FIRST_NAME",
      "LAST_NAME",
      "EMAIL",
      "BUSINESS_GUID",
      "BUSINESS_LEGAL_NAME",
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


CREATE TRIGGER [dbo].[HMR_SYS_USR_I_S_U_TR] ON HMR_SYSTEM_USER INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.SYSTEM_USER_ID = deleted.SYSTEM_USER_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SYSTEM_USER
    set "SYSTEM_USER_ID" = inserted."SYSTEM_USER_ID",
      "PARTY_ID" = inserted."PARTY_ID", 
      "API_CLIENT_ID" = inserted."API_CLIENT_ID",
      "USER_GUID" = inserted."USER_GUID",
      "USERNAME" = inserted."USERNAME",
      "USER_DIRECTORY" = inserted."USER_DIRECTORY",
      "USER_TYPE" = inserted."USER_TYPE",
      "FIRST_NAME" = inserted."FIRST_NAME",
      "LAST_NAME" = inserted."LAST_NAME",
      "EMAIL" = inserted."EMAIL",
      "BUSINESS_GUID" = inserted."BUSINESS_GUID",
      "BUSINESS_LEGAL_NAME" = inserted."BUSINESS_LEGAL_NAME",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SYSTEM_USER
    inner join inserted
    on (HMR_SYSTEM_USER.SYSTEM_USER_ID = inserted.SYSTEM_USER_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO

