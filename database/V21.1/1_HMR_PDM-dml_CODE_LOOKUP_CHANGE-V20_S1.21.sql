/* ---------------------------------------------------------------------- */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project name:          Highway Maintenance Reporting                   */
/* Author:                Ayodeji Kuponiyi                                */
/* Script type:           DML					                          */
/* Created on:            2020-05-11		                              */
/* ---------------------------------------------------------------------- */

USE HMR_DEV;

/* Updates
	JIRA Ticket: https://jira.th.gov.bc.ca/browse/HMCR-633

	i) Remove major event type as an option from  HMR_CODE_LOOKUP.

*/


IF EXISTS 
	(SELECT * FROM [dbo].[HMR_CODE_LOOKUP] WHERE CODE_SET = 'WRK_RPT_MAINT_TYPE' AND CODE_NAME = 'Major Event')
	DELETE [dbo].[HMR_CODE_LOOKUP] WHERE CODE_SET = 'WRK_RPT_MAINT_TYPE' AND CODE_NAME = 'Major Event';

GO