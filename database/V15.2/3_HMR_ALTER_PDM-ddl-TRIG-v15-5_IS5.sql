/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:                                                          */
/* Author:                                                                */
/* Script type:           Alter database script                           */
/* Created on:            2020-02-11 06:41                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-11
-- Updates: 
--	
-- 
-- Description:	Incremnetal DML in support of sprint 5.
--  - Revise a few trigger names to meet DA standards
--
--
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


DROP TRIGGER [dbo].[FM5_I_S_I_TR]
GO


DROP TRIGGER [dbo].[FM5_I_S_U_TR]
GO


DROP TRIGGER [dbo].[SS19_I_S_I_TR]
GO


DROP TRIGGER [dbo].[SS19_I_S_U_TR]
GO


DROP TRIGGER [dbo].[SV22_I_S_I_TR]
GO


DROP TRIGGER [dbo].[SV22_I_S_U_TR]
GO


/* ---------------------------------------------------------------------- */
/* Repair/add triggers                                                    */
/* ---------------------------------------------------------------------- */

GO


CREATE TRIGGER [dbo].[HMR_FDBK_MSG_I_S_I_TR] ON HMR_FEEDBACK_MESSAGE INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted)
    RETURN;


  insert into HMR_FEEDBACK_MESSAGE ("FEEDBACK_MESSAGE_ID",
      "SUBMISSION_OBJECT_ID",
      "COMMUNICATION_SUBJECT",
      "COMMUNICATION_TEXT",
      "COMMUNICATION_DATE",
      "IS_SENT",
      "IS_ERROR",
      "SEND_ERROR_TEXT",
      "CONCURRENCY_CONTROL_NUMBER")
    select "FEEDBACK_MESSAGE_ID",
      "SUBMISSION_OBJECT_ID",
      "COMMUNICATION_SUBJECT",
      "COMMUNICATION_TEXT",
      "COMMUNICATION_DATE",
      "IS_SENT",
      "IS_ERROR",
      "SEND_ERROR_TEXT",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_FDBK_MSG_I_S_U_TR] ON HMR_FEEDBACK_MESSAGE INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.FEEDBACK_MESSAGE_ID = deleted.FEEDBACK_MESSAGE_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_FEEDBACK_MESSAGE
    set "FEEDBACK_MESSAGE_ID" = inserted."FEEDBACK_MESSAGE_ID",
      "SUBMISSION_OBJECT_ID" = inserted."SUBMISSION_OBJECT_ID",
      "COMMUNICATION_SUBJECT" = inserted."COMMUNICATION_SUBJECT",
      "COMMUNICATION_TEXT" = inserted."COMMUNICATION_TEXT",
      "COMMUNICATION_DATE" = inserted."COMMUNICATION_DATE",
      "IS_SENT" = inserted."IS_SENT",
      "IS_ERROR" = inserted."IS_ERROR",
      "SEND_ERROR_TEXT" = inserted."SEND_ERROR_TEXT",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_FEEDBACK_MESSAGE
    inner join inserted
    on (HMR_FEEDBACK_MESSAGE.FEEDBACK_MESSAGE_ID = inserted.FEEDBACK_MESSAGE_ID);

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
      "STATUS_TYPE",
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
      "STATUS_TYPE",
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
      "STATUS_TYPE" = inserted."STATUS_TYPE",
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


CREATE TRIGGER [dbo].[HMR_SYS_VLD_I_S_I_TR] ON HMR_SYSTEM_VALIDATION INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted)
    RETURN;


  insert into HMR_SYSTEM_VALIDATION ("SYSTEM_VALIDATION_ID",
      "ENTITY_NAME",
      "ATTRIBUTE_NAME",
      "ATTRIBUTE_TYPE",
      "IS_REQUIRED",
      "MAX_LENGTH",
      "MIN_LENGTH",
      "MAX_VALUE",
      "MIN_VALUE",
      "MAX_DATE",
      "MIN_DATE",
      "REG_EXP",
      "CODE_SET",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER")
    select "SYSTEM_VALIDATION_ID",
      "ENTITY_NAME",
      "ATTRIBUTE_NAME",
      "ATTRIBUTE_TYPE",
      "IS_REQUIRED",
      "MAX_LENGTH",
      "MIN_LENGTH",
      "MAX_VALUE",
      "MIN_VALUE",
      "MAX_DATE",
      "MIN_DATE",
      "REG_EXP",
      "CODE_SET",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_SYS_VLD_I_S_U_TR] ON HMR_SYSTEM_VALIDATION INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.SYSTEM_VALIDATION_ID = deleted.SYSTEM_VALIDATION_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SYSTEM_VALIDATION
    set "SYSTEM_VALIDATION_ID" = inserted."SYSTEM_VALIDATION_ID",
      "ENTITY_NAME" = inserted."ENTITY_NAME",
      "ATTRIBUTE_NAME" = inserted."ATTRIBUTE_NAME",
      "ATTRIBUTE_TYPE" = inserted."ATTRIBUTE_TYPE",
      "IS_REQUIRED" = inserted."IS_REQUIRED",
      "MAX_LENGTH" = inserted."MAX_LENGTH",
      "MIN_LENGTH" = inserted."MIN_LENGTH",
      "MAX_VALUE" = inserted."MAX_VALUE",
      "MIN_VALUE" = inserted."MIN_VALUE",
      "MAX_DATE" = inserted."MAX_DATE",
      "MIN_DATE" = inserted."MIN_DATE",
      "REG_EXP" = inserted."REG_EXP",
      "CODE_SET" = inserted."CODE_SET",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SYSTEM_VALIDATION
    inner join inserted
    on (HMR_SYSTEM_VALIDATION.SYSTEM_VALIDATION_ID = inserted.SYSTEM_VALIDATION_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO

