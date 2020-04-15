/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting                   */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-04-14 15:57                                */
/* ---------------------------------------------------------------------- */
-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-04-14
-- Updates: 
--	
-- 
-- Description:	
--  - Increase size of HMR_PARTY.DISPLAY_NAME to 200 characters.  FK constraint management not required.
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* ---------------------------------------------------------------------- */
/* REMOVED: Drop foreign key constraints                                  */
/* ---------------------------------------------------------------------- */


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_PARTY"                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_PARTY] ALTER COLUMN [DISPLAY_NAME] VARCHAR(200)
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_PARTY_HIST"                                       */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_PARTY_HIST] ALTER COLUMN [DISPLAY_NAME] VARCHAR(200)
GO


/* ---------------------------------------------------------------------- */
/* REMOVED: Add foreign key constraints                                   */
/* ---------------------------------------------------------------------- */


