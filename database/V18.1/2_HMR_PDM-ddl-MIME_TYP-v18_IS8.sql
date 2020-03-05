/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting - v18             */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-02-28 15:01                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-29
-- Updates: 
--	
-- 
-- Description:	
--  - Trigger name revision to be standards compliant
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


DROP TRIGGER [dbo].[MT7_I_S_I_TR]
GO


DROP TRIGGER [dbo].[MT7_I_S_U_TR]
GO

/* ---------------------------------------------------------------------- */
/* Repair/add triggers                                                    */
/* ---------------------------------------------------------------------- */

GO


CREATE TRIGGER [dbo].[HMR_MIME_TYP_I_S_I_TR] ON HMR_MIME_TYPE INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted)
    RETURN;


  insert into HMR_MIME_TYPE ("MIME_TYPE_ID",
      "MIME_TYPE_CODE",
      "DESCRIPTION",
      "CONCURRENCY_CONTROL_NUMBER")
    select "MIME_TYPE_ID",
      "MIME_TYPE_CODE",
      "DESCRIPTION",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_MIME_TYP_I_S_U_TR] ON HMR_MIME_TYPE INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.MIME_TYPE_ID = deleted.MIME_TYPE_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_MIME_TYPE
    set "MIME_TYPE_ID" = inserted."MIME_TYPE_ID",
      "MIME_TYPE_CODE" = inserted."MIME_TYPE_CODE",
      "DESCRIPTION" = inserted."DESCRIPTION",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_MIME_TYPE
    inner join inserted
    on (HMR_MIME_TYPE.MIME_TYPE_ID = inserted.MIME_TYPE_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO

