/*
Author:	Ayodeji Kuponiyi
Date:	April 30, 2020
Project:	HMR
*/

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* Update Initial STATUS Entries with new version */
/* Updates 30/04/2020
1. change case for first word letters to  CAPS in Description for status code DE 
*/

IF EXISTS 
	(SELECT * FROM [dbo].[HMR_SUBMISSION_STATUS] WHERE STATUS_CODE IN ('DE'))
	UPDATE [dbo].[HMR_SUBMISSION_STATUS] SET DESCRIPTION = 'Stage 2 - Basic Error',STAGE = 2,CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1 WHERE STATUS_CODE = 'DE';

GO