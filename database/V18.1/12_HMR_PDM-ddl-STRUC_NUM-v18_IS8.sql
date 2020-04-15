/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting                   */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-03-12 17:19                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-03-13
-- Updates: 
--	
-- 
-- Description:	Incremental DML in support of sprint 8.
--  - Revise STRUCTURE_NUMBER size within HMR_WORK_REPORT table to accomodate numbers > 5 characters
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


DROP TRIGGER [dbo].[HMR_WRK_RPT_I_S_U_TR]
GO


/* ---------------------------------------------------------------------- */
/* Drop foreign key constraints                                           */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_RW_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SRV_ARA_FK]
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_WORK_REPORT"                                      */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ALTER COLUMN [STRUCTURE_NUMBER] VARCHAR(30)
GO

-- MODIFIED: removed sp_updateextendedproperty scripts, as this table alter will not result in lost metadata


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_WORK_REPORT_HIST"                                 */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_WORK_REPORT_HIST] ALTER COLUMN [STRUCTURE_NUMBER] VARCHAR(30)
GO


/* ---------------------------------------------------------------------- */
/* Add foreign key constraints                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_STAT_FK] 
    FOREIGN KEY ([VALIDATION_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SRV_ARA_FK] 
    FOREIGN KEY ([SERVICE_AREA]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_RW_FK] 
    FOREIGN KEY ([ROW_ID]) REFERENCES [dbo].[HMR_SUBMISSION_ROW] ([ROW_ID])
GO


/* ---------------------------------------------------------------------- */
/* Repair/add triggers                                                    */
/* ---------------------------------------------------------------------- */

GO


CREATE TRIGGER [dbo].[HMR_WRK_RPT_I_S_U_TR] ON HMR_WORK_REPORT INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.WORK_REPORT_ID = deleted.WORK_REPORT_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_WORK_REPORT
    set "WORK_REPORT_ID" = inserted."WORK_REPORT_ID",
      "SUBMISSION_OBJECT_ID" = inserted."SUBMISSION_OBJECT_ID",
      "ROW_ID" = inserted."ROW_ID",
      "ROW_NUM" = inserted."ROW_NUM",
      "VALIDATION_STATUS_ID" = inserted."VALIDATION_STATUS_ID",
      "RECORD_TYPE" = inserted."RECORD_TYPE",
      "SERVICE_AREA" = inserted."SERVICE_AREA",
      "RECORD_NUMBER" = inserted."RECORD_NUMBER",
      "TASK_NUMBER" = inserted."TASK_NUMBER",
      "ACTIVITY_NUMBER" = inserted."ACTIVITY_NUMBER",
      "START_DATE" = inserted."START_DATE",
      "END_DATE" = inserted."END_DATE",
      "ACCOMPLISHMENT" = inserted."ACCOMPLISHMENT",
      "UNIT_OF_MEASURE" = inserted."UNIT_OF_MEASURE",
      "POSTED_DATE" = inserted."POSTED_DATE",
      "HIGHWAY_UNIQUE" = inserted."HIGHWAY_UNIQUE",
      "HIGHWAY_UNIQUE_NAME" = inserted."HIGHWAY_UNIQUE_NAME",
      "HIGHWAY_UNIQUE_LENGTH" = inserted."HIGHWAY_UNIQUE_LENGTH",
      "LANDMARK" = inserted."LANDMARK",
      "START_OFFSET" = inserted."START_OFFSET",
      "END_OFFSET" = inserted."END_OFFSET",
      "START_LATITUDE" = inserted."START_LATITUDE",
      "START_LONGITUDE" = inserted."START_LONGITUDE",
      "END_LATITUDE" = inserted."END_LATITUDE",
      "END_LONGITUDE" = inserted."END_LONGITUDE",
      "WORK_LENGTH" = inserted."WORK_LENGTH",
      "STRUCTURE_NUMBER" = inserted."STRUCTURE_NUMBER",
      "SITE_NUMBER" = inserted."SITE_NUMBER",
      "VALUE_OF_WORK" = inserted."VALUE_OF_WORK",
      "COMMENTS" = inserted."COMMENTS", 
      "GEOMETRY" = inserted."GEOMETRY",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_WORK_REPORT
    inner join inserted
    on (HMR_WORK_REPORT.WORK_REPORT_ID = inserted.WORK_REPORT_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO

