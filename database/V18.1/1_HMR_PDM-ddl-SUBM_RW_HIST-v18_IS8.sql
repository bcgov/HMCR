/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting - v18             */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-03-04 13:58                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-03-04
-- Updates: 
--	
-- 
-- Description:	Incremental DML in support of sprint 8.
--  - Correct HMR_SUBMISSION_ROW_HIST.RECORD_NUMBER data length
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_SUBMISSION_ROW_HIST"                              */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW_HIST] ALTER COLUMN [RECORD_NUMBER] VARCHAR(30)
GO

