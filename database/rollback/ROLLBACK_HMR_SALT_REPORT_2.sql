
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_VULNAREA', @level2type=N'COLUMN',@level2name=N'ENV_MONITORING'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_VULNAREA', @level2type=N'COLUMN',@level2name=N'PROT_MEASURES'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_VULNAREA', @level2type=N'COLUMN',@level2name=N'TYPE'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_VULNAREA', @level2type=N'COLUMN',@level2name=N'FEATURE'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_VULNAREA', @level2type=N'COLUMN',@level2name=N'LONG'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_VULNAREA', @level2type=N'COLUMN',@level2name=N'LAT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_VULNAREA', @level2type=N'COLUMN',@level2name=N'HWY_NO'
GO
/****** Object:  Trigger [HMR_SLTVUL_PK_I_S_U_TR]    Script Date: 2024-05-24 2:30:13 PM ******/
DROP TRIGGER [dbo].[HMR_SLTVUL_PK_I_S_U_TR]
GO
/****** Object:  Trigger [HMR_SLTVUL_PK_I_S_I_TR]    Script Date: 2024-05-24 2:30:13 PM ******/
DROP TRIGGER [dbo].[HMR_SLTVUL_PK_I_S_I_TR]
GO
/****** Object:  Trigger [HMR_SLTVUL_PK_A_S_IUD_TR]    Script Date: 2024-05-24 2:30:13 PM ******/
DROP TRIGGER [dbo].[HMR_SLTVUL_PK_A_S_IUD_TR]
GO
ALTER TABLE [dbo].[HMR_SALT_VULNAREA] DROP CONSTRAINT [HMR_SALT_VULNAREA__SALT_REPORT_FK]
GO
ALTER TABLE [dbo].[HMR_SALT_VULNAREA] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__6B84DD35]
GO
ALTER TABLE [dbo].[HMR_SALT_VULNAREA] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__6A90B8FC]
GO
ALTER TABLE [dbo].[HMR_SALT_VULNAREA] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__699C94C3]
GO
ALTER TABLE [dbo].[HMR_SALT_VULNAREA] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__68A8708A]
GO
ALTER TABLE [dbo].[HMR_SALT_VULNAREA] DROP CONSTRAINT [DF__HMR_SALT___CONCU__67B44C51]
GO
ALTER TABLE [dbo].[HMR_SALT_VULNAREA] DROP CONSTRAINT [DF_HMR_SALT_VULNAREA_VULNAREA_ID]
GO
/****** Object:  Table [dbo].[HMR_SALT_VULNAREA_HIST]    Script Date: 2024-05-24 2:30:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HMR_SALT_VULNAREA_HIST]') AND type in (N'U'))
DROP TABLE [dbo].[HMR_SALT_VULNAREA_HIST]
GO
/****** Object:  Table [dbo].[HMR_SALT_VULNAREA]    Script Date: 2024-05-24 2:30:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HMR_SALT_VULNAREA]') AND type in (N'U'))
DROP TABLE [dbo].[HMR_SALT_VULNAREA]
GO

DROP SEQUENCE [dbo].[HMR_SLT_VULNAREA_ID_SEQ];
DROP SEQUENCE [dbo].[HMR_SALT_VULNAREA_H_ID_SEQ];

DELETE FROM [dbo].[HMR_SUBMISSION_STREAM]
WHERE [STAGING_TABLE_NAME] = 'HMR_SALT_REPORT';
