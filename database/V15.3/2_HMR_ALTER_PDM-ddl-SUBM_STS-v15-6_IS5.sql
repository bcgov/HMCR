/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:                                                          */
/* Author:                                                                */
/* Script type:           Alter database script                           */
/* Created on:            2020-02-12 18:16                                */
/* ---------------------------------------------------------------------- */


-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-12
-- Updates: 
--	
-- 
-- Description:	Incremnetal DML in support of sprint 5.
--  - Restore table and field definitions HMR_SUBMISSION_STATUS
--
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* ---------------------------------------------------------------------- */
/* Restore table and field definitions                                     */
/* ---------------------------------------------------------------------- */

EXECUTE sp_addextendedproperty N'MS_Description', N'Indicates the statues a SUBMISSION_OBJECT can be assigned during ingestion (ie:  Received, Invalid, Valid)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for a record.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'STATUS_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Describes the file processing status.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'STATUS_CODE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Provides business description of the submission processing status', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'DESCRIPTION'
GO

-- already applied when field was added
--EXECUTE sp_addextendedproperty N'MS_Description', N'Full description of the status code.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'LONG_DESCRIPTION'
--GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Indicates if status code is for SUBMISSION OBJECT, SUBMISSION  ROW or final staging table..', 'SCHEMA', N'dbo', 'TABLE', N'HMR_SUBMISSION_STATUS', 'COLUMN', N'STATUS_TYPE'
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
