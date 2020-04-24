/* ---------------------------------------------------------------------- */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          3_WildLife_Report_V19_S22.sql					  */
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

ALTER VIEW [dbo].[HMR_WILDLIFE_REPORT_VW] AS
      SELECT
	   wldlfrpt.[WILDLIFE_RECORD_ID]
	  ,'WILDLIFE_REPORT' AS REPORT_TYPE
      ,wldlfrpt.[RECORD_TYPE]
      ,CAST(wldlfrpt.[SERVICE_AREA] AS numeric) AS SERVICE_AREA
      ,wldlfrpt.[ACCIDENT_DATE]
      ,wldlfrpt.[TIME_OF_KILL]
      ,wldlfrpt.[LATITUDE]
      ,wldlfrpt.[LONGITUDE]
	  ,subm_rw.[START_VARIANCE]	AS SPATIAL_VARIANCE
	  ,subm_rw.[WARNING_SP_THRESHOLD]
	  ,CASE
			WHEN ISNULL(subm_rw.START_VARIANCE,0) > ISNULL(subm_rw.WARNING_SP_THRESHOLD,0)
				OR ISNULL(subm_rw.END_VARIANCE,0) > ISNULL(subm_rw.WARNING_SP_THRESHOLD,0)
			THEN 'Y' ELSE 'N'
	   END IS_OVER_SP_THRESHOLD
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