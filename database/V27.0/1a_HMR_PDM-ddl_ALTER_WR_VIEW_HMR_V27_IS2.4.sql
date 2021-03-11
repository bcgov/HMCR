
-- Drop view dbo.HMR_WORK_REPORT_VW
PRINT N'Drop view dbo.HMR_WORK_REPORT_VW'
GO
DROP VIEW [dbo].[HMR_WORK_REPORT_VW]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO


-- Create view dbo.HMR_WORK_REPORT_VW
PRINT N'Create view dbo.HMR_WORK_REPORT_VW'
GO
/* ---------------------------------------------------------------------- */
/* Add views                                                              */
/* ---------------------------------------------------------------------- */

/* ======================================================================= */
/* Add views                                                               */
/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
/* Author        Date         Comment                                      */
/* ------------  -----------  -------------------------------------------- */
/* Update        2020-Apr-23  Added spatial warning thresholds             */
/* Doug Filteau  2020-Dec-15  Extended spatial warning thresholds          */
/* ======================================================================= */

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
	  ,CAST(subm_obj.APP_CREATE_TIMESTAMP  AS datetime) AS SUBMISSION_DATE
	  ,wrkrpt.RECORD_VERSION_NUMBER
	  ,REPLACE((SELECT distinct FIELDMESSAGE FROM OPENJSON(subm_rw.WARNING_DETAIL,'$.fieldMessages') 
			WITH (FIELD nvarchar(1000) '$.field', FIELDMESSAGE nvarchar(1000) '$.messages[0]') WHERE charindex('Minimum / Maximum Value Validation', FIELD) >= 1 ), ',', '/') AS 'MIN_MAX_VALUE_VALID_WARNING'
	  ,REPLACE((SELECT distinct FIELDMESSAGE FROM OPENJSON(subm_rw.WARNING_DETAIL,'$.fieldMessages') 
			WITH (FIELD nvarchar(1000) '$.field', FIELDMESSAGE nvarchar(1000) '$.messages[0]') WHERE charindex('Data Precision Validation', FIELD) >= 1 ), ',', '/') AS 'DATA_PRECISION_VALID_WARNING'
	  ,REPLACE((SELECT distinct FIELDMESSAGE FROM OPENJSON(subm_rw.WARNING_DETAIL,'$.fieldMessages') 
			WITH (FIELD nvarchar(1000) '$.field', FIELDMESSAGE nvarchar(1000) '$.messages[0]') WHERE charindex('Reporting Frequency Validation', FIELD) >= 1 ), ',', '/') AS 'REPORTING_FREQ_VALID_WARNING'
	  ,REPLACE((SELECT distinct FIELDMESSAGE FROM OPENJSON(subm_rw.WARNING_DETAIL,'$.fieldMessages') 
			WITH (FIELD nvarchar(1000) '$.field', FIELDMESSAGE nvarchar(1000) '$.messages[0]') WHERE FIELD = 'Surface Type Validation'), ',', '/') AS 'SURFACE_TYPE_VALID_WARNING'
	  ,REPLACE((SELECT distinct FIELDMESSAGE FROM OPENJSON(subm_rw.WARNING_DETAIL,'$.fieldMessages') 
			WITH (FIELD nvarchar(1000) '$.field', FIELDMESSAGE nvarchar(1000) '$.messages[0]') WHERE FIELD = 'Road Class Validation'), ',', '/') AS 'ROAD_CLASS_VALID_WARNING'
    ,REPLACE((SELECT distinct FIELDMESSAGE FROM OPENJSON(subm_rw.WARNING_DETAIL,'$.fieldMessages') 
			WITH (FIELD nvarchar(1000) '$.field', FIELDMESSAGE nvarchar(1000) '$.messages[0]') WHERE FIELD = 'Road Length Validation'), ',', '/') AS 'ROAD_LENGTH_VALID_WARNING'
	  ,REPLACE((SELECT distinct FIELDMESSAGE FROM OPENJSON(subm_rw.WARNING_DETAIL,'$.fieldMessages') 
			WITH (FIELD nvarchar(1000) '$.field', FIELDMESSAGE nvarchar(1000) '$.messages[0]') WHERE FIELD = 'Structure Validation'), ',', '/') AS 'STRUCTURE_VALIDATION'
	  ,REPLACE((SELECT distinct FIELDMESSAGE FROM OPENJSON(subm_rw.WARNING_DETAIL,'$.fieldMessages') 
			WITH (FIELD nvarchar(1000) '$.field', FIELDMESSAGE nvarchar(1000) '$.messages[0]') WHERE FIELD = 'Site Validation'), ',', '/') AS 'SITE_VALIDATION'
  FROM HMR_WORK_REPORT wrkrpt
  INNER JOIN HMR_SUBMISSION_OBJECT subm_obj ON wrkrpt.SUBMISSION_OBJECT_ID = subm_obj.SUBMISSION_OBJECT_ID
  LEFT OUTER JOIN HMR_ACTIVITY_CODE actcode ON wrkrpt.ACTIVITY_NUMBER = actcode.ACTIVITY_NUMBER
  LEFT OUTER JOIN HMR_SUBMISSION_ROW subm_rw ON wrkrpt.ROW_ID = subm_rw.ROW_ID
  LEFT OUTER JOIN HMR_SUBMISSION_STATUS subm_stat ON subm_rw.ROW_STATUS_ID = subm_stat.STATUS_ID
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO