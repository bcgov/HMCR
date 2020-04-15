/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting - v18             */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-03-04 12:38                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-03-04
-- Updates: 
--	
-- 
-- Description:	Incremental DML in support of sprint 8.
--  - Revise join to optional between PARTY and CONTRACT_TERM to faciliate manual association
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


ALTER TABLE [dbo].[HMR_CONTRACT_TERM] DROP CONSTRAINT [HMR_CNRT_TRM_PRTY_FK]
GO


ALTER TABLE [dbo].[HMR_CONTRACT_TERM] DROP CONSTRAINT [HMR_CNRT_TRM_SRV_ARA_FK]
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_CONTRACT_TERM"                                    */
/* ---------------------------------------------------------------------- */

GO


DROP INDEX [dbo].[HMR_CONTRACT_TERM].[HMR_CNT_TRM_PRTY_FK_I]
GO


ALTER TABLE [dbo].[HMR_CONTRACT_TERM] DROP CONSTRAINT [HMR_CNRT_TRM_PK]
GO


ALTER TABLE [dbo].[HMR_CONTRACT_TERM] DROP CONSTRAINT [HMR_CNT_TRM_UQ_CH]
GO


ALTER TABLE [dbo].[HMR_CONTRACT_TERM] ALTER COLUMN [PARTY_ID] NUMERIC(9)
GO


ALTER TABLE [dbo].[HMR_CONTRACT_TERM] ADD CONSTRAINT [HMR_CNRT_TRM_PK] 
    PRIMARY KEY NONCLUSTERED ([CONTRACT_TERM_ID])
GO


ALTER TABLE [dbo].[HMR_CONTRACT_TERM] ADD CONSTRAINT [HMR_CNT_TRM_UQ_CH] 
    UNIQUE ([PARTY_ID], [SERVICE_AREA_NUMBER], [START_DATE])
GO


CREATE NONCLUSTERED INDEX [HMR_CNT_TRM_PRTY_FK_I] ON [dbo].[HMR_CONTRACT_TERM] ([PARTY_ID] ASC)
GO


/* ---------------------------------------------------------------------- */
/* Add foreign key constraints                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_CONTRACT_TERM] ADD CONSTRAINT [HMR_CNRT_TRM_PRTY_FK] 
    FOREIGN KEY ([PARTY_ID]) REFERENCES [dbo].[HMR_PARTY] ([PARTY_ID])
GO


ALTER TABLE [dbo].[HMR_CONTRACT_TERM] ADD CONSTRAINT [HMR_CNRT_TRM_SRV_ARA_FK] 
    FOREIGN KEY ([SERVICE_AREA_NUMBER]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


/* ---------------------------------------------------------------------- */
/* Alter table "dbo.HMR_CONTRACT_TERM_HIST"                               */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_CONTRACT_TERM_HIST] ALTER COLUMN [PARTY_ID] NUMERIC(18)
GO
