-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-08-25
-- Updates: 
--	2020-08-25: data management for ticket HMCR-651 to addresss TH-65135, TH-64889  
--    Description:	- Load CONTRACT_TERM PARTY association based on the business area specified users : 
--			BCeID\BCORBACH from ARGO ROAD MAINTENANCE (SOUTH OKANAGAN) INC. for Service Area 21
--			BCeID\SMONTEMURRO from EMIL ANDERSON MAINTENANCE CO. LTD., for Service Area 26 

-- =============================================

--USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
USE HMR_PRD;
GO

/* Load user context variables.  Redeclared further below for subsequent executions */

DECLARE @utcdate DATETIME = (SELECT getutcdate() AS utcdate)
DECLARE @app_guid UNIQUEIDENTIFIER = (SELECT CASE WHEN SUSER_SID() IS NOT NULL THEN SUSER_SID() ELSE (SELECT CONVERT(uniqueidentifier,STUFF(STUFF(STUFF(STUFF('B00D00A0AC0A0D0C00DD00F0D0C00000',9,0,'-'),14,0,'-'),19,0,'-'),24,0,'-'))) END AS  app_guid)
DECLARE @app_user VARCHAR(30) = (SELECT CASE WHEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) IS NOT NULL THEN SUBSTRING(SUSER_NAME(),CHARINDEX('\',SUSER_NAME())+1,LEN(SUSER_NAME())) ELSE CURRENT_USER END AS app_user)
DECLARE @app_user_dir VARCHAR(12) = (SELECT CASE WHEN LEN(SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)))>1 THEN SUBSTRING(SUSER_NAME(),0,CHARINDEX('\',SUSER_NAME(),0)) ELSE 'WIN AUTH' END AS app_user_dir)


UPDATE CNRT_TRM
   SET CNRT_TRM.[PARTY_ID] = prty_cnrt.PARTY_ID
      ,CNRT_TRM.[CONCURRENCY_CONTROL_NUMBER] = CONCURRENCY_CONTROL_NUMBER+1
	  ,CNRT_TRM.[APP_LAST_UPDATE_USERID] = @app_user
      ,CNRT_TRM.[APP_LAST_UPDATE_TIMESTAMP] = @utcdate
      ,CNRT_TRM.[APP_LAST_UPDATE_USER_GUID] = @app_guid
      ,CNRT_TRM.[APP_LAST_UPDATE_USER_DIRECTORY] = @app_user_dir
      ,CNRT_TRM.[DB_AUDIT_LAST_UPDATE_USERID] = @app_user
      ,CNRT_TRM.[DB_AUDIT_LAST_UPDATE_TIMESTAMP] = @utcdate
FROM [dbo].[HMR_CONTRACT_TERM] CNRT_TRM
INNER JOIN (
SELECT PARTY_ID, CONTRACT_NAME FROM (
	SELECT
			 PRTY.PARTY_ID,
			 RANK() OVER (PARTITION BY SRV_ARA.[SERVICE_AREA_NUMBER] 
		  ORDER BY
			 SRV_ARA_USR.APP_LAST_UPDATE_TIMESTAMP DESC) AS Rank,
			 cnrt_trm.CONTRACT_NAME,
			 SYS_USR.USERNAME 
		  FROM
			 [dbo].[HMR_SERVICE_AREA] SRV_ARA 
			 INNER JOIN
				[dbo].[HMR_SERVICE_AREA_USER] SRV_ARA_USR 
				ON SRV_ARA.SERVICE_AREA_NUMBER = SRV_ARA_USR.SERVICE_AREA_NUMBER 
			 INNER JOIN
				[dbo].[HMR_SYSTEM_USER] SYS_USR 
				ON SRV_ARA_USR.SYSTEM_USER_ID = SYS_USR.SYSTEM_USER_ID 
			 INNER JOIN
				[dbo].[HMR_PARTY] PRTY 
				ON SYS_USR.PARTY_ID = PRTY.PARTY_ID 
			 INNER JOIN
				[dbo].[HMR_CONTRACT_TERM] cnrt_trm 
				ON cnrt_trm.SERVICE_AREA_NUMBER = SRV_ARA_USR.SERVICE_AREA_NUMBER 
		  WHERE
			 SYS_USR.USER_TYPE = 'BUSINESS' 
			 AND SYS_USR.BUSINESS_GUID IS NOT NULL 
			 AND PRTY.BUSINESS_GUID IS NOT NULL 
			-- business area specified the user names to associate with each service area
			 AND SYS_USR.USERNAME IN ('BCORBACH', 'SMONTEMURRO') AND cnrt_trm.SERVICE_AREA_NUMBER IN (21, 26) 
			-- only acceptable to overwrite an exiting party relationship without managing the contract term dates if there are no previous data submissions
			AND SRV_ARA.SERVICE_AREA_NUMBER NOT IN (
				SELECT
				   SERVICE_AREA_NUMBER 
				FROM
				   [dbo].[HMR_SUBMISSION_ROW] SUBM_RW 
				   INNER JOIN
					  [dbo].[HMR_SUBMISSION_OBJECT] SUBM_OBJ 
					  ON SUBM_RW.SUBMISSION_OBJECT_ID = SUBM_OBJ.SUBMISSION_OBJECT_ID 
				WHERE
					[SERVICE_AREA_NUMBER] IN (26, 21)
			)
		) PRTY_USR_SRV_ARA 
) prty_cnrt ON CNRT_TRM.CONTRACT_NAME = prty_cnrt.CONTRACT_NAME