/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:                                                          */
/* Author:                                                                */
/* Script type:           Alter database script                           */
/* Created on:            2020-02-09 03:30                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-11
-- Updates: 
--	
-- 
-- Description:	Incremnetal DML in support of sprint 5.
--  - Revise blob storage type from IMAGE to VARBINARY to achieve ANSI compatabilty standard
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* ---------------------------------------------------------------------- */
/* Drop foreign key constraints                                           */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] DROP CONSTRAINT [HMR_SUBM_OBJ_PRTY_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] DROP CONSTRAINT [HMR_SUBM_OBJ_SUBM_STR_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] DROP CONSTRAINT [HRM_SUBM_OBJ_MIME_TYPE_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] DROP CONSTRAINT [HRM_SUBM_OBJ_SRV_AREA_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] DROP CONSTRAINT [HRM_SUBM_OBJ_SUBM_STAT_CODE_FK]
GO


ALTER TABLE [dbo].[HMR_FEEDBACK_MESSAGE] DROP CONSTRAINT [HMR_FDBK_MSG_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RCKFL_RPT_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] DROP CONSTRAINT [HMR_SUBM_RW_HMR_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RPT_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_OBJ_FK]
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_SUBMISSION_OBJECT"                                */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] ALTER COLUMN [DIGITAL_REPRESENTATION] VARBINARY(MAX) NOT NULL
GO


/* ---------------------------------------------------------------------- */
/* Add foreign key constraints                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] ADD CONSTRAINT [HRM_SUBM_OBJ_MIME_TYPE_FK] 
    FOREIGN KEY ([MIME_TYPE_ID]) REFERENCES [dbo].[HMR_MIME_TYPE] ([MIME_TYPE_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] ADD CONSTRAINT [HRM_SUBM_OBJ_SRV_AREA_FK] 
    FOREIGN KEY ([SERVICE_AREA_NUMBER]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] ADD CONSTRAINT [HRM_SUBM_OBJ_SUBM_STAT_CODE_FK] 
    FOREIGN KEY ([SUBMISSION_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] ADD CONSTRAINT [HMR_SUBM_OBJ_PRTY_FK] 
    FOREIGN KEY ([PARTY_ID]) REFERENCES [dbo].[HMR_PARTY] ([PARTY_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_OBJECT] ADD CONSTRAINT [HMR_SUBM_OBJ_SUBM_STR_FK] 
    FOREIGN KEY ([SUBMISSION_STREAM_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STREAM] ([SUBMISSION_STREAM_ID])
GO


ALTER TABLE [dbo].[HMR_FEEDBACK_MESSAGE] ADD CONSTRAINT [HMR_FDBK_MSG_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] ADD CONSTRAINT [HMR_RCKFL_RPT_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ADD CONSTRAINT [HMR_SUBM_RW_HMR_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RPT_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO

