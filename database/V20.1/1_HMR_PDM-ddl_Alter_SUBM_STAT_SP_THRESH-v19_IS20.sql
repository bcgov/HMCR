/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR_updated_SUBM_STAT_SP_THRESH_21042020.dez*/
/* Project name:          Highway Maintenance Reporting                   */
/* Author:                Ayodeji Kuponiyi                                */
/* Script type:           Alter database script                           */
/* Created on:            2020-04-21 14:51                                */
/* ---------------------------------------------------------------------- */

USE HMR_DEV;


/*	Updates
i)		added missing descriptions to HMR_DEV
ii)		added 1 new column ( STAGE) to SUBMISSION_STATUS
iii)	added 2 new columns (WARNING_SP_THRESHOLD and ERROR_SP_THRESHOLD) to SUBMISSION_ROW and SUBMISSION_ROW_HIST

*/



/* ---------------------------------------------------------------------- */
/* Drop triggers                                                          */
/* ---------------------------------------------------------------------- */

GO


DROP TRIGGER [dbo].[HMR_SUBM_RW_A_S_IUD_TR]
GO


DROP TRIGGER [dbo].[HMR_SUBM_RW_I_S_I_TR]
GO


DROP TRIGGER [dbo].[HMR_SUBM_RW_I_S_U_TR]
GO


DROP TRIGGER [dbo].[HMR_SUBM_STAT_I_S_I_TR]
GO


DROP TRIGGER [dbo].[HMR_SUBM_STAT_I_S_U_TR]
GO


/* ---------------------------------------------------------------------- */
/* Drop foreign key constraints                                           */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] DROP CONSTRAINT [HMR_SUBM_OBJ_CNT_TRM_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] DROP CONSTRAINT [HMR_SUBM_RW_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] DROP CONSTRAINT [HMR_SUBM_RW_HMR_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RKFLL_RRT_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] DROP CONSTRAINT [HRM_SUBM_OBJ_SUBM_STAT_CODE_FK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RRT_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_RW_FK]
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RCKF_RRT_SUBM_RW_FK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RRT_SUBM_RW_FK]
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Digital file containing a batch of records being submitted for validation,  ingestion and reporting.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', NULL, NULL
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for a record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'SUBMISSION_OBJECT_ID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'The name of the document file name as was supplied by the user.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'FILE_NAME'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Raw file storage within the database.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'DIGITAL_REPRESENTATION'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Multipurpose Internet Mail Extensions (MIME) type of the submitted file', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'MIME_TYPE_ID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier relecting the current status of the submission.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'SUBMISSION_STATUS_ID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for SERVICE AREA', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'SERVICE_AREA_NUMBER'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier of related PARTY record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'PARTY_ID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Cryptographic hash for each submission object received. The hash total is used to compare with subsequently submitted data to check for duplicate submissions. If a match exists, newly matched data is not processed further.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'FILE_HASH'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Error descriptions applicable to an early stage file validation error (eg: missing mandatory column).', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'ERROR_DETAIL'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for SUBMISSION STREAM', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'SUBMISSION_STREAM_ID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'CONCURRENCY_CONTROL_NUMBER'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'APP_CREATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of record creation', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'APP_CREATE_TIMESTAMP'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'APP_CREATE_USER_GUID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'APP_CREATE_USER_DIRECTORY'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'APP_LAST_UPDATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of last record update', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'APP_LAST_UPDATE_TIMESTAMP'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'APP_LAST_UPDATE_USER_GUID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'APP_LAST_UPDATE_USER_DIRECTORY'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'DB_AUDIT_CREATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record created in the database', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'DB_AUDIT_CREATE_TIMESTAMP'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record was last updated in the database.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_OBJECT', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_TIMESTAMP'
GO


/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_SUBMISSION_ROW"                       */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] DROP CONSTRAINT [HMR_SUBM_RW_PK]
GO


CREATE TABLE [dbo].[HMR_SUBMISSION_ROW_TMP] (
    [ROW_ID] NUMERIC(9) DEFAULT NEXT VALUE FOR [HMR_SUBM_RW_ID_SEQ] NOT NULL,
    [SUBMISSION_OBJECT_ID] NUMERIC(9) NOT NULL,
    [ROW_STATUS_ID] NUMERIC(9),
    [ROW_NUM] NUMERIC(9),
    [RECORD_NUMBER] VARCHAR(30),
    [ROW_VALUE] VARCHAR(4000),
    [ROW_HASH] VARCHAR(256),
    [START_VARIANCE] NUMERIC(25,20),
    [END_VARIANCE] NUMERIC(25,20),
    [IS_RESUBMITTED] BIT,
    [ERROR_DETAIL] VARCHAR(4000),
    [WARNING_DETAIL] VARCHAR(4000),
    [WARNING_SP_THRESHOLD] NUMERIC(12,0),
    [ERROR_SP_THRESHOLD] NUMERIC(12,0),
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


INSERT INTO [dbo].[HMR_SUBMISSION_ROW_TMP]
    ([ROW_ID],[SUBMISSION_OBJECT_ID],[ROW_STATUS_ID],[ROW_NUM],[RECORD_NUMBER],[ROW_VALUE],[ROW_HASH],[START_VARIANCE],[END_VARIANCE],[IS_RESUBMITTED],[ERROR_DETAIL],[WARNING_DETAIL],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [ROW_ID],[SUBMISSION_OBJECT_ID],[ROW_STATUS_ID],[ROW_NUM],[RECORD_NUMBER],[ROW_VALUE],[ROW_HASH],[START_VARIANCE],[END_VARIANCE],[IS_RESUBMITTED],[ERROR_DETAIL],[WARNING_DETAIL],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_SUBMISSION_ROW]
GO


DROP TABLE [dbo].[HMR_SUBMISSION_ROW]
GO


EXEC sp_rename '[dbo].[HMR_SUBMISSION_ROW_TMP]', 'HMR_SUBMISSION_ROW', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ADD CONSTRAINT [HMR_SUBM_RW_PK] 
    PRIMARY KEY NONCLUSTERED ([ROW_ID])
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Each row of data within a  SUBMISSION OBJECT for each file submission that  passes basic file validation.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'A system generated unique identifier.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'ROW_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for a SUBMISSION OBJECT record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'SUBMISSION_OBJECT_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier relecting the current status of the submission row.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'ROW_STATUS_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Relative row number within the SUBMISSION_OBJECT.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'ROW_NUM'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique work report record number from the maintainence contractor. This is uniquely identifies each record submission for a contractor. <Service Area><Record Number> will uniquely identify each record in the application for a particular contractor.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'RECORD_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Contains a complete row of submitted data, including delimiters (ie: comma) and text qualifiers (ie: quote).  The row value is used to queue data for validation and loading.  This is staged data used to queue and compare data before loading it within the appropriate tables for reporting.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'ROW_VALUE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Cryptographic hash for each row of data received. The hash total is is used to compared with subsequently submitted data to check for duplicate submissions. If a match exists, newly matched data is not processed further.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'ROW_HASH'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Measured spatial distance from submitted coordinates to nearest road segment, as determined at time of validation.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'START_VARIANCE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Measured spatial distance from submitted coordinates to nearest road segment, as determined at time of validation.  Only applicable to submissions with end coordinates.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'END_VARIANCE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Indicates if the RECORD_NUMBER for the same CONTRACT_TERM and PARTY has been previously processed successfully and is being overwritten by a subsequent submission row.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'IS_RESUBMITTED'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Full listing of validation errors in JSON format for the submitted row.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'ERROR_DETAIL'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Full listing of validation warnings for the submitted row.  Thresholds can be  established whereby data will not be rejected, but a warning will be noted.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'WARNING_DETAIL'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Spatial warning threshold beyond which a warning is raised, when comparing input and actual values', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'WARNING_SP_THRESHOLD'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Spatial error threshold beyond which an error is raised, when comparing input and actual values', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'ERROR_SP_THRESHOLD'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'CONCURRENCY_CONTROL_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'APP_CREATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of record creation', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'APP_CREATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'APP_CREATE_USER_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'APP_CREATE_USER_DIRECTORY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'APP_LAST_UPDATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of last record update', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'APP_LAST_UPDATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'APP_LAST_UPDATE_USER_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'APP_LAST_UPDATE_USER_DIRECTORY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'DB_AUDIT_CREATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record created in the database', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'DB_AUDIT_CREATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record was last updated in the database.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_ROW', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_TIMESTAMP'
GO



/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_SUBMISSION_ROW_HIST"                  */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW_HIST] DROP CONSTRAINT [HMR_SUBM__H_PK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW_HIST] DROP CONSTRAINT [HMR_SUBM__H_UK]
GO


CREATE TABLE [dbo].[HMR_SUBMISSION_ROW_HIST_TMP] (
    [SUBMISSION_ROW_HIST_ID] BIGINT DEFAULT NEXT VALUE FOR [HMR_SUBMISSION_ROW_H_ID_SEQ] NOT NULL,
    [EFFECTIVE_DATE_HIST] DATETIME DEFAULT getutcdate() NOT NULL,
    [END_DATE_HIST] DATETIME,
    [ROW_ID] NUMERIC(18) NOT NULL,
    [SUBMISSION_OBJECT_ID] NUMERIC(18) NOT NULL,
    [ROW_STATUS_ID] NUMERIC(18),
    [ROW_NUM] NUMERIC(30),
    [RECORD_NUMBER] VARCHAR(30),
    [ROW_VALUE] VARCHAR(4000),
    [ROW_HASH] VARCHAR(256),
    [START_VARIANCE] NUMERIC(25,20),
    [END_VARIANCE] NUMERIC(25,20),
    [IS_RESUBMITTED] BIT,
    [ERROR_DETAIL] VARCHAR(4000),
    [WARNING_DETAIL] VARCHAR(4000),
    [WARNING_SP_THRESHOLD] NUMERIC(12,0),
    [ERROR_SP_THRESHOLD] NUMERIC(12,0),
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


INSERT INTO [dbo].[HMR_SUBMISSION_ROW_HIST_TMP]
    ([SUBMISSION_ROW_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[ROW_ID],[SUBMISSION_OBJECT_ID],[ROW_STATUS_ID],[ROW_NUM],[RECORD_NUMBER],[ROW_VALUE],[ROW_HASH],[START_VARIANCE],[END_VARIANCE],[IS_RESUBMITTED],[ERROR_DETAIL],[WARNING_DETAIL],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [SUBMISSION_ROW_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[ROW_ID],[SUBMISSION_OBJECT_ID],[ROW_STATUS_ID],[ROW_NUM],[RECORD_NUMBER],[ROW_VALUE],[ROW_HASH],[START_VARIANCE],[END_VARIANCE],[IS_RESUBMITTED],[ERROR_DETAIL],[WARNING_DETAIL],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_SUBMISSION_ROW_HIST]
GO


DROP TABLE [dbo].[HMR_SUBMISSION_ROW_HIST]
GO


EXEC sp_rename '[dbo].[HMR_SUBMISSION_ROW_HIST_TMP]', 'HMR_SUBMISSION_ROW_HIST', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW_HIST] ADD CONSTRAINT [HMR_SUBM__H_PK] 
    PRIMARY KEY CLUSTERED ([SUBMISSION_ROW_HIST_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW_HIST] ADD CONSTRAINT [HMR_SUBM__H_UK] 
    UNIQUE ([SUBMISSION_ROW_HIST_ID], [END_DATE_HIST])
GO


/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_SUBMISSION_STATUS"                    */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_STATUS] DROP CONSTRAINT [HMR_SUBMISSION_STATUS_CODE_PK]
GO


CREATE TABLE [dbo].[HMR_SUBMISSION_STATUS_TMP] (
    [STATUS_ID] NUMERIC(9) DEFAULT NEXT VALUE FOR [HMR_SUBM_STAT_ID_SEQ] NOT NULL,
    [STATUS_CODE] VARCHAR(20) NOT NULL,
    [DESCRIPTION] VARCHAR(150) DEFAULT '0' NOT NULL,
    [LONG_DESCRIPTION] VARCHAR(255),
    [STATUS_TYPE] VARCHAR(12),
    [STAGE] NUMERIC(9) DEFAULT 0 NOT NULL,
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


INSERT INTO [dbo].[HMR_SUBMISSION_STATUS_TMP]
    ([STATUS_ID],[STATUS_CODE],[DESCRIPTION],[LONG_DESCRIPTION],[STATUS_TYPE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [STATUS_ID],[STATUS_CODE],[DESCRIPTION],[LONG_DESCRIPTION],[STATUS_TYPE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_SUBMISSION_STATUS]
GO

DROP TABLE [dbo].[HMR_SUBMISSION_STATUS]
GO


EXEC sp_rename '[dbo].[HMR_SUBMISSION_STATUS_TMP]', 'HMR_SUBMISSION_STATUS', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_STATUS] ADD CONSTRAINT [HMR_SUBMISSION_STATUS_CODE_PK] 
    PRIMARY KEY CLUSTERED ([STATUS_ID])
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Indicates the statues a SUBMISSION_OBJECT can be assigned during ingestion (ie:  Received, Invalid, Valid)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for a record.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'STATUS_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Describes the file processing status.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'STATUS_CODE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Provides business description of the submission processing status', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'DESCRIPTION'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Full description of the status code.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'LONG_DESCRIPTION'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Indicates if status code is for SUBMISSION OBJECT, SUBMISSION  ROW or final staging table..', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'STATUS_TYPE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Stages in the processing/parsing of a submitted file', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'STAGE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'CONCURRENCY_CONTROL_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'APP_CREATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of record creation', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'APP_CREATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'APP_CREATE_USER_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'APP_CREATE_USER_DIRECTORY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'APP_LAST_UPDATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of last record update', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'APP_LAST_UPDATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'APP_LAST_UPDATE_USER_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'APP_LAST_UPDATE_USER_DIRECTORY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'DB_AUDIT_CREATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record created in the database', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'DB_AUDIT_CREATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record was last updated in the database.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_TIMESTAMP'
GO


/* ---------------------------------------------------------------------- */
/* Add foreign key constraints                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] ADD CONSTRAINT [HMR_SUBM_OBJ_CNT_TRM_FK] 
    FOREIGN KEY ([CONTRACT_TERM_ID]) REFERENCES [dbo].[HMR_CONTRACT_TERM] ([CONTRACT_TERM_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ADD CONSTRAINT [HMR_SUBM_RW_SUBM_STAT_FK] 
    FOREIGN KEY ([ROW_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ADD CONSTRAINT [HMR_SUBM_RW_HMR_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] ADD CONSTRAINT [HMR_RKFLL_RRT_SUBM_STAT_FK] 
    FOREIGN KEY ([VALIDATION_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] ADD CONSTRAINT [HRM_SUBM_OBJ_SUBM_STAT_CODE_FK] 
    FOREIGN KEY ([SUBMISSION_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RRT_SUBM_STAT_FK] 
    FOREIGN KEY ([VALIDATION_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_STAT_FK] 
    FOREIGN KEY ([VALIDATION_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_RW_FK] 
    FOREIGN KEY ([ROW_ID]) REFERENCES [dbo].[HMR_SUBMISSION_ROW] ([ROW_ID])
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] ADD CONSTRAINT [HMR_RCKF_RRT_SUBM_RW_FK] 
    FOREIGN KEY ([ROW_ID]) REFERENCES [dbo].[HMR_SUBMISSION_ROW] ([ROW_ID])
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RRT_SUBM_RW_FK] 
    FOREIGN KEY ([ROW_ID]) REFERENCES [dbo].[HMR_SUBMISSION_ROW] ([ROW_ID])
GO


/* ---------------------------------------------------------------------- */
/* Repair/add triggers                                                    */
/* ---------------------------------------------------------------------- */

GO


CREATE TRIGGER [dbo].[HMR_SUBM_RW_A_S_IUD_TR] ON HMR_SUBMISSION_ROW FOR INSERT, UPDATE, DELETE AS
SET NOCOUNT ON
BEGIN TRY
DECLARE @curr_date datetime;
SET @curr_date = getutcdate();
  IF NOT EXISTS(SELECT * FROM inserted) AND NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- historical
  IF EXISTS(SELECT * FROM deleted)
    update HMR_SUBMISSION_ROW_HIST set END_DATE_HIST = @curr_date where ROW_ID in (select ROW_ID from deleted) and END_DATE_HIST is null;

  IF EXISTS(SELECT * FROM inserted)
    insert into HMR_SUBMISSION_ROW_HIST ([ROW_ID], [SUBMISSION_OBJECT_ID], [ROW_STATUS_ID], [ROW_NUM], [RECORD_NUMBER], [ROW_VALUE], [ROW_HASH], [START_VARIANCE], [END_VARIANCE], [IS_RESUBMITTED], [ERROR_DETAIL],  [WARNING_DETAIL],[WARNING_SP_THRESHOLD], [ERROR_SP_THRESHOLD], [CONCURRENCY_CONTROL_NUMBER],  [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], SUBMISSION_ROW_HIST_ID, END_DATE_HIST, EFFECTIVE_DATE_HIST)
      select [ROW_ID], [SUBMISSION_OBJECT_ID], [ROW_STATUS_ID], [ROW_NUM], [RECORD_NUMBER], [ROW_VALUE], [ROW_HASH], [START_VARIANCE], [END_VARIANCE], [IS_RESUBMITTED], [ERROR_DETAIL], [WARNING_DETAIL], [WARNING_SP_THRESHOLD], [ERROR_SP_THRESHOLD], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_SUBMISSION_ROW_H_ID_SEQ]) as [SUBMISSION_ROW_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_SUBM_RW_I_S_I_TR] ON HMR_SUBMISSION_ROW INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted)
    RETURN;


  insert into HMR_SUBMISSION_ROW ("ROW_ID",
      "SUBMISSION_OBJECT_ID",
      "ROW_STATUS_ID",
      "ROW_NUM",
      "RECORD_NUMBER",
      "ROW_VALUE",
      "ROW_HASH",
      "START_VARIANCE",
      "END_VARIANCE",
      "IS_RESUBMITTED",
      "ERROR_DETAIL",
      "WARNING_DETAIL",
      "WARNING_SP_THRESHOLD",
      "ERROR_SP_THRESHOLD",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "ROW_ID",
      "SUBMISSION_OBJECT_ID",
      "ROW_STATUS_ID",
      "ROW_NUM",
      "RECORD_NUMBER",
      "ROW_VALUE",
      "ROW_HASH",
      "START_VARIANCE",
      "END_VARIANCE",
      "IS_RESUBMITTED",
      "ERROR_DETAIL",
      "WARNING_DETAIL",
      "WARNING_SP_THRESHOLD",
      "ERROR_SP_THRESHOLD",
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


CREATE TRIGGER [dbo].[HMR_SUBM_RW_I_S_U_TR] ON HMR_SUBMISSION_ROW INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.ROW_ID = deleted.ROW_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SUBMISSION_ROW
    set "ROW_ID" = inserted."ROW_ID",
      "SUBMISSION_OBJECT_ID" = inserted."SUBMISSION_OBJECT_ID",
      "ROW_STATUS_ID" = inserted."ROW_STATUS_ID",
      "ROW_NUM" = inserted."ROW_NUM",
      "RECORD_NUMBER" = inserted."RECORD_NUMBER",
      "ROW_VALUE" = inserted."ROW_VALUE",
      "ROW_HASH" = inserted."ROW_HASH",
      "START_VARIANCE" = inserted."START_VARIANCE",
      "END_VARIANCE" = inserted."END_VARIANCE",
      "IS_RESUBMITTED" = inserted."IS_RESUBMITTED",
      "ERROR_DETAIL" = inserted."ERROR_DETAIL",
      "WARNING_DETAIL" = inserted."WARNING_DETAIL",
      "WARNING_SP_THRESHOLD" = inserted."WARNING_SP_THRESHOLD",
      "ERROR_SP_THRESHOLD" = inserted."ERROR_SP_THRESHOLD",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SUBMISSION_ROW
    inner join inserted
    on (HMR_SUBMISSION_ROW.ROW_ID = inserted.ROW_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_SUBM_STAT_I_S_I_TR] ON HMR_SUBMISSION_STATUS INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted)
    RETURN;


  insert into HMR_SUBMISSION_STATUS ("STATUS_ID",
      "STATUS_CODE",
      "DESCRIPTION", 
      "LONG_DESCRIPTION",
      "STATUS_TYPE",
      "STAGE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "STATUS_ID",
      "STATUS_CODE",
      "DESCRIPTION",
      "LONG_DESCRIPTION",
      "STATUS_TYPE",
      "STAGE",
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


CREATE TRIGGER [dbo].[HMR_SUBM_STAT_I_S_U_TR] ON HMR_SUBMISSION_STATUS INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.STATUS_ID = deleted.STATUS_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SUBMISSION_STATUS
    set "STATUS_ID" = inserted."STATUS_ID",
      "STATUS_CODE" = inserted."STATUS_CODE",
      "DESCRIPTION" = inserted."DESCRIPTION", 
      "LONG_DESCRIPTION" = inserted."LONG_DESCRIPTION",
      "STATUS_TYPE" = inserted."STATUS_TYPE",
      "STAGE" = inserted."STAGE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SUBMISSION_STATUS
    inner join inserted
    on (HMR_SUBMISSION_STATUS.STATUS_ID = inserted.STATUS_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO

