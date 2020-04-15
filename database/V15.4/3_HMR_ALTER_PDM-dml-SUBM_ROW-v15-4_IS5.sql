/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:                                                          */
/* Author:                                                                */
/* Script type:           Alter database script                           */
/* Created on:            2020-02-14 16:11                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-14
-- Updates: 
--	
-- 
-- Description:	Incremnetal DML in support of sprint 5.
--  - increase size of HMR_SUBMISSION_ROW.RECORD_NUMBER to accomodate revised scale.
-- 
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


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] DROP CONSTRAINT [HMR_SUBM_RW_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] DROP CONSTRAINT [HMR_SUBM_RW_HMR_SUBM_OBJ_FK]
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_SUBMISSION_ROW"                                   */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ALTER COLUMN [RECORD_NUMBER] VARCHAR(30)
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_SUBMISSION_ROW_HIST"                              */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW_HIST] ALTER COLUMN [ROW_NUM] NUMERIC(30)
GO


/* ---------------------------------------------------------------------- */
/* Add foreign key constraints                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ADD CONSTRAINT [HMR_SUBM_RW_SUBM_STAT_FK] 
    FOREIGN KEY ([ROW_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ADD CONSTRAINT [HMR_SUBM_RW_HMR_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO
