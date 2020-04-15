/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting - v17             */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-03-02 09:35                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-03-02
-- Updates: 
--	
-- 
-- Description:	Incremnetal DML in support of sprint 7.
--  - Correct triggers for START_VARIANCE and END_VARIANCE in SUBMISSION_ROW
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


DROP TRIGGER [dbo].[HMR_SUBM_RW_I_S_I_TR]
GO


DROP TRIGGER [dbo].[HMR_SUBM_RW_I_S_U_TR]
GO


/* ---------------------------------------------------------------------- */
/* Repair/add triggers                                                    */
/* ---------------------------------------------------------------------- */

GO


CREATE TRIGGER HMR_SUBM_RW_I_S_U_TR ON HMR_SUBMISSION_ROW INSTEAD OF UPDATE AS
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
go
GO


CREATE TRIGGER HMR_SUBM_RW_I_S_I_TR ON HMR_SUBMISSION_ROW INSTEAD OF INSERT AS
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
go
GO

