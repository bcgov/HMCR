-- Script to rollback Salt Reporting features in HMCR database.

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'WETLANDS_PROT_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'WETLANDS_CHLOR_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'WETLANDS_AREA_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'VAL_LANDS_PROT_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'VAL_LANDS_CHLOR_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'VAL_LANDS_AREA_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DRINK_WATER_PROT_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DRINK_WATER_CHLOR_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DRINK_WATER_AREA_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DELIM_AREA_PROT_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DELIM_AREA_CHLOR_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DELIM_AREA_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'AQUA_LIFE_PROT_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'AQUA_LIFE_CHLOR_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'AQUA_LIFE_AREA_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'ENV_MON_COND'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PROT_MEAS_IMPL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'ACT_PLAN_PREP'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'SET_VULN_AREAS'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'COMP_INV'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MELTWATER_DISP_METH_USED'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'SNOW_MELT_USED'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'SNOW_DISP_SITE_TOT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'TEST_SMDS_TOT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'TEST_MDSS_REL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'APP_RATE_CHRT_TOT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'APP_RATE_CHRT_REL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'SALT_APP_RATE_TOT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'SALT_APP_RATE_REL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'AVL_TOT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'AVL_REL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MOB_RWIS_TOT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MOB_RWIS_REL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'FIX_RWIS_TOT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'FIX_RWIS_REL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MET_SVC_REL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'IR_THRM_TOT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'IR_THRM_REL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'REG_CALIB_TOT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'REG_CALIB'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'VEH_DLA'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'VEH_PREWET_EQ'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'VEH_CONV'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'VEH_SALT_APP'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'NUM_VEHICLES'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'RISK_MGMT_PLAN_SITES'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'RISK_MGMT_PLAN_PLAN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'REMOV_CONT_SITES'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'REMOV_CONT_PLAN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'OTH_DISCH_PT_SITES'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'OTH_DISCH_PT_PLAN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'ONGOING_CLNUP_SITES'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'ONGOING_CLNUP_PLAN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MUN_SEWER_SYS_SITES'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MUN_SEWER_SYS_PLAN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'EQP_PRVNT_OVRLOAD_SITES'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'EQP_PRVNT_OVRLOAD_PLAN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DRAIN_COLL_SYS_SITES'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DRAIN_COLL_SYS_PLAN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'CTRL_DIV_EXT_WAT_SITES'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'CTRL_DIV_EXT_WAT_PLAN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'ALL_MAT_HNDL_SITES'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'ALL_MAT_HNDL_PLAN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'SALT_STRG_SITES_TOT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MULTICHL_B_CACL2_PCT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MULTICHL_B_NACL_PCT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MULTICHL_B_MGCL2_PCT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MULTICHL_B_LTRS'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MULTICHL_A_MGCL2_PCT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MULTICHL_A_NACL_PCT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MULTICHL_A_LTRS'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MULTICHL_A_CACL2_PCT'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'ANTIC_NONCL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'ANTIC_ACET'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'ANTIC_CACL2'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'ANTIC_MGCL2'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'ANTIC_NACL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PRTT_NONCL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PRTT_ACET'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PRTT_CACL2'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PRTT_MGCL2'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PRTT_NACL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PRWT_NONCL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PRWT_ACET'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PRWT_CACL2'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PRWT_MGCL2'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PRWT_NACL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'TRTD_ABR_CACL2'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'TRTD_ABR_MGCL2'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'TRTD_ABR_NACL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'TRTD_ABR_SDST'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DEICER_ACET'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DEICER_CACL2'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DEICER_MGCL2'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'DEICER_NACL'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'SALT_TOT_DAYS'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'RD_TOT_LEN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'SALT_APP_ACH'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'SALT_APP_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MAT_STOR_ACH'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MAT_STOR_ID'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PATR_TRAIN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MECH_TRAIN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'OPR_TRAIN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'SUPV_TRAIN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'MGR_TRAIN'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PLAN_UPD'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PLAN_REV'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HMR_SALT_REPORT', @level2type=N'COLUMN',@level2name=N'PLAN_DEV'
GO
/****** Object:  Trigger [HMR_SLTSTP_PK_I_S_U_TR]    Script Date: 2024-04-16 11:42:00 AM ******/
DROP TRIGGER [dbo].[HMR_SLTSTP_PK_I_S_U_TR]
GO
/****** Object:  Trigger [HMR_SLTSTP_PK_I_S_I_TR]    Script Date: 2024-04-16 11:42:00 AM ******/
DROP TRIGGER [dbo].[HMR_SLTSTP_PK_I_S_I_TR]
GO
/****** Object:  Trigger [HMR_SLTSTP_PK_A_S_IUD_TR]    Script Date: 2024-04-16 11:42:00 AM ******/
DROP TRIGGER [dbo].[HMR_SLTSTP_PK_A_S_IUD_TR]
GO
/****** Object:  Trigger [HMR_SLTRPT_PK_I_S_U_TR]    Script Date: 2024-04-16 11:42:00 AM ******/
DROP TRIGGER [dbo].[HMR_SLTRPT_PK_I_S_U_TR]
GO
/****** Object:  Trigger [HMR_SLTRPT_PK_I_S_I_TR]    Script Date: 2024-04-16 11:42:00 AM ******/
DROP TRIGGER [dbo].[HMR_SLTRPT_PK_I_S_I_TR]
GO
/****** Object:  Trigger [HMR_SLTRPT_PK_A_S_IUD_TR]    Script Date: 2024-04-16 11:42:00 AM ******/
DROP TRIGGER [dbo].[HMR_SLTRPT_PK_A_S_IUD_TR]
GO
/****** Object:  Trigger [HMR_SLTAPP_PK_I_S_U_TR]    Script Date: 2024-04-16 11:42:00 AM ******/
DROP TRIGGER [dbo].[HMR_SLTAPP_PK_I_S_U_TR]
GO
/****** Object:  Trigger [HMR_SLTAPP_PK_I_S_I_TR]    Script Date: 2024-04-16 11:42:00 AM ******/
DROP TRIGGER [dbo].[HMR_SLTAPP_PK_I_S_I_TR]
GO
/****** Object:  Trigger [HMR_SLTAPP_PK_A_S_IUD_TR]    Script Date: 2024-04-16 11:42:00 AM ******/
DROP TRIGGER [dbo].[HMR_SLTAPP_PK_A_S_IUD_TR]
GO
ALTER TABLE [dbo].[HMR_SALT_STOCKPILE] DROP CONSTRAINT [HMR_SALT_STOCKPILE__SALT_REPORT_FK]
GO
ALTER TABLE [dbo].[HMR_SALT_REPORT] DROP CONSTRAINT [HMR_SALT_REPORT__SERVICE_AREA_FK]
GO
ALTER TABLE [dbo].[HMR_SALT_APPENDIX] DROP CONSTRAINT [HMR_SALT_APPENDIX__SALT_REPORT_FK]
GO
ALTER TABLE [dbo].[HMR_SALT_STOCKPILE_HIST] DROP CONSTRAINT [DF__HMR_SALT___EFFEC__3535502A]
GO
ALTER TABLE [dbo].[HMR_SALT_STOCKPILE_HIST] DROP CONSTRAINT [DF__HMR_SALT___SALT___51D18ED8]
GO
ALTER TABLE [dbo].[HMR_SALT_STOCKPILE] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__56F3D4A3]
GO
ALTER TABLE [dbo].[HMR_SALT_STOCKPILE] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__55FFB06A]
GO
ALTER TABLE [dbo].[HMR_SALT_STOCKPILE] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__550B8C31]
GO
ALTER TABLE [dbo].[HMR_SALT_STOCKPILE] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__541767F8]
GO
ALTER TABLE [dbo].[HMR_SALT_STOCKPILE] DROP CONSTRAINT [DF__HMR_SALT___CONCU__532343BF]
GO
ALTER TABLE [dbo].[HMR_SALT_STOCKPILE] DROP CONSTRAINT [DF_HMR_SALT_STOCKPILE_STOCKPILE_ID]
GO
ALTER TABLE [dbo].[HMR_SALT_REPORT_HIST] DROP CONSTRAINT [DF__HMR_SALT___EFFEC__2F7C76D4]
GO
ALTER TABLE [dbo].[HMR_SALT_REPORT_HIST] DROP CONSTRAINT [DF__HMR_SALT___SALT___4FE94666]
GO
ALTER TABLE [dbo].[HMR_SALT_REPORT] DROP CONSTRAINT [DF_HMR_SALT_REPORT_DB_AUDIT_LAST_UPDATE_TIMESTAMP]
GO
ALTER TABLE [dbo].[HMR_SALT_REPORT] DROP CONSTRAINT [DF_HMR_SALT_REPORT_DB_AUDIT_LAST_UPDATE_USERID]
GO
ALTER TABLE [dbo].[HMR_SALT_REPORT] DROP CONSTRAINT [DF_HMR_SALT_REPORT_DB_AUDIT_CREATE_TIMESTAMP]
GO
ALTER TABLE [dbo].[HMR_SALT_REPORT] DROP CONSTRAINT [DF_HMR_SALT_REPORT_DB_AUDIT_CREATE_USERID]
GO
ALTER TABLE [dbo].[HMR_SALT_REPORT] DROP CONSTRAINT [DF_HMR_SALT_REPORT_CONCURRENCY_CONTROL_NUMBER]
GO
ALTER TABLE [dbo].[HMR_SALT_REPORT] DROP CONSTRAINT [DF_HMR_SALT_REPORT_SALT_REPORT_ID]
GO
ALTER TABLE [dbo].[HMR_SALT_APPENDIX_HIST] DROP CONSTRAINT [DF__HMR_SALT___EFFEC__29C39D7E]
GO
ALTER TABLE [dbo].[HMR_SALT_APPENDIX_HIST] DROP CONSTRAINT [DF__HMR_SALT___SALT___4E00FDF4]
GO
ALTER TABLE [dbo].[HMR_SALT_APPENDIX] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__522F1F86]
GO
ALTER TABLE [dbo].[HMR_SALT_APPENDIX] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__513AFB4D]
GO
ALTER TABLE [dbo].[HMR_SALT_APPENDIX] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__5046D714]
GO
ALTER TABLE [dbo].[HMR_SALT_APPENDIX] DROP CONSTRAINT [DF__HMR_SALT___DB_AU__4F52B2DB]
GO
ALTER TABLE [dbo].[HMR_SALT_APPENDIX] DROP CONSTRAINT [DF__HMR_SALT___CONCU__4E5E8EA2]
GO
ALTER TABLE [dbo].[HMR_SALT_APPENDIX] DROP CONSTRAINT [DF_HMR_SALT_APPENDIX_APPENDIX_ID]
GO
/****** Object:  Table [dbo].[HMR_SALT_STOCKPILE_HIST]    Script Date: 2024-04-16 11:42:00 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HMR_SALT_STOCKPILE_HIST]') AND type in (N'U'))
DROP TABLE [dbo].[HMR_SALT_STOCKPILE_HIST]
GO
/****** Object:  Table [dbo].[HMR_SALT_STOCKPILE]    Script Date: 2024-04-16 11:42:00 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HMR_SALT_STOCKPILE]') AND type in (N'U'))
DROP TABLE [dbo].[HMR_SALT_STOCKPILE]
GO
/****** Object:  Table [dbo].[HMR_SALT_REPORT_HIST]    Script Date: 2024-04-16 11:42:00 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HMR_SALT_REPORT_HIST]') AND type in (N'U'))
DROP TABLE [dbo].[HMR_SALT_REPORT_HIST]
GO
/****** Object:  Table [dbo].[HMR_SALT_REPORT]    Script Date: 2024-04-16 11:42:00 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HMR_SALT_REPORT]') AND type in (N'U'))
DROP TABLE [dbo].[HMR_SALT_REPORT]
GO
/****** Object:  Table [dbo].[HMR_SALT_APPENDIX_HIST]    Script Date: 2024-04-16 11:42:00 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HMR_SALT_APPENDIX_HIST]') AND type in (N'U'))
DROP TABLE [dbo].[HMR_SALT_APPENDIX_HIST]
GO
/****** Object:  Table [dbo].[HMR_SALT_APPENDIX]    Script Date: 2024-04-16 11:42:00 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HMR_SALT_APPENDIX]') AND type in (N'U'))
DROP TABLE [dbo].[HMR_SALT_APPENDIX]
GO

DROP SEQUENCE [dbo].[HMR_SLT_RPT_ID_SEQ];
GO
DROP SEQUENCE [dbo].[HMR_SLT_STOCKPILE_ID_SEQ];
GO
DROP SEQUENCE [dbo].[HMR_SLT_APPENDIX_ID_SEQ];
GO
DROP SEQUENCE [dbo].[HMR_SALT_STOCKPILE_H_ID_SEQ];
GO
DROP SEQUENCE [dbo].[HMR_SALT_APPENDIX_H_ID_SEQ];
GO
DROP SEQUENCE [dbo].[HMR_SALT_REPORT_H_ID_SEQ];
GO

-- Step 1: Find the IDs of the inserted role and permission
DECLARE @RoleID INT;
DECLARE @PermissionID INT;

SELECT @RoleID = [ROLE_ID] 
FROM [dbo].[HMR_ROLE]
WHERE [NAME] = 'Salt Reporting' AND [DESCRIPTION] = 'Submit and view Submitted Annual Salt Report';

SELECT @PermissionID = [PERMISSION_ID] 
FROM [dbo].[HMR_PERMISSION]
WHERE [NAME] = 'SALT' AND [DESCRIPTION] = 'Salt Reporting';

-- Step 2: Delete from HMR_USER_ROLE using the captured RoleID
DELETE FROM [dbo].[HMR_USER_ROLE]
WHERE [ROLE_ID] = @RoleID;

-- Step 3: Delete from HMR_ROLE_PERMISSION using the captured RoleID and PermissionID
DELETE FROM [dbo].[HMR_ROLE_PERMISSION]
WHERE [ROLE_ID] = @RoleID AND [PERMISSION_ID] = @PermissionID;

-- Step 4: Delete from HMR_ROLE
DELETE FROM [dbo].[HMR_ROLE]
WHERE [ROLE_ID] = @RoleID;

-- Step 5: Delete from HMR_PERMISSION
DELETE FROM [dbo].[HMR_PERMISSION]
WHERE [PERMISSION_ID] = @PermissionID;
