
-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-24
-- Updates: 
--	
-- 
-- Description:	Incremnetal DML in support of sprint 7.
--  - Added spatial varience threshold values
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* ---------------------------------------------------------------------- */
/* DML Generated - Spatial Varience Threshold Values                      */
/* ---------------------------------------------------------------------- */

INSERT INTO HMR_CODE_LOOKUP (CODE_SET, CODE_NAME, CODE_VALUE_TEXT, CODE_VALUE_NUM, DISPLAY_ORDER, CODE_VALUE_FORMAT) VALUES ('THRSHLD_SP_VAR_WARN', 'Geo Variance Warning', '','500', NULL, 'NUMBER');
GO
INSERT INTO HMR_CODE_LOOKUP (CODE_SET, CODE_NAME, CODE_VALUE_TEXT, CODE_VALUE_NUM, DISPLAY_ORDER, CODE_VALUE_FORMAT) VALUES ('THRSHLD_SP_VAR_ERROR', 'Geo Variance Error', '','1000', NULL, 'NUMBER');
GO

