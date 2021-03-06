USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

-- Create foreign key constraint dbo.HMR_ACT_CD_RL_RD_CLSS_FK
PRINT N'Create foreign key constraint dbo.HMR_ACT_CD_RL_RD_CLSS_FK'
GO
ALTER TABLE [dbo].[HMR_ACTIVITY_CODE]
	ADD FOREIGN KEY([ROAD_CLASS_RULE])
	REFERENCES [dbo].[HMR_ACTIVITY_CODE_RULE]([ACTIVITY_CODE_RULE_ID])
	ON DELETE NO ACTION 
	ON UPDATE NO ACTION 
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO

-- Create foreign key constraint dbo.HMR_ACT_CD_RL_RD_LNGTH_FK
PRINT N'Create foreign key constraint dbo.HMR_ACT_CD_RL_RD_LNGTH_FK'
GO
ALTER TABLE [dbo].[HMR_ACTIVITY_CODE]
	ADD FOREIGN KEY([ROAD_LENGTH_RULE])
	REFERENCES [dbo].[HMR_ACTIVITY_CODE_RULE]([ACTIVITY_CODE_RULE_ID])
	ON DELETE NO ACTION 
	ON UPDATE NO ACTION 
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO

-- Create foreign key constraint dbo.HMR_ACT_CD_RL_SRFC_TYP_FK
PRINT N'Create foreign key constraint dbo.HMR_ACT_CD_RL_SRFC_TYP_FK'
GO
ALTER TABLE [dbo].[HMR_ACTIVITY_CODE]
	ADD FOREIGN KEY([SURFACE_TYPE_RULE])
	REFERENCES [dbo].[HMR_ACTIVITY_CODE_RULE]([ACTIVITY_CODE_RULE_ID])
	ON DELETE NO ACTION 
	ON UPDATE NO ACTION 
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO