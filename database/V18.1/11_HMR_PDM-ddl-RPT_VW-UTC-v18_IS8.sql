/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting                   */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-03-13 02:11                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-03-13
-- Updates: 
--	
-- 
-- Description:	Incremental DML in support of sprint 8.
--  - Revise views to be compatible with MSSQL Server 2012, disgarding AT TIME ZONE syntax
--  - Revised views to CAST non-nullable fields so GeoServer will not require their inclusion on file exports
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


DROP VIEW [dbo].[HMR_WILDLIFE_REPORT_VW]
GO


DROP VIEW [dbo].[HMR_ROCKFALL_REPORT_VW]
GO


DROP VIEW [dbo].[HMR_WORK_REPORT_VW]
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
      ,CAST(wrkrpt.[SERVICE_AREA] AS numeric) AS SERVICE_AREA
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
	  ,CAST(subm_obj.[FILE_NAME] AS varchar) AS FILE_NAME
      ,wrkrpt.ROW_NUM
      ,CAST(subm_stat.[STATUS_CODE] + ' - ' + subm_stat.[DESCRIPTION] AS varchar) AS VALIDATION_STATUS
      ,CAST(wrkrpt.APP_CREATE_TIMESTAMP AS datetime) AS APP_CREATE_TIMESTAMP_UTC
	  ,CAST(wrkrpt.APP_LAST_UPDATE_TIMESTAMP  AS datetime) AS APP_LAST_UPDATE_TIMESTAMP_UTC
  FROM HMR_WORK_REPORT wrkrpt
  INNER JOIN HMR_SUBMISSION_OBJECT subm_obj ON wrkrpt.SUBMISSION_OBJECT_ID = subm_obj.SUBMISSION_OBJECT_ID
  LEFT OUTER JOIN HMR_ACTIVITY_CODE actcode ON wrkrpt.ACTIVITY_NUMBER = actcode.ACTIVITY_NUMBER
  LEFT OUTER JOIN HMR_SUBMISSION_ROW subm_rw ON wrkrpt.ROW_ID = subm_rw.ROW_ID
  LEFT OUTER JOIN HMR_SUBMISSION_STATUS subm_stat ON subm_rw.ROW_STATUS_ID = subm_stat.STATUS_ID
GO


CREATE VIEW [dbo].[HMR_WILDLIFE_REPORT_VW] AS
      SELECT 
	   wldlfrpt.[WILDLIFE_RECORD_ID]
	  ,'WILDLIFE_REPORT' AS REPORT_TYPE
      ,wldlfrpt.[RECORD_TYPE]
      ,CAST(wldlfrpt.[SERVICE_AREA] AS numeric) AS SERVICE_AREA
      ,wldlfrpt.[ACCIDENT_DATE]
      ,wldlfrpt.[TIME_OF_KILL]
      ,wldlfrpt.[LATITUDE]
      ,wldlfrpt.[LONGITUDE]
	  ,CASE WHEN subm_rw.[START_VARIANCE] >= (SELECT TOP (1) Min(CODE_VALUE_NUM) AS THRSHLD_SP_VAR_WARN_LOW FROM  HMR_CODE_LOOKUP  WHERE  CODE_SET IN ( 'THRSHLD_SP_VAR_WARN') AND END_DATE IS NULL) THEN 'Y' 
            ELSE 'N' 
	   END IS_OVER_SP_TOLERANCE
      ,wldlfrpt.[HIGHWAY_UNIQUE]
      ,wldlfrpt.[HIGHWAY_UNIQUE_NAME]
      ,wldlfrpt.[HIGHWAY_UNIQUE_LENGTH]
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
      ,CAST(wldlfrpt.[SUBMISSION_OBJECT_ID] AS numeric) AS SUBMISSION_OBJECT_ID 
	  ,CAST(subm_obj.[FILE_NAME] AS varchar) AS FILE_NAME
	  ,wldlfrpt.[ROW_NUM]
      ,CAST(subm_stat.[STATUS_CODE] + ' - ' + subm_stat.[DESCRIPTION] AS varchar) AS VALIDATION_STATUS
      ,CAST(wldlfrpt.APP_CREATE_TIMESTAMP AS datetime) AS APP_CREATE_TIMESTAMP_UTC
	  ,CAST(wldlfrpt.APP_LAST_UPDATE_TIMESTAMP  AS datetime) AS APP_LAST_UPDATE_TIMESTAMP_UTC
  FROM [dbo].[HMR_WILDLIFE_REPORT] wldlfrpt
  INNER JOIN HMR_SUBMISSION_OBJECT subm_obj ON wldlfrpt.SUBMISSION_OBJECT_ID = subm_obj.SUBMISSION_OBJECT_ID
  LEFT OUTER JOIN HMR_SUBMISSION_ROW subm_rw ON wldlfrpt.ROW_ID = subm_rw.ROW_ID
  LEFT OUTER JOIN HMR_SUBMISSION_STATUS subm_stat ON subm_rw.ROW_STATUS_ID = subm_stat.STATUS_ID
GO


CREATE VIEW [dbo].[HMR_ROCKFALL_REPORT_VW] AS
SELECT 
      rckflrpt.[ROCKFALL_REPORT_ID]
      ,'ROCKFALL_REPORT' AS REPORT_TYPE
      ,CAST(rckflrpt.[RECORD_TYPE] AS varchar) AS RECORD_TYPE
      ,CAST(rckflrpt.[SERVICE_AREA] AS numeric) AS SERVICE_AREA
	  ,rckflrpt.[MCRR_INCIDENT_NUMBER]
      ,rckflrpt.[ESTIMATED_ROCKFALL_DATE]
      ,rckflrpt.[ESTIMATED_ROCKFALL_TIME]
      ,rckflrpt.[START_LATITUDE]
      ,rckflrpt.[START_LONGITUDE]
      ,rckflrpt.[END_LATITUDE]
      ,rckflrpt.[END_LONGITUDE]
      ,rckflrpt.[HIGHWAY_UNIQUE]
      ,rckflrpt.[HIGHWAY_UNIQUE_NAME]
      ,rckflrpt.[HIGHWAY_UNIQUE_LENGTH]
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
	  ,CAST(subm_obj.[FILE_NAME] AS varchar) AS FILE_NAME
      ,rckflrpt.[ROW_NUM]
      ,CAST(subm_stat.[STATUS_CODE] + ' - ' + subm_stat.[DESCRIPTION] AS varchar) AS VALIDATION_STATUS
	  ,CAST(rckflrpt.APP_CREATE_TIMESTAMP AS datetime) AS APP_CREATE_TIMESTAMP_UTC
	  ,CAST(rckflrpt.APP_LAST_UPDATE_TIMESTAMP  AS datetime) AS APP_LAST_UPDATE_TIMESTAMP_UTC
  FROM HMR_ROCKFALL_REPORT rckflrpt
  INNER JOIN HMR_SUBMISSION_OBJECT subm_obj ON rckflrpt.SUBMISSION_OBJECT_ID = subm_obj.SUBMISSION_OBJECT_ID
  LEFT OUTER JOIN HMR_SUBMISSION_ROW subm_rw ON rckflrpt.ROW_ID = subm_rw.ROW_ID
  LEFT OUTER JOIN HMR_SUBMISSION_STATUS subm_stat ON subm_rw.ROW_STATUS_ID = subm_stat.STATUS_ID
  LEFT OUTER JOIN HMR_SYSTEM_USER usr ON rckflrpt.APP_CREATE_USER_GUID = usr.APP_CREATE_USER_GUID
GO

