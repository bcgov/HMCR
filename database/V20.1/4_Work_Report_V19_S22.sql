/* ---------------------------------------------------------------------- */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          1_Work_Report_V19_S22.sql						  */
/* Project name:          Highway Maintenance Reporting                   */
/* Author:                Ayodeji Kuponiyi                                */
/* Script type:           Updates on Views	                              */
/* Created on:            2020-04-23 13:05                                */
/* ---------------------------------------------------------------------- */

USE HMR_DEV;

/* Update 23/04/2020

i) Added spatial warning thresholds

*/
GO

ALTER VIEW [dbo].[HMR_WORK_REPORT_VW] AS
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
	  ,subm_rw.WARNING_SP_THRESHOLD
	  ,wrkrpt.WORK_LENGTH
	  ,CASE
			WHEN ISNULL(subm_rw.START_VARIANCE,0) > ISNULL(subm_rw.WARNING_SP_THRESHOLD,0)
				OR ISNULL(subm_rw.END_VARIANCE,0) > ISNULL(subm_rw.WARNING_SP_THRESHOLD,0)
			THEN 'Y' ELSE 'N'
	   END IS_OVER_SP_THRESHOLD
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