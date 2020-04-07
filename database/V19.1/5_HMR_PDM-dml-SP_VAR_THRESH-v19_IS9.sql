-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-04-06
-- Updates: 
-- 
-- Description:	Create Spatial Threshold levels to be applied against ACTIVITY_CODE values.  These levels will determine the range of spatial variance before a warning (first value in comma separated list) or error (second value in comma separated list) is generated during submission.
-- =============================================


USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO


DELETE FROM [dbo].[HMR_CODE_LOOKUP] WHERE CODE_SET IN ('THRSHLD_SP_VAR_WARN','THRSHLD_SP_VAR_ERROR')

DELETE FROM [dbo].[HMR_CODE_LOOKUP] WHERE CODE_SET IN ('THRSHLD_SP_VAR')

INSERT INTO HMR_CODE_LOOKUP (CODE_SET, CODE_NAME, CODE_VALUE_TEXT, CODE_VALUE_NUM, DISPLAY_ORDER, CODE_VALUE_FORMAT) VALUES ('THRSHLD_SP_VAR', 'LEVEL 1', '50,100',NULL, '1', 'STRING');
GO
INSERT INTO HMR_CODE_LOOKUP (CODE_SET, CODE_NAME, CODE_VALUE_TEXT, CODE_VALUE_NUM, DISPLAY_ORDER, CODE_VALUE_FORMAT) VALUES ('THRSHLD_SP_VAR', 'LEVEL 2', '100,250',NULL, '2', 'STRING');
GO
INSERT INTO HMR_CODE_LOOKUP (CODE_SET, CODE_NAME, CODE_VALUE_TEXT, CODE_VALUE_NUM, DISPLAY_ORDER, CODE_VALUE_FORMAT) VALUES ('THRSHLD_SP_VAR', 'LEVEL 3', '250,500',NULL, '3', 'STRING');
GO
INSERT INTO HMR_CODE_LOOKUP (CODE_SET, CODE_NAME, CODE_VALUE_TEXT, CODE_VALUE_NUM, DISPLAY_ORDER, CODE_VALUE_FORMAT) VALUES ('THRSHLD_SP_VAR', 'LEVEL 4', '500,1000',NULL, '4', 'STRING');
GO

