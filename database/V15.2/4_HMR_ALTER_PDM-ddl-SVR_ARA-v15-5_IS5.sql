/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:                                                          */
/* Author:                                                                */
/* Script type:           Alter database script                           */
/* Created on:            2020-02-11 07:05                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-11
-- Updates: 
--	
-- 
-- Description:	Incremnetal DML in support of sprint 5.
--  - Added Service Area to HMR_ROCKFALL_REPORT
--
--
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

/* ---------------------------------------------------------------------- */
/* Drop triggers                                                          */
/* ---------------------------------------------------------------------- */

GO


DROP TRIGGER [dbo].[HMR_RCKFL_RPT_A_S_IUD_TR]
GO


DROP TRIGGER [dbo].[HMR_RCKFL_RPT_I_S_I_TR]
GO


DROP TRIGGER [dbo].[HMR_RCKFL_RPT_I_S_U_TR]
GO


/* ---------------------------------------------------------------------- */
/* Drop foreign key constraints                                           */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RKFLL_RRT_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RCKFL_RPT_SUBM_OBJ_FK]
GO


/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_ROCKFALL_REPORT"                      */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RCKFL_RPT_PK]
GO


CREATE TABLE [dbo].[HMR_ROCKFALL_REPORT_TMP] (
    [ROCKFALL_REPORT_ID] NUMERIC(9) DEFAULT NEXT VALUE FOR [HMR_RCKF_ID_SEQ] NOT NULL,
    [SUBMISSION_OBJECT_ID] NUMERIC(9) NOT NULL,
    [ROW_NUM] NUMERIC(9),
    [VALIDATION_STATUS_ID] NUMERIC(9),
    [MCRR_INCIDENT_NUMBER] VARCHAR(12),
    [RECORD_TYPE] VARCHAR(1) NOT NULL,
    [SERVICE_AREA] NUMERIC(9),
    [ESTIMATED_ROCKFALL_DATE] DATE,
    [ESTIMATED_ROCKFALL_TIME] TIME,
    [START_LATITUDE] NUMERIC(16,8),
    [START_LONGITUDE] NUMERIC(16,8),
    [END_LATITUDE] NUMERIC(16,8),
    [END_LONGITUDE] NUMERIC(16,8),
    [HIGHWAY_UNIQUE] VARCHAR(16),
    [HIGHWAY_UNIQUE_NAME] VARCHAR(255),
    [LANDMARK] VARCHAR(8),
    [LANDMARK_NAME] VARCHAR(255),
    [START_OFFSET] NUMERIC(7,3),
    [END_OFFSET] NUMERIC(7,3),
    [DIRECTION_FROM_LANDMARK] VARCHAR(1),
    [LOCATION_DESCRIPTION] VARCHAR(4000),
    [DITCH_VOLUME] VARCHAR(30),
    [TRAVELLED_LANES_VOLUME] VARCHAR(30),
    [OTHER_TRAVELLED_LANES_VOLUME] NUMERIC(6,2),
    [OTHER_DITCH_VOLUME] NUMERIC(6,2),
    [HEAVY_PRECIP] VARCHAR(1),
    [FREEZE_THAW] VARCHAR(1),
    [DITCH_SNOW_ICE] VARCHAR(1),
    [VEHICLE_DAMAGE] VARCHAR(1),
    [COMMENTS] VARCHAR(4000),
    [REPORTER_NAME] VARCHAR(150),
    [MC_PHONE_NUMBER] VARCHAR(12),
    [REPORT_DATE] DATE,
    [CONCURRENCY_CONTROL_NUMBER] BIGINT DEFAULT 1 NOT NULL,
    [APP_CREATE_USERID] VARCHAR(30) NOT NULL,
    [APP_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_CREATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_CREATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [APP_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [APP_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_LAST_UPDATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_LAST_UPDATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) DEFAULT user_name() NOT NULL,
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME DEFAULT getutcdate() NOT NULL,
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) DEFAULT user_name() NOT NULL,
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME DEFAULT getutcdate() NOT NULL)
GO


INSERT INTO [dbo].[HMR_ROCKFALL_REPORT_TMP]
    ([ROCKFALL_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[MCRR_INCIDENT_NUMBER],[RECORD_TYPE],[SERVICE_AREA], [ESTIMATED_ROCKFALL_DATE],[ESTIMATED_ROCKFALL_TIME],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[HIGHWAY_UNIQUE],[HIGHWAY_UNIQUE_NAME],[LANDMARK],[LANDMARK_NAME],[START_OFFSET],[END_OFFSET],[DIRECTION_FROM_LANDMARK],[LOCATION_DESCRIPTION],[DITCH_VOLUME],[TRAVELLED_LANES_VOLUME],[OTHER_TRAVELLED_LANES_VOLUME],[OTHER_DITCH_VOLUME],[HEAVY_PRECIP],[FREEZE_THAW],[DITCH_SNOW_ICE],[VEHICLE_DAMAGE],[COMMENTS],[REPORTER_NAME],[MC_PHONE_NUMBER],[REPORT_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    HRR.[ROCKFALL_REPORT_ID],HRR.[SUBMISSION_OBJECT_ID],HRR.[ROW_NUM],HRR.[VALIDATION_STATUS_ID],HRR.[MCRR_INCIDENT_NUMBER],HRR.[RECORD_TYPE],HSO.SERVICE_AREA_NUMBER,HRR.[ESTIMATED_ROCKFALL_DATE],HRR.[ESTIMATED_ROCKFALL_TIME],HRR.[START_LATITUDE],HRR.[START_LONGITUDE],HRR.[END_LATITUDE],HRR.[END_LONGITUDE],HRR.[HIGHWAY_UNIQUE],HRR.[HIGHWAY_UNIQUE_NAME],HRR.[LANDMARK],HRR.[LANDMARK_NAME],HRR.[START_OFFSET],HRR.[END_OFFSET],HRR.[DIRECTION_FROM_LANDMARK],HRR.[LOCATION_DESCRIPTION],HRR.[DITCH_VOLUME],HRR.[TRAVELLED_LANES_VOLUME],HRR.[OTHER_TRAVELLED_LANES_VOLUME],HRR.[OTHER_DITCH_VOLUME],HRR.[HEAVY_PRECIP],HRR.[FREEZE_THAW],HRR.[DITCH_SNOW_ICE],HRR.[VEHICLE_DAMAGE],HRR.[COMMENTS],HRR.[REPORTER_NAME],HRR.[MC_PHONE_NUMBER],HRR.[REPORT_DATE],HRR.[CONCURRENCY_CONTROL_NUMBER],HRR.[APP_CREATE_USERID],HRR.[APP_CREATE_TIMESTAMP],HRR.[APP_CREATE_USER_GUID],HRR.[APP_CREATE_USER_DIRECTORY],HRR.[APP_LAST_UPDATE_USERID],HRR.[APP_LAST_UPDATE_TIMESTAMP],HRR.[APP_LAST_UPDATE_USER_GUID],HRR.[APP_LAST_UPDATE_USER_DIRECTORY],HRR.[DB_AUDIT_CREATE_USERID],HRR.[DB_AUDIT_CREATE_TIMESTAMP],HRR.[DB_AUDIT_LAST_UPDATE_USERID],HRR.[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_ROCKFALL_REPORT] HRR INNER JOIN [dbo].[HMR_SUBMISSION_OBJECT] HSO ON HRR.SUBMISSION_OBJECT_ID = HSO.SUBMISSION_OBJECT_ID
GO


DROP INDEX [dbo].[HMR_ROCKFALL_REPORT].[HMR_RCKFL_RPT_FK_I]
GO


DROP TABLE [dbo].[HMR_ROCKFALL_REPORT]
GO


EXEC sp_rename '[dbo].[HMR_ROCKFALL_REPORT_TMP]', 'HMR_ROCKFALL_REPORT', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] ADD CONSTRAINT [HMR_RCKFL_RPT_PK] 
    PRIMARY KEY CLUSTERED ([ROCKFALL_REPORT_ID])
GO


CREATE NONCLUSTERED INDEX [HMR_RCKFL_RPT_FK_I] ON [dbo].[HMR_ROCKFALL_REPORT] ([SUBMISSION_OBJECT_ID] ASC)
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Submission data regarding rockfall incidents is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for SUBMISSION OBJECT.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'SUBMISSION_OBJECT_ID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Relative row number the record was located within a submission.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'ROW_NUM'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for validation STATUS.  Indicates the overall status of the submitted row of data.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'VALIDATION_STATUS_ID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Rockfall reporting incident number. Unique work report record number from the Contractor maintenance management system.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'MCRR_INCIDENT_NUMBER'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'The Ministry Contract Service Area number in which the incident occured.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'SERVICE_AREA'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Estimated date of occurrence.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'ESTIMATED_ROCKFALL_DATE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Estimated time of occurrence using the 24-hour clock', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'ESTIMATED_ROCKFALL_TIME'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'The M (northing) portion of the activity start coordinate. Specified as a latitude in decimal degrees with six decimal places of precision. Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'START_LATITUDE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'The X (easting) portion of the activity start coordinate. Specified as a longitude in decimal degrees with six decimal places of precision. Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'START_LONGITUDE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'The M (northing) portion of the activity end coordinate. Specified as a latitude in decimal degrees with six decimal places of precision. Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum. For point activity if this field is not provided it can be defaulted to same as START LATITUDE', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'END_LATITUDE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'The X (easting) portion of the activity end coordinate. Specified as a longitude in decimal degrees with six decimal places of precision. Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum. For point activity if this field is not provided it can be defaulted to same as START LONGITUDE.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'END_LONGITUDE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'This identifies the section of road on which the activity occurred.  Road or Highway number sourced from a road network data product (RFI as of  2019) This is a value in the in the format: [Service Area]-[area manager area]-[subarea]-[highway number]', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'HIGHWAY_UNIQUE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Road or Highway name sourced from a road network data product (RFI as of Dec 2019)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'HIGHWAY_UNIQUE_NAME'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'This field needed for location reference: Landmarks provided should be those listed in the CHRIS HRP report for each Highway or road within the Service Area', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'LANDMARK'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'This field is needed for linear referencing for location specific reports.  Offset from beginning of segment.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'START_OFFSET'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'This field is needed for linear referencing for location specific reports. If the work is less than 30 m, this field is not mandatory Offset from beginning of segment', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'END_OFFSET'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Direction of travel from Landmark to START_OFFSET', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'DIRECTION_FROM_LANDMARK'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Text field for comments and/or notes pertinent to the specified activity.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'LOCATION_DESCRIPTION'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Range of estimated volume of material in ditch (m cubed). if volume exceeds 5.0 m3 and report value in the other volume field.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'DITCH_VOLUME'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Range of estimated volume of material on the road (m cubed).', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'TRAVELLED_LANES_VOLUME'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Heavy precipitation conditions present at rockfall site. Enter “Y” or leave blank.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'HEAVY_PRECIP'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Freezing/thawing conditions present at rockfall site. Enter “Y” or leave blank.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'FREEZE_THAW'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Ditch snow or ice conditions present at rockfall site. Enter “Y” or leave blank.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'DITCH_SNOW_ICE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Vehicle damage present at rockfall site. Enter “Y” or leave blank.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'VEHICLE_DAMAGE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Comments of occurrence', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'COMMENTS'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Name of person reporting occurrence', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'REPORTER_NAME'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Phone number of person reporting', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'MC_PHONE_NUMBER'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date reported', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'REPORT_DATE'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'CONCURRENCY_CONTROL_NUMBER'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'APP_CREATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of record creation', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'APP_CREATE_TIMESTAMP'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'APP_CREATE_USER_GUID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'APP_CREATE_USER_DIRECTORY'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'APP_LAST_UPDATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of last record update', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'APP_LAST_UPDATE_TIMESTAMP'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'APP_LAST_UPDATE_USER_GUID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'APP_LAST_UPDATE_USER_DIRECTORY'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'DB_AUDIT_CREATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record created in the database', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'DB_AUDIT_CREATE_TIMESTAMP'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_USERID'
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record was last updated in the database.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_TIMESTAMP'
GO


/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_ROCKFALL_REPORT_HIST"                 */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT_HIST] DROP CONSTRAINT [HMR_RCKFL_H_PK]
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT_HIST] DROP CONSTRAINT [HMR_RCKFL_H_UK]
GO


CREATE TABLE [dbo].[HMR_ROCKFALL_REPORT_HIST_TMP] (
    [ROCKFALL_REPORT_HIST_ID] BIGINT DEFAULT NEXT VALUE FOR [HMR_ROCKFALL_REPORT_H_ID_SEQ] NOT NULL,
    [EFFECTIVE_DATE_HIST] DATETIME DEFAULT getutcdate() NOT NULL,
    [END_DATE_HIST] DATETIME,
    [ROCKFALL_REPORT_ID] NUMERIC(18) NOT NULL,
    [SUBMISSION_OBJECT_ID] NUMERIC(18) NOT NULL,
    [ROW_NUM] NUMERIC(18),
    [VALIDATION_STATUS_ID] NUMERIC(18),
    [MCRR_INCIDENT_NUMBER] VARCHAR(12),
    [RECORD_TYPE] VARCHAR(1),
    [SERVICE_AREA] NUMERIC(9),
    [ESTIMATED_ROCKFALL_DATE] DATE,
    [ESTIMATED_ROCKFALL_TIME] TIME,
    [START_LATITUDE] NUMERIC(18),
    [START_LONGITUDE] NUMERIC(18),
    [END_LATITUDE] NUMERIC(18),
    [END_LONGITUDE] NUMERIC(18),
    [HIGHWAY_UNIQUE] VARCHAR(16),
    [HIGHWAY_UNIQUE_NAME] VARCHAR(255),
    [LANDMARK] VARCHAR(8),
    [LANDMARK_NAME] VARCHAR(255),
    [START_OFFSET] NUMERIC(18),
    [END_OFFSET] NUMERIC(18),
    [DIRECTION_FROM_LANDMARK] VARCHAR(1),
    [LOCATION_DESCRIPTION] VARCHAR(4000),
    [DITCH_VOLUME] VARCHAR(30),
    [TRAVELLED_LANES_VOLUME] VARCHAR(30),
    [OTHER_TRAVELLED_LANES_VOLUME] NUMERIC(18),
    [OTHER_DITCH_VOLUME] NUMERIC(18),
    [HEAVY_PRECIP] VARCHAR(1),
    [FREEZE_THAW] VARCHAR(1),
    [DITCH_SNOW_ICE] VARCHAR(1),
    [VEHICLE_DAMAGE] VARCHAR(1),
    [COMMENTS] VARCHAR(4000),
    [REPORTER_NAME] VARCHAR(150),
    [MC_PHONE_NUMBER] VARCHAR(12),
    [REPORT_DATE] DATE,
    [CONCURRENCY_CONTROL_NUMBER] BIGINT NOT NULL,
    [APP_CREATE_USERID] VARCHAR(30) NOT NULL,
    [APP_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_CREATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_CREATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [APP_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [APP_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_LAST_UPDATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_LAST_UPDATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) NOT NULL,
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL)
GO


INSERT INTO [dbo].[HMR_ROCKFALL_REPORT_HIST_TMP]
    ([ROCKFALL_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[ROCKFALL_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[MCRR_INCIDENT_NUMBER],[RECORD_TYPE],[SERVICE_AREA],[ESTIMATED_ROCKFALL_DATE],[ESTIMATED_ROCKFALL_TIME],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[HIGHWAY_UNIQUE],[HIGHWAY_UNIQUE_NAME],[LANDMARK],[LANDMARK_NAME],[START_OFFSET],[END_OFFSET],[DIRECTION_FROM_LANDMARK],[LOCATION_DESCRIPTION],[DITCH_VOLUME],[TRAVELLED_LANES_VOLUME],[OTHER_TRAVELLED_LANES_VOLUME],[OTHER_DITCH_VOLUME],[HEAVY_PRECIP],[FREEZE_THAW],[DITCH_SNOW_ICE],[VEHICLE_DAMAGE],[COMMENTS],[REPORTER_NAME],[MC_PHONE_NUMBER],[REPORT_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    HRRH.[ROCKFALL_REPORT_HIST_ID],HRRH.[EFFECTIVE_DATE_HIST],HRRH.[END_DATE_HIST],HRRH.[ROCKFALL_REPORT_ID],HRRH.[SUBMISSION_OBJECT_ID],HRRH.[ROW_NUM],HRRH.[VALIDATION_STATUS_ID],HRRH.[MCRR_INCIDENT_NUMBER],HRRH.[RECORD_TYPE],HRR.SERVICE_AREA,HRRH.[ESTIMATED_ROCKFALL_DATE],HRRH.[ESTIMATED_ROCKFALL_TIME],HRRH.[START_LATITUDE],HRRH.[START_LONGITUDE],HRRH.[END_LATITUDE],HRRH.[END_LONGITUDE],HRRH.[HIGHWAY_UNIQUE],HRRH.[HIGHWAY_UNIQUE_NAME],HRRH.[LANDMARK],HRRH.[LANDMARK_NAME],HRRH.[START_OFFSET],HRRH.[END_OFFSET],HRRH.[DIRECTION_FROM_LANDMARK],HRRH.[LOCATION_DESCRIPTION],HRRH.[DITCH_VOLUME],HRRH.[TRAVELLED_LANES_VOLUME],HRRH.[OTHER_TRAVELLED_LANES_VOLUME],HRRH.[OTHER_DITCH_VOLUME],HRRH.[HEAVY_PRECIP],HRRH.[FREEZE_THAW],HRRH.[DITCH_SNOW_ICE],HRRH.[VEHICLE_DAMAGE],HRRH.[COMMENTS],HRRH.[REPORTER_NAME],HRRH.[MC_PHONE_NUMBER],HRRH.[REPORT_DATE],HRRH.[CONCURRENCY_CONTROL_NUMBER],HRRH.[APP_CREATE_USERID],HRRH.[APP_CREATE_TIMESTAMP],HRRH.[APP_CREATE_USER_GUID],HRRH.[APP_CREATE_USER_DIRECTORY],HRRH.[APP_LAST_UPDATE_USERID],HRRH.[APP_LAST_UPDATE_TIMESTAMP],HRRH.[APP_LAST_UPDATE_USER_GUID],HRRH.[APP_LAST_UPDATE_USER_DIRECTORY],HRRH.[DB_AUDIT_CREATE_USERID],HRRH.[DB_AUDIT_CREATE_TIMESTAMP],HRRH.[DB_AUDIT_LAST_UPDATE_USERID],HRRH.[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_ROCKFALL_REPORT_HIST] HRRH INNER JOIN [dbo].[HMR_ROCKFALL_REPORT] HRR ON HRRH.ROCKFALL_REPORT_ID = HRR.ROCKFALL_REPORT_ID
GO


DROP TABLE [dbo].[HMR_ROCKFALL_REPORT_HIST]
GO


EXEC sp_rename '[dbo].[HMR_ROCKFALL_REPORT_HIST_TMP]', 'HMR_ROCKFALL_REPORT_HIST', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT_HIST] ADD CONSTRAINT [HMR_RCKFL_H_PK] 
    PRIMARY KEY CLUSTERED ([ROCKFALL_REPORT_HIST_ID])
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT_HIST] ADD CONSTRAINT [HMR_RCKFL_H_UK] 
    UNIQUE ([ROCKFALL_REPORT_HIST_ID], [END_DATE_HIST])
GO


/* ---------------------------------------------------------------------- */
/* Add foreign key constraints                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] ADD CONSTRAINT [HMR_RCKFL_RPT_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] ADD CONSTRAINT [HMR_RKFLL_RRT_SUBM_STAT_FK] 
    FOREIGN KEY ([VALIDATION_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] ADD CONSTRAINT [HMR_RCKFL_RPT_HMR_SRV_ARA_FK] 
    FOREIGN KEY ([SERVICE_AREA]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


/* ---------------------------------------------------------------------- */
/* Repair/add triggers                                                    */
/* ---------------------------------------------------------------------- */

GO


CREATE TRIGGER [dbo].[HMR_RCKFL_RPT_A_S_IUD_TR] ON HMR_ROCKFALL_REPORT FOR INSERT, UPDATE, DELETE AS
SET NOCOUNT ON
BEGIN TRY
DECLARE @curr_date datetime;
SET @curr_date = getutcdate();
  IF NOT EXISTS(SELECT * FROM inserted) AND NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- historical
  IF EXISTS(SELECT * FROM deleted)
    update HMR_ROCKFALL_REPORT_HIST set END_DATE_HIST = @curr_date where ROCKFALL_REPORT_ID in (select ROCKFALL_REPORT_ID from deleted) and END_DATE_HIST is null;

  IF EXISTS(SELECT * FROM inserted)
    insert into HMR_ROCKFALL_REPORT_HIST ([ROCKFALL_REPORT_ID], [SUBMISSION_OBJECT_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [MCRR_INCIDENT_NUMBER], [RECORD_TYPE], [SERVICE_AREA], [ESTIMATED_ROCKFALL_DATE], [ESTIMATED_ROCKFALL_TIME], [START_LATITUDE], [START_LONGITUDE], [END_LATITUDE], [END_LONGITUDE], [HIGHWAY_UNIQUE], [HIGHWAY_UNIQUE_NAME], [LANDMARK], [LANDMARK_NAME], [START_OFFSET], [END_OFFSET], [DIRECTION_FROM_LANDMARK], [LOCATION_DESCRIPTION], [DITCH_VOLUME], [TRAVELLED_LANES_VOLUME], [OTHER_TRAVELLED_LANES_VOLUME], [OTHER_DITCH_VOLUME], [HEAVY_PRECIP], [FREEZE_THAW], [DITCH_SNOW_ICE], [VEHICLE_DAMAGE], [COMMENTS], [REPORTER_NAME], [MC_PHONE_NUMBER], [REPORT_DATE], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], ROCKFALL_REPORT_HIST_ID, END_DATE_HIST, EFFECTIVE_DATE_HIST)
      select [ROCKFALL_REPORT_ID], [SUBMISSION_OBJECT_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [MCRR_INCIDENT_NUMBER], [RECORD_TYPE], [SERVICE_AREA], [ESTIMATED_ROCKFALL_DATE], [ESTIMATED_ROCKFALL_TIME], [START_LATITUDE], [START_LONGITUDE], [END_LATITUDE], [END_LONGITUDE], [HIGHWAY_UNIQUE], [HIGHWAY_UNIQUE_NAME], [LANDMARK], [LANDMARK_NAME], [START_OFFSET], [END_OFFSET], [DIRECTION_FROM_LANDMARK], [LOCATION_DESCRIPTION], [DITCH_VOLUME], [TRAVELLED_LANES_VOLUME], [OTHER_TRAVELLED_LANES_VOLUME], [OTHER_DITCH_VOLUME], [HEAVY_PRECIP], [FREEZE_THAW], [DITCH_SNOW_ICE], [VEHICLE_DAMAGE], [COMMENTS], [REPORTER_NAME], [MC_PHONE_NUMBER], [REPORT_DATE], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_ROCKFALL_REPORT_H_ID_SEQ]) as [ROCKFALL_REPORT_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_RCKFL_RPT_I_S_I_TR] ON HMR_ROCKFALL_REPORT INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted)
    RETURN;


  insert into HMR_ROCKFALL_REPORT ("ROCKFALL_REPORT_ID",
      "SUBMISSION_OBJECT_ID",
      "VALIDATION_STATUS_ID",
      "MCRR_INCIDENT_NUMBER",
      "RECORD_TYPE", 
      "SERVICE_AREA",
      "ESTIMATED_ROCKFALL_DATE",
      "ESTIMATED_ROCKFALL_TIME",
      "START_LATITUDE",
      "START_LONGITUDE",
      "END_LATITUDE",
      "END_LONGITUDE",
      "HIGHWAY_UNIQUE",
      "HIGHWAY_UNIQUE_NAME",
      "LANDMARK",
      "LANDMARK_NAME",
      "START_OFFSET",
      "END_OFFSET",
      "DIRECTION_FROM_LANDMARK",
      "LOCATION_DESCRIPTION",
      "DITCH_VOLUME",
      "TRAVELLED_LANES_VOLUME",
      "OTHER_TRAVELLED_LANES_VOLUME",
      "OTHER_DITCH_VOLUME",
      "HEAVY_PRECIP",
      "FREEZE_THAW",
      "DITCH_SNOW_ICE",
      "VEHICLE_DAMAGE",
      "COMMENTS",
      "REPORTER_NAME",
      "MC_PHONE_NUMBER",
      "REPORT_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "ROCKFALL_REPORT_ID",
      "SUBMISSION_OBJECT_ID",
      "VALIDATION_STATUS_ID",
      "MCRR_INCIDENT_NUMBER", 
      "RECORD_TYPE", 
      "SERVICE_AREA",
      "ESTIMATED_ROCKFALL_DATE",
      "ESTIMATED_ROCKFALL_TIME",
      "START_LATITUDE",
      "START_LONGITUDE",
      "END_LATITUDE",
      "END_LONGITUDE",
      "HIGHWAY_UNIQUE",
      "HIGHWAY_UNIQUE_NAME",
      "LANDMARK",
      "LANDMARK_NAME",
      "START_OFFSET",
      "END_OFFSET",
      "DIRECTION_FROM_LANDMARK",
      "LOCATION_DESCRIPTION",
      "DITCH_VOLUME",
      "TRAVELLED_LANES_VOLUME",
      "OTHER_TRAVELLED_LANES_VOLUME",
      "OTHER_DITCH_VOLUME",
      "HEAVY_PRECIP",
      "FREEZE_THAW",
      "DITCH_SNOW_ICE",
      "VEHICLE_DAMAGE",
      "COMMENTS",
      "REPORTER_NAME",
      "MC_PHONE_NUMBER",
      "REPORT_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_RCKFL_RPT_I_S_U_TR] ON HMR_ROCKFALL_REPORT INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.ROCKFALL_REPORT_ID = deleted.ROCKFALL_REPORT_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_ROCKFALL_REPORT
    set "ROCKFALL_REPORT_ID" = inserted."ROCKFALL_REPORT_ID",
      "SUBMISSION_OBJECT_ID" = inserted."SUBMISSION_OBJECT_ID",
      "VALIDATION_STATUS_ID" = inserted."VALIDATION_STATUS_ID",
      "MCRR_INCIDENT_NUMBER" = inserted."MCRR_INCIDENT_NUMBER",
      "RECORD_TYPE" = inserted."RECORD_TYPE",  
      "SERVICE_AREA" = inserted."SERVICE_AREA",
      "ESTIMATED_ROCKFALL_DATE" = inserted."ESTIMATED_ROCKFALL_DATE",
      "ESTIMATED_ROCKFALL_TIME" = inserted."ESTIMATED_ROCKFALL_TIME",
      "START_LATITUDE" = inserted."START_LATITUDE",
      "START_LONGITUDE" = inserted."START_LONGITUDE",
      "END_LATITUDE" = inserted."END_LATITUDE",
      "END_LONGITUDE" = inserted."END_LONGITUDE",
      "HIGHWAY_UNIQUE" = inserted."HIGHWAY_UNIQUE",
      "HIGHWAY_UNIQUE_NAME" = inserted."HIGHWAY_UNIQUE_NAME",
      "LANDMARK" = inserted."LANDMARK",
      "LANDMARK_NAME" = inserted."LANDMARK_NAME",
      "START_OFFSET" = inserted."START_OFFSET",
      "END_OFFSET" = inserted."END_OFFSET",
      "DIRECTION_FROM_LANDMARK" = inserted."DIRECTION_FROM_LANDMARK",
      "LOCATION_DESCRIPTION" = inserted."LOCATION_DESCRIPTION",
      "DITCH_VOLUME" = inserted."DITCH_VOLUME",
      "TRAVELLED_LANES_VOLUME" = inserted."TRAVELLED_LANES_VOLUME",
      "OTHER_TRAVELLED_LANES_VOLUME" = inserted."OTHER_TRAVELLED_LANES_VOLUME",
      "OTHER_DITCH_VOLUME" = inserted."OTHER_DITCH_VOLUME",
      "HEAVY_PRECIP" = inserted."HEAVY_PRECIP",
      "FREEZE_THAW" = inserted."FREEZE_THAW",
      "DITCH_SNOW_ICE" = inserted."DITCH_SNOW_ICE",
      "VEHICLE_DAMAGE" = inserted."VEHICLE_DAMAGE",
      "COMMENTS" = inserted."COMMENTS",
      "REPORTER_NAME" = inserted."REPORTER_NAME",
      "MC_PHONE_NUMBER" = inserted."MC_PHONE_NUMBER",
      "REPORT_DATE" = inserted."REPORT_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_ROCKFALL_REPORT
    inner join inserted
    on (HMR_ROCKFALL_REPORT.ROCKFALL_REPORT_ID = inserted.ROCKFALL_REPORT_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO

