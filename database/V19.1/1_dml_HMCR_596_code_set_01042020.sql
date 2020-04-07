/* ---------------------------------------------------------------------- */
/* Script generated with: Scripted						                  */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:                                              			  */
/* Project name:          Highway Maintenance Reporting                   */
/* Author:                Ayodeji Kuponiyi                                */
/* Script type:           DML											  */
/* Created on:            2020-04-01 13:30:00                             */
/* ---------------------------------------------------------------------- */

USE HMR_DEV;
GO

/*
Major updates:
	i)		Deleted rows containing code_set 'THRSHLD_SP_VAR_ERROR' and 'THRSHLD_SP_VAR_WARN'
	ii)		created four levels of new code_set 'THRSHLD_SP_VAR'
*/



GO

DELETE FROM [hmr_dev].[dbo].[HMR_CODE_LOOKUP] WHERE CODE_SET = 'THRSHLD_SP_VAR_WARN' 
OR CODE_SET = 'THRSHLD_SP_VAR_ERROR';

GO

DELETE FROM [hmr_dev].[dbo].[HMR_CODE_LOOKUP] WHERE CODE_SET = 'THRSHLD_SP_VAR';


GO

INSERT INTO HMR_CODE_LOOKUP (CODE_SET, CODE_NAME, CODE_VALUE_TEXT, CODE_VALUE_FORMAT, DISPLAY_ORDER, END_DATE) VALUES ('THRSHLD_SP_VAR', 'Level 1','50, 100', NULL,NULL,NULL);
INSERT INTO HMR_CODE_LOOKUP (CODE_SET, CODE_NAME, CODE_VALUE_TEXT, CODE_VALUE_FORMAT, DISPLAY_ORDER, END_DATE) VALUES ('THRSHLD_SP_VAR', 'Level 2','100, 250', NULL,NULL,NULL);
INSERT INTO HMR_CODE_LOOKUP (CODE_SET, CODE_NAME, CODE_VALUE_TEXT, CODE_VALUE_FORMAT, DISPLAY_ORDER, END_DATE) VALUES ('THRSHLD_SP_VAR', 'Level 3','250, 500', NULL,NULL,NULL);
INSERT INTO HMR_CODE_LOOKUP (CODE_SET, CODE_NAME, CODE_VALUE_TEXT, CODE_VALUE_FORMAT, DISPLAY_ORDER, END_DATE) VALUES ('THRSHLD_SP_VAR', 'Level 4','500, 1000', NULL,NULL,NULL);

GO