/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting - v18             */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-03-09 11:15                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-03-09
-- Updates: 
--	
-- 
-- Description:	Incremental DML in support of sprint 8.
--  - Revise views to begin with primary key (GeoServer requirement).  
--  - Revise views to convert UTC APP Audit dates to Pacific Standard Time, for ease of reporting
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* ---------------------------------------------------------------------- */
/* Drop views                                                             */
/* ---------------------------------------------------------------------- */

GO


DROP VIEW [dbo].[HMR_WORK_REPORT_VW]
GO


DROP VIEW [dbo].[HMR_WILDLIFE_REPORT_VW]
GO


DROP VIEW [dbo].[HMR_ROCKFALL_REPORT_VW]
GO


/* ---------------------------------------------------------------------- */
/* Repair/add views                                                       */
/* ---------------------------------------------------------------------- */

GO


CREATE VIEW [dbo].[HMR_WORK_REPORT_VW] AS
SELECT
      wrkrpt.WORK_REPORT_ID
      ,'WORK_REPORT' AS REPORT_TYPE
	  ,wrkrpt.RECORD_TYPE
      ,srv_ara.SERVICE_AREA_NUMBER
      ,wrkrpt.RECORD_NUMBER
      ,wrkrpt.TASK_NUMBER
      ,wrkrpt.ACTIVITY_NUMBER
	  ,actcode.ACTIVITY_NAME
      ,wrkrpt.START_DATE
      ,wrkrpt.END_DATE
      ,wrkrpt.ACCOMPLISHMENT
      ,wrkrpt.UNIT_OF_MEASURE
      ,wrkrpt.POSTED_DATE
      ,wrkrpt.HIGHWAY_UNIQUE
	  ,wrkrpt.HIGHWAY_UNIQUE_NAME
	  ,wrkrpt.HIGHWAY_UNIQUE_LENGTH
      ,wrkrpt.LANDMARK
      ,wrkrpt.START_OFFSET
      ,wrkrpt.END_OFFSET
      ,wrkrpt.START_LATITUDE
      ,wrkrpt.START_LONGITUDE
	  ,subm_rw.START_VARIANCE
      ,wrkrpt.END_LATITUDE
      ,wrkrpt.END_LONGITUDE
	  ,subm_rw.END_VARIANCE
	  ,wrkrpt.WORK_LENGTH
	  ,CASE WHEN subm_rw.START_VARIANCE >= (SELECT TOP (1) Min(CODE_VALUE_NUM) AS THRSHLD_SP_VAR_WARN_LOW FROM  HMR_CODE_LOOKUP  WHERE  CODE_SET IN ( 'THRSHLD_SP_VAR_WARN') AND END_DATE IS NULL) THEN 'Y' 
            WHEN subm_rw.END_VARIANCE >= (SELECT TOP (1) Min(CODE_VALUE_NUM) AS THRSHLD_SP_VAR_WARN_LOW FROM  HMR_CODE_LOOKUP  WHERE  CODE_SET IN ( 'THRSHLD_SP_VAR_WARN') AND END_DATE IS NULL) THEN 'Y'
            ELSE 'N' 
	   END IS_OVER_SP_TOLERANCE
      ,wrkrpt.STRUCTURE_NUMBER
      ,wrkrpt.SITE_NUMBER
      ,wrkrpt.VALUE_OF_WORK
      ,wrkrpt.COMMENTS
      ,wrkrpt.GEOMETRY
	  ,wrkrpt.SUBMISSION_OBJECT_ID
	  ,subm_obj.FILE_NAME AS SUBMISSION_FILE_NAME
      ,wrkrpt.ROW_NUM
      ,subm_stat.STATUS_CODE + ' - ' + subm_stat.DESCRIPTION AS VALIDATION_STATUS
      ,CONVERT(datetime, SWITCHOFFSET(wrkrpt.APP_CREATE_TIMESTAMP, DATEPART(TZOFFSET, wrkrpt.APP_CREATE_TIMESTAMP AT TIME ZONE 'Pacific Standard Time'))) AS APP_CREATE_TIMESTAMP_PST
	  ,CONVERT(datetime, SWITCHOFFSET(wrkrpt.APP_LAST_UPDATE_TIMESTAMP, DATEPART(TZOFFSET, wrkrpt.APP_LAST_UPDATE_TIMESTAMP AT TIME ZONE 'Pacific Standard Time'))) AS APP_LAST_UPDATE_TIMESTAMP_PST
  FROM HMR_WORK_REPORT wrkrpt
  INNER JOIN HMR_SUBMISSION_OBJECT subm_obj ON wrkrpt.SUBMISSION_OBJECT_ID = subm_obj.SUBMISSION_OBJECT_ID
  LEFT OUTER JOIN HMR_ACTIVITY_CODE actcode ON wrkrpt.ACTIVITY_NUMBER = actcode.ACTIVITY_NUMBER
  LEFT OUTER JOIN HMR_SUBMISSION_ROW subm_rw ON wrkrpt.ROW_ID = subm_rw.ROW_ID
  LEFT OUTER JOIN HMR_SERVICE_AREA srv_ara ON wrkrpt.[SERVICE_AREA] = srv_ara.SERVICE_AREA_NUMBER
  LEFT OUTER JOIN HMR_SUBMISSION_STATUS subm_stat ON subm_rw.ROW_STATUS_ID = subm_stat.STATUS_ID
GO


CREATE VIEW [dbo].[HMR_WILDLIFE_REPORT_VW] AS
SELECT 
	   wldlfrpt.[WILDLIFE_RECORD_ID]
	  ,'WILDLIFE_REPORT' AS REPORT_TYPE
      ,wldlfrpt.[RECORD_TYPE]
      ,wldlfrpt.[SERVICE_AREA]
      ,wldlfrpt.[ACCIDENT_DATE]
      ,wldlfrpt.[TIME_OF_KILL]
      ,wldlfrpt.[LATITUDE]
      ,wldlfrpt.[LONGITUDE]
	  ,CASE WHEN subm_rw.[START_VARIANCE] >= (SELECT TOP (1) Min(CODE_VALUE_NUM) AS THRSHLD_SP_VAR_WARN_LOW FROM  HMR_CODE_LOOKUP  WHERE  CODE_SET IN ( 'THRSHLD_SP_VAR_WARN') AND END_DATE IS NULL) THEN 'Y' 
            ELSE 'N' 
	   END IS_OVER_SP_TOLERANCE
      ,wldlfrpt.[HIGHWAY_UNIQUE]
      ,wldlfrpt.[LANDMARK]
      ,wldlfrpt.[OFFSET]
      ,wldlfrpt.[NEAREST_TOWN]
      ,wldlfrpt.[WILDLIFE_SIGN]
      ,wldlfrpt.[QUANTITY]
      ,wldlfrpt.[SPECIES]
      ,wldlfrpt.[SEX]
      ,wldlfrpt.[AGE]
      ,wldlfrpt.[COMMENT]
      ,wldlfrpt.[GEOMETRY]
      ,wldlfrpt.[SUBMISSION_OBJECT_ID]
	  ,subm_obj.[FILE_NAME]
	  ,wldlfrpt.[ROW_NUM]
      ,subm_stat.[STATUS_CODE] + ' - ' + subm_stat.[DESCRIPTION] AS VALIDATION_STATUS
      ,CONVERT(datetime, SWITCHOFFSET(wldlfrpt.APP_CREATE_TIMESTAMP, DATEPART(TZOFFSET, wldlfrpt.APP_CREATE_TIMESTAMP AT TIME ZONE 'Pacific Standard Time'))) AS APP_CREATE_TIMESTAMP_PST
	  ,CONVERT(datetime, SWITCHOFFSET(wldlfrpt.APP_LAST_UPDATE_TIMESTAMP, DATEPART(TZOFFSET, wldlfrpt.APP_LAST_UPDATE_TIMESTAMP AT TIME ZONE 'Pacific Standard Time'))) AS APP_LAST_UPDATE_TIMESTAMP_PST
  FROM [dbo].[HMR_WILDLIFE_REPORT] wldlfrpt
  INNER JOIN HMR_SUBMISSION_OBJECT subm_obj ON wldlfrpt.SUBMISSION_OBJECT_ID = subm_obj.SUBMISSION_OBJECT_ID
  LEFT OUTER JOIN HMR_SUBMISSION_ROW subm_rw ON wldlfrpt.ROW_ID = subm_rw.ROW_ID
  LEFT OUTER JOIN HMR_SERVICE_AREA srv_ara ON wldlfrpt.[SERVICE_AREA] = srv_ara.SERVICE_AREA_NUMBER
  LEFT OUTER JOIN HMR_SUBMISSION_STATUS subm_stat ON subm_rw.ROW_STATUS_ID = subm_stat.STATUS_ID
GO


CREATE VIEW [dbo].[HMR_ROCKFALL_REPORT_VW] AS
  SELECT 
      rckflrpt.[ROCKFALL_REPORT_ID]
      ,'ROCKFALL_REPORT' AS REPORT_TYPE
      ,rckflrpt.[RECORD_TYPE]
      ,rckflrpt.[SERVICE_AREA]
	  ,rckflrpt.[MCRR_INCIDENT_NUMBER]
      ,rckflrpt.[ESTIMATED_ROCKFALL_DATE]
      ,rckflrpt.[ESTIMATED_ROCKFALL_TIME]
      ,rckflrpt.[START_LATITUDE]
      ,rckflrpt.[START_LONGITUDE]
      ,rckflrpt.[END_LATITUDE]
      ,rckflrpt.[END_LONGITUDE]
      ,rckflrpt.[HIGHWAY_UNIQUE]
      ,rckflrpt.[HIGHWAY_UNIQUE_NAME]
      ,rckflrpt.[LANDMARK]
      ,rckflrpt.[LANDMARK_NAME]
      ,rckflrpt.[START_OFFSET]
      ,rckflrpt.[END_OFFSET]
      ,rckflrpt.[DIRECTION_FROM_LANDMARK]
      ,rckflrpt.[LOCATION_DESCRIPTION]
      ,rckflrpt.[TRAVELLED_LANES_VOLUME]
      ,rckflrpt.[OTHER_TRAVELLED_LANES_VOLUME]
      ,rckflrpt.[DITCH_VOLUME]
      ,rckflrpt.[OTHER_DITCH_VOLUME]
      ,rckflrpt.[HEAVY_PRECIP]
      ,rckflrpt.[FREEZE_THAW]
      ,rckflrpt.[DITCH_SNOW_ICE]
      ,rckflrpt.[VEHICLE_DAMAGE]
      ,rckflrpt.[COMMENTS]
      ,rckflrpt.[REPORTER_NAME]
      ,rckflrpt.[MC_PHONE_NUMBER]
	  ,usr.[BUSINESS_LEGAL_NAME] AS MC_NAME
      ,rckflrpt.[REPORT_DATE]
      ,rckflrpt.[GEOMETRY]
      ,rckflrpt.SUBMISSION_OBJECT_ID
	  ,subm_obj.[FILE_NAME] AS SUBMISSION_FILE_NAME
      ,rckflrpt.[ROW_NUM]
      ,subm_stat.[STATUS_CODE] + ' - ' + subm_stat.[DESCRIPTION] AS VALIDATION_STATUS
	  ,CONVERT(datetime, SWITCHOFFSET(rckflrpt.APP_CREATE_TIMESTAMP, DATEPART(TZOFFSET, rckflrpt.APP_CREATE_TIMESTAMP AT TIME ZONE 'Pacific Standard Time'))) AS APP_CREATE_TIMESTAMP_PST
	  ,CONVERT(datetime, SWITCHOFFSET(rckflrpt.APP_LAST_UPDATE_TIMESTAMP, DATEPART(TZOFFSET, rckflrpt.APP_LAST_UPDATE_TIMESTAMP AT TIME ZONE 'Pacific Standard Time'))) AS APP_LAST_UPDATE_TIMESTAMP_PST
  FROM HMR_ROCKFALL_REPORT rckflrpt
  INNER JOIN HMR_SUBMISSION_OBJECT subm_obj ON rckflrpt.SUBMISSION_OBJECT_ID = subm_obj.SUBMISSION_OBJECT_ID
  LEFT OUTER JOIN HMR_SUBMISSION_ROW subm_rw ON rckflrpt.ROW_ID = subm_rw.ROW_ID
  LEFT OUTER JOIN HMR_SERVICE_AREA srv_ara ON rckflrpt.SERVICE_AREA = srv_ara.SERVICE_AREA_NUMBER
  LEFT OUTER JOIN HMR_SUBMISSION_STATUS subm_stat ON subm_rw.ROW_STATUS_ID = subm_stat.STATUS_ID
  LEFT OUTER JOIN HMR_SYSTEM_USER usr ON rckflrpt.APP_CREATE_USER_GUID = usr.APP_CREATE_USER_GUID
GO

