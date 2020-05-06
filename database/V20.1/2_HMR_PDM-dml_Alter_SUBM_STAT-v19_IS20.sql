/* Target DBMS:           MS SQL Server 2017                              */
/* Project name:          Highway Maintenance Reporting                   */
/* Author:                Ayodeji Kuponiyi                                */
/* Script type:           DML database script                             */
/* Created on:            2020-04-21 14:51                                */
/* ---------------------------------------------------------------------- */

USE HMR_DEV;
GO
/* Updates

1.	Updates all existing rows – with STATUS_CODES = FE, DS, DP, DE, 'VS','RR', 'DR','RE','RS' with their respective STAGE values
2.	Creates 3 new rows with STATUS_CODES = S3, S4, UE
3.	Updates description for DS, DP AND DE
4.	If needed, check for idempotence

*/



IF EXISTS 
	(SELECT * FROM [dbo].[HMR_SUBMISSION_STATUS] WHERE STATUS_CODE IN ('FR', 'FE', 'DS','DP','DE', 'VS','RR', 'DR','RE','RS'))
	UPDATE [dbo].[HMR_SUBMISSION_STATUS] SET STAGE = 0,CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1 WHERE STATUS_CODE = 'FR';
	UPDATE [dbo].[HMR_SUBMISSION_STATUS] SET STAGE = 5,CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1 WHERE STATUS_CODE = 'VS';
	UPDATE [dbo].[HMR_SUBMISSION_STATUS] SET DESCRIPTION = 'Stage 1 - File Error',STAGE = 1,CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1 WHERE STATUS_CODE = 'FE';
	UPDATE [dbo].[HMR_SUBMISSION_STATUS] SET DESCRIPTION = 'Stage 1 - Duplicate Submission',STAGE = 1,CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1 WHERE STATUS_CODE = 'DS';
	UPDATE [dbo].[HMR_SUBMISSION_STATUS] SET DESCRIPTION = 'Stage 2 - Data validation In-progress',STAGE = 2,CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1 WHERE STATUS_CODE = 'DP';
	UPDATE [dbo].[HMR_SUBMISSION_STATUS] SET DESCRIPTION = 'Stage 2 - Basic error',STAGE = 2,CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1 WHERE STATUS_CODE = 'DE';
	UPDATE [dbo].[HMR_SUBMISSION_STATUS] SET STAGE = 0,CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1 WHERE STATUS_CODE = 'RR';
	UPDATE [dbo].[HMR_SUBMISSION_STATUS] SET STAGE = 0,CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1 WHERE STATUS_CODE = 'DR';
	UPDATE [dbo].[HMR_SUBMISSION_STATUS] SET STAGE = 0,CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1 WHERE STATUS_CODE = 'RE';
	UPDATE [dbo].[HMR_SUBMISSION_STATUS] SET STAGE = 0,CONCURRENCY_CONTROL_NUMBER=CONCURRENCY_CONTROL_NUMBER+1 WHERE STATUS_CODE = 'RS';

GO


/* Load user context variables. */
DECLARE @utcdate DATETIME = (SELECT getutcdate() AS utcdate)
DECLARE @app_guid UNIQUEIDENTIFIER = (SELECT CASE WHEN SUSER_SID() IS NOT NULL THEN SUSER_SID() ELSE (SELECT CONVERT(uniqueidentifier,STUFF(STUFF(STUFF(STUFF('B00D00A0AC0A0D0C00DD00F0D0C00000',9,0,'-'),14,0,'-'),19,0,'-'),24,0,'-'))) END AS  app_guid)
DECLARE @app_user VARCHAR(30) = (SELECT CASE WHEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) IS NOT NULL THEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) ELSE CURRENT_USER END AS app_user)
DECLARE @app_user_dir VARCHAR(12) = (SELECT CASE WHEN LEN(SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)))>1 THEN SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)) ELSE 'WIN AUTH' END AS app_user_dir)

IF EXISTS
	
	(SELECT * FROM [dbo].[HMR_SUBMISSION_STATUS] WHERE STATUS_CODE IN ('S3', 'S4', 'UE'))
	BEGIN
		DELETE FROM HMR_SUBMISSION_STATUS WHERE STATUS_CODE IN ('S3', 'S4', 'UE');
		INSERT INTO HMR_SUBMISSION_STATUS (STATUS_CODE, DESCRIPTION, LONG_DESCRIPTION, STATUS_TYPE,STAGE, APP_CREATE_USERID, APP_CREATE_TIMESTAMP, APP_CREATE_USER_GUID, APP_CREATE_USER_DIRECTORY, APP_LAST_UPDATE_USERID, APP_LAST_UPDATE_TIMESTAMP, APP_LAST_UPDATE_USER_GUID, APP_LAST_UPDATE_USER_DIRECTORY) VALUES ('S3', 'Stage 3 - Confliction Error', 'Indicates a submitted file has been evaluated and conflicting values have been submitted. The file will need to be uploaded again after making required corrections.', 'F',3, @app_user,@utcdate,@app_guid,@app_user_dir,@app_user,@utcdate,@app_guid,@app_user_dir);
		INSERT INTO HMR_SUBMISSION_STATUS (STATUS_CODE, DESCRIPTION, LONG_DESCRIPTION, STATUS_TYPE,STAGE, APP_CREATE_USERID, APP_CREATE_TIMESTAMP, APP_CREATE_USER_GUID, APP_CREATE_USER_DIRECTORY, APP_LAST_UPDATE_USERID, APP_LAST_UPDATE_TIMESTAMP, APP_LAST_UPDATE_USER_GUID, APP_LAST_UPDATE_USER_DIRECTORY) VALUES ('S4', 'Stage 4 - Location Error', 'Indicates a submitted file has been evaluated and start or end coordinates are not within acceptable distance from known ministry infrastructure. The file will need to be uploaded again after making required corrections.', 'F',4, @app_user,@utcdate,@app_guid,@app_user_dir,@app_user,@utcdate,@app_guid,@app_user_dir);
		INSERT INTO HMR_SUBMISSION_STATUS (STATUS_CODE, DESCRIPTION, LONG_DESCRIPTION, STATUS_TYPE,STAGE, APP_CREATE_USERID, APP_CREATE_TIMESTAMP, APP_CREATE_USER_GUID, APP_CREATE_USER_DIRECTORY, APP_LAST_UPDATE_USERID, APP_LAST_UPDATE_TIMESTAMP, APP_LAST_UPDATE_USER_GUID, APP_LAST_UPDATE_USER_DIRECTORY) VALUES ('UE', 'Unexpected Error', 'Indicates the system has encountered an unexpected error and a submitted file cannot be fully processed. Please notify the Ministry and provide the attempted file for diagnosis.', 'F',-1, @app_user,@utcdate,@app_guid,@app_user_dir,@app_user,@utcdate,@app_guid,@app_user_dir);
	END
ELSE

	BEGIN
		INSERT INTO HMR_SUBMISSION_STATUS (STATUS_CODE, DESCRIPTION, LONG_DESCRIPTION, STATUS_TYPE,STAGE, APP_CREATE_USERID, APP_CREATE_TIMESTAMP, APP_CREATE_USER_GUID, APP_CREATE_USER_DIRECTORY, APP_LAST_UPDATE_USERID, APP_LAST_UPDATE_TIMESTAMP, APP_LAST_UPDATE_USER_GUID, APP_LAST_UPDATE_USER_DIRECTORY) VALUES ('S3', 'Stage 3 - Confliction Error', 'Indicates a submitted file has been evaluated and conflicting values have been submitted. The file will need to be uploaded again after making required corrections.', 'F',3, @app_user,@utcdate,@app_guid,@app_user_dir,@app_user,@utcdate,@app_guid,@app_user_dir);
		INSERT INTO HMR_SUBMISSION_STATUS (STATUS_CODE, DESCRIPTION, LONG_DESCRIPTION, STATUS_TYPE,STAGE, APP_CREATE_USERID, APP_CREATE_TIMESTAMP, APP_CREATE_USER_GUID, APP_CREATE_USER_DIRECTORY, APP_LAST_UPDATE_USERID, APP_LAST_UPDATE_TIMESTAMP, APP_LAST_UPDATE_USER_GUID, APP_LAST_UPDATE_USER_DIRECTORY) VALUES ('S4', 'Stage 4 - Location Error', 'Indicates a submitted file has been evaluated and start or end coordinates are not within acceptable distance from known ministry infrastructure. The file will need to be uploaded again after making required corrections.', 'F',4, @app_user,@utcdate,@app_guid,@app_user_dir,@app_user,@utcdate,@app_guid,@app_user_dir);
		INSERT INTO HMR_SUBMISSION_STATUS (STATUS_CODE, DESCRIPTION, LONG_DESCRIPTION, STATUS_TYPE,STAGE, APP_CREATE_USERID, APP_CREATE_TIMESTAMP, APP_CREATE_USER_GUID, APP_CREATE_USER_DIRECTORY, APP_LAST_UPDATE_USERID, APP_LAST_UPDATE_TIMESTAMP, APP_LAST_UPDATE_USER_GUID, APP_LAST_UPDATE_USER_DIRECTORY) VALUES ('UE', 'Unexpected Error', 'Indicates the system has encountered an unexpected error and a submitted file cannot be fully processed. Please notify the Ministry and provide the attempted file for diagnosis.', 'F',-1, @app_user,@utcdate,@app_guid,@app_user_dir,@app_user,@utcdate,@app_guid,@app_user_dir);
	END	
GO