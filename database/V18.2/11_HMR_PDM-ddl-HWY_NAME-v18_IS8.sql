/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting - v18             */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-03-11 09:56                                */
/* ---------------------------------------------------------------------- */


-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-03-10
-- Updates: 
--	
-- 
-- Description:	Incremental DML in support of sprint 8.
--  - Addition of spatially derived attributes HIGHWAY_UNIQUE_NAME, HIGHWAY_UNIQUE_LENGTH to rockfall and wildlife staging tables
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


DROP TRIGGER [dbo].[HMR_SUBM_RW_I_S_I_TR]
GO


DROP TRIGGER [dbo].[HMR_SUBM_RW_I_S_U_TR]
GO


DROP TRIGGER [dbo].[HMR_WLDLF_RPT_A_S_IUD_TR]
GO


DROP TRIGGER [dbo].[HMR_WLDLF_RPT_I_S_I_TR]
GO


DROP TRIGGER [dbo].[HMR_WLDLF_RPT_I_S_U_TR]
GO


/* ---------------------------------------------------------------------- */
/* Drop views                                                             */
/* ---------------------------------------------------------------------- */

GO


DROP VIEW [dbo].[HMR_WILDLIFE_REPORT_VW]
GO


DROP VIEW [dbo].[HMR_ROCKFALL_REPORT_VW]
GO


/* ---------------------------------------------------------------------- */
/* Drop foreign key constraints                                           */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RCKF_RRT_SUBM_RW_FK]
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RCKFL_RPT_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RKFLL_RRT_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RCKFL_RPT_HMR_SRV_ARA_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] DROP CONSTRAINT [HMR_SUBM_RW_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] DROP CONSTRAINT [HMR_SUBM_RW_HMR_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RRT_SUBM_RW_FK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RPT_HMR_SRV_ARA_FK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RPT_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RRT_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_RW_FK]
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
    [ROW_ID] NUMERIC(9) NOT NULL,
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
    [HIGHWAY_UNIQUE_LENGTH] NUMERIC(25,20),
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
    [REPORTER_NAME] VARCHAR(1024),
    [MC_PHONE_NUMBER] VARCHAR(30),
    [REPORT_DATE] DATE,
    [GEOMETRY] GEOMETRY,
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
    ([ROCKFALL_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[MCRR_INCIDENT_NUMBER],[RECORD_TYPE],[SERVICE_AREA],[ESTIMATED_ROCKFALL_DATE],[ESTIMATED_ROCKFALL_TIME],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[HIGHWAY_UNIQUE],[HIGHWAY_UNIQUE_NAME],[LANDMARK],[LANDMARK_NAME],[START_OFFSET],[END_OFFSET],[DIRECTION_FROM_LANDMARK],[LOCATION_DESCRIPTION],[DITCH_VOLUME],[TRAVELLED_LANES_VOLUME],[OTHER_TRAVELLED_LANES_VOLUME],[OTHER_DITCH_VOLUME],[HEAVY_PRECIP],[FREEZE_THAW],[DITCH_SNOW_ICE],[VEHICLE_DAMAGE],[COMMENTS],[REPORTER_NAME],[MC_PHONE_NUMBER],[REPORT_DATE],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [ROCKFALL_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[MCRR_INCIDENT_NUMBER],[RECORD_TYPE],[SERVICE_AREA],[ESTIMATED_ROCKFALL_DATE],[ESTIMATED_ROCKFALL_TIME],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[HIGHWAY_UNIQUE],[HIGHWAY_UNIQUE_NAME],[LANDMARK],[LANDMARK_NAME],[START_OFFSET],[END_OFFSET],[DIRECTION_FROM_LANDMARK],[LOCATION_DESCRIPTION],[DITCH_VOLUME],[TRAVELLED_LANES_VOLUME],[OTHER_TRAVELLED_LANES_VOLUME],[OTHER_DITCH_VOLUME],[HEAVY_PRECIP],[FREEZE_THAW],[DITCH_SNOW_ICE],[VEHICLE_DAMAGE],[COMMENTS],[REPORTER_NAME],[MC_PHONE_NUMBER],[REPORT_DATE],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_ROCKFALL_REPORT]
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


CREATE NONCLUSTERED INDEX [HMR_RCKFL_RPT_FK_I] ON [dbo].[HMR_ROCKFALL_REPORT] ([SUBMISSION_OBJECT_ID] ASC,[ROW_ID] ASC)
GO

-- MODIFIED:  Reinstate table metadata

EXECUTE sp_addextendedproperty N'MS_Description', N'Submission data regarding rockfall incidents is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'A system generated unique identifier.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'ROCKFALL_REPORT_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for SUBMISSION OBJECT.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'SUBMISSION_OBJECT_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for originating SUBMISSION ROW.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'ROW_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Relative row number the record was located within a submission.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'ROW_NUM'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for validation STATUS.  Indicates the overall status of the submitted row of data.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'VALIDATION_STATUS_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Rockfall reporting incident number. Unique work report record number from the Contractor maintenance management system.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'MCRR_INCIDENT_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Alpha identifier for a Rockfall report submission.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'RECORD_TYPE'
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

EXECUTE sp_addextendedproperty N'MS_Description', N'Road or Highway description sourced from a road network data product (RFI as of Dec 2019)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'HIGHWAY_UNIQUE_NAME'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Driven length in KM of the HIGHWAY_UNIQUE segment at the time of data submission.  ', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'HIGHWAY_UNIQUE_LENGTH'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'This field needed for location reference: Landmarks provided should be those listed in the CHRIS HRP report for each Highway or road within the Service Area', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'LANDMARK'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Highway reference point (HRP) landmark.  This reference name reflects a valid landmark in the infrastructure asset management system (currenlty CHRIS as of 2019)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'LANDMARK_NAME'
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

EXECUTE sp_addextendedproperty N'MS_Description', N'Travelled lanes volume total when the estimated volume in traveled lanes exceeds 5.0 m3.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'OTHER_TRAVELLED_LANES_VOLUME'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Ditch volume total when the estimated volume in the ditch exceeds 5.0 m3.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'OTHER_DITCH_VOLUME'
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

EXECUTE sp_addextendedproperty N'MS_Description', N'Spatial geometry where the event occured, as conformed to the road network.   ', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'GEOMETRY'
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
    [ROW_ID] NUMERIC(9) NOT NULL,
    [ROW_NUM] NUMERIC(18),
    [VALIDATION_STATUS_ID] NUMERIC(18),
    [MCRR_INCIDENT_NUMBER] VARCHAR(12),
    [RECORD_TYPE] VARCHAR(1),
    [SERVICE_AREA] NUMERIC(9),
    [ESTIMATED_ROCKFALL_DATE] DATE,
    [ESTIMATED_ROCKFALL_TIME] TIME,
    [START_LATITUDE] NUMERIC(16,8),
    [START_LONGITUDE] NUMERIC(16,8),
    [END_LATITUDE] NUMERIC(16,8),
    [END_LONGITUDE] NUMERIC(16,8),
    [HIGHWAY_UNIQUE] VARCHAR(16),
    [HIGHWAY_UNIQUE_NAME] VARCHAR(255),
    [HIGHWAY_UNIQUE_LENGTH] NUMERIC(25,20),
    [LANDMARK] VARCHAR(8),
    [LANDMARK_NAME] VARCHAR(255),
    [START_OFFSET] NUMERIC(7,3),
    [END_OFFSET] NUMERIC(7,3),
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
    [REPORTER_NAME] VARCHAR(1024),
    [MC_PHONE_NUMBER] VARCHAR(30),
    [REPORT_DATE] DATE,
    [GEOMETRY] GEOMETRY,
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
    ([ROCKFALL_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[ROCKFALL_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[MCRR_INCIDENT_NUMBER],[RECORD_TYPE],[SERVICE_AREA],[ESTIMATED_ROCKFALL_DATE],[ESTIMATED_ROCKFALL_TIME],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[HIGHWAY_UNIQUE],[HIGHWAY_UNIQUE_NAME],[LANDMARK],[LANDMARK_NAME],[START_OFFSET],[END_OFFSET],[DIRECTION_FROM_LANDMARK],[LOCATION_DESCRIPTION],[DITCH_VOLUME],[TRAVELLED_LANES_VOLUME],[OTHER_TRAVELLED_LANES_VOLUME],[OTHER_DITCH_VOLUME],[HEAVY_PRECIP],[FREEZE_THAW],[DITCH_SNOW_ICE],[VEHICLE_DAMAGE],[COMMENTS],[REPORTER_NAME],[MC_PHONE_NUMBER],[REPORT_DATE],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [ROCKFALL_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[ROCKFALL_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[MCRR_INCIDENT_NUMBER],[RECORD_TYPE],[SERVICE_AREA],[ESTIMATED_ROCKFALL_DATE],[ESTIMATED_ROCKFALL_TIME],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[HIGHWAY_UNIQUE],[HIGHWAY_UNIQUE_NAME],[LANDMARK],[LANDMARK_NAME],[START_OFFSET],[END_OFFSET],[DIRECTION_FROM_LANDMARK],[LOCATION_DESCRIPTION],[DITCH_VOLUME],[TRAVELLED_LANES_VOLUME],[OTHER_TRAVELLED_LANES_VOLUME],[OTHER_DITCH_VOLUME],[HEAVY_PRECIP],[FREEZE_THAW],[DITCH_SNOW_ICE],[VEHICLE_DAMAGE],[COMMENTS],[REPORTER_NAME],[MC_PHONE_NUMBER],[REPORT_DATE],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_ROCKFALL_REPORT_HIST]
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
/* Drop and recreate table "dbo.HMR_WILDLIFE_REPORT"                      */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RRT_PK]
GO


CREATE TABLE [dbo].[HMR_WILDLIFE_REPORT_TMP] (
    [WILDLIFE_RECORD_ID] NUMERIC(9) DEFAULT NEXT VALUE FOR [HMR_WLDLF_ID_SEQ] NOT NULL,
    [SUBMISSION_OBJECT_ID] NUMERIC(9) NOT NULL,
    [ROW_ID] NUMERIC(9) NOT NULL,
    [ROW_NUM] NUMERIC(9),
    [VALIDATION_STATUS_ID] NUMERIC(9),
    [RECORD_TYPE] VARCHAR(1),
    [SERVICE_AREA] NUMERIC(9) NOT NULL,
    [ACCIDENT_DATE] DATETIME,
    [TIME_OF_KILL] VARCHAR(1),
    [LATITUDE] NUMERIC(16,8),
    [LONGITUDE] NUMERIC(16,8),
    [HIGHWAY_UNIQUE] VARCHAR(16),
    [HIGHWAY_UNIQUE_NAME] VARCHAR(40),
    [HIGHWAY_UNIQUE_LENGTH] NUMERIC(25,20),
    [LANDMARK] VARCHAR(8),
    [OFFSET] NUMERIC(7,3),
    [NEAREST_TOWN] VARCHAR(150),
    [WILDLIFE_SIGN] VARCHAR(1),
    [QUANTITY] NUMERIC(4),
    [SPECIES] NUMERIC(2),
    [SEX] VARCHAR(1),
    [AGE] VARCHAR(1),
    [COMMENT] VARCHAR(1024),
    [GEOMETRY] GEOMETRY,
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


INSERT INTO [dbo].[HMR_WILDLIFE_REPORT_TMP]
    ([WILDLIFE_RECORD_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[ACCIDENT_DATE],[TIME_OF_KILL],[LATITUDE],[LONGITUDE],[HIGHWAY_UNIQUE],[LANDMARK],[OFFSET],[NEAREST_TOWN],[WILDLIFE_SIGN],[QUANTITY],[SPECIES],[SEX],[AGE],[COMMENT],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [WILDLIFE_RECORD_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[ACCIDENT_DATE],[TIME_OF_KILL],[LATITUDE],[LONGITUDE],[HIGHWAY_UNIQUE],[LANDMARK],[OFFSET],[NEAREST_TOWN],[WILDLIFE_SIGN],[QUANTITY],[SPECIES],[SEX],[AGE],[COMMENT],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_WILDLIFE_REPORT]
GO


DROP INDEX [dbo].[HMR_WILDLIFE_REPORT].[HMR_WLDLF_RPT_SUBM_FK_I]
GO


DROP INDEX [dbo].[HMR_WILDLIFE_REPORT].[WLDLF_RPT_CNT_ARA_FK_I]
GO


DROP TABLE [dbo].[HMR_WILDLIFE_REPORT]
GO


EXEC sp_rename '[dbo].[HMR_WILDLIFE_REPORT_TMP]', 'HMR_WILDLIFE_REPORT', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RRT_PK] 
    PRIMARY KEY CLUSTERED ([WILDLIFE_RECORD_ID])
GO


CREATE NONCLUSTERED INDEX [HMR_WLDLF_RPT_SUBM_FK_I] ON [dbo].[HMR_WILDLIFE_REPORT] ([SUBMISSION_OBJECT_ID] ASC,[ROW_ID] ASC)
GO


CREATE NONCLUSTERED INDEX [WLDLF_RPT_CNT_ARA_FK_I] ON [dbo].[HMR_WILDLIFE_REPORT] ([SERVICE_AREA] ASC)
GO

-- MODIFIED:  Reinstate table metadata

EXECUTE sp_addextendedproperty N'MS_Description', N'Submission data regarding wildlife incidents is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for a record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'WILDLIFE_RECORD_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for SUBMISSION OBJECT.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'SUBMISSION_OBJECT_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for originating SUBMISSION ROW', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'ROW_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Relative row number the record was located within a submission.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'ROW_NUM'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for VALIDATION STATUS.  Indicates the overall status of the submitted row of data.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'VALIDATION_STATUS_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Identifies the type of record.  WARS = W / Allowed Values: W', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'RECORD_TYPE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'The Ministry Contract Service Area number in which the incident occured.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'SERVICE_AREA'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date of accident. ', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'ACCIDENT_DATE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'General  light conditions at time the incident occured. (eg: 1=Dawn, 2=Dusk)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'TIME_OF_KILL'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'The Y (northing) portion of the accident coordinate. Coordinate is to be reported using the WGS84 datum.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'LATITUDE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'The X (easting)  portion of the accident coordinate. Coordinate is to be reported using the WGS84 datum.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'LONGITUDE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'This identifies the section of road on which the incident occurred. This is a value in the in the format: [Service Area]-[area manager area]-[subarea]-[highway number] This reference number reflects a valid reference in the road network (currenltyRFI within  CHRIS as of 2019)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'HIGHWAY_UNIQUE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Road or Highway description sourced from a road network data product (RFI as of Dec 2019)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'HIGHWAY_UNIQUE_NAME'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Driven length in KM of the HIGHWAY_UNIQUE segment at the time of data submission.  ', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'HIGHWAY_UNIQUE_LENGTH'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Highway reference point (HRP) landmark.  This reference number reflects a valid landmark in the asset management system (currenlty CHRIS as of 2019)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'LANDMARK'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'This field is needed for linear referencing for location specific reports.  Offset from beginning of segment.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'OFFSET'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Name of nearest town to wildlife accident', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'NEAREST_TOWN'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Is Wildlife sign within 100m (Y, N or Unknown)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'WILDLIFE_SIGN'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Number of animals injured or killed', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'QUANTITY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for animal species. (eg: 2 = Moose)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'SPECIES'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifer for sex of involved animal.  Allowed values: M, F, U', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'SEX'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifer for age range of involved animal.  (eg: A=Adult, Y=Young,U=unknown) ', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'AGE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Text field for comments and/or notes pertinent to the specified occurance.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'COMMENT'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Spatial geometry where the event occured, as conformed to the road network.  ', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'GEOMETRY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'CONCURRENCY_CONTROL_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'APP_CREATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of record creation', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'APP_CREATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'APP_CREATE_USER_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'APP_CREATE_USER_DIRECTORY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'APP_LAST_UPDATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of last record update', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'APP_LAST_UPDATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'APP_LAST_UPDATE_USER_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'APP_LAST_UPDATE_USER_DIRECTORY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'DB_AUDIT_CREATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record created in the database', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'DB_AUDIT_CREATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record was last updated in the database.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_TIMESTAMP'
GO


/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_WILDLIFE_REPORT_HIST"                 */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT_HIST] DROP CONSTRAINT [HMR_WLDLF_H_PK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT_HIST] DROP CONSTRAINT [HMR_WLDLF_H_UK]
GO


CREATE TABLE [dbo].[HMR_WILDLIFE_REPORT_HIST_TMP] (
    [WILDLIFE_REPORT_HIST_ID] BIGINT DEFAULT NEXT VALUE FOR [HMR_WILDLIFE_REPORT_H_ID_SEQ] NOT NULL,
    [EFFECTIVE_DATE_HIST] DATETIME DEFAULT getutcdate() NOT NULL,
    [END_DATE_HIST] DATETIME,
    [WILDLIFE_RECORD_ID] NUMERIC(18) NOT NULL,
    [SUBMISSION_OBJECT_ID] NUMERIC(18) NOT NULL,
    [ROW_ID] NUMERIC(9) NOT NULL,
    [ROW_NUM] NUMERIC(18),
    [VALIDATION_STATUS_ID] NUMERIC(18),
    [RECORD_TYPE] VARCHAR(1),
    [SERVICE_AREA] NUMERIC(18) NOT NULL,
    [ACCIDENT_DATE] DATETIME,
    [TIME_OF_KILL] VARCHAR(1),
    [LATITUDE] NUMERIC(16,8),
    [LONGITUDE] NUMERIC(16,8),
    [HIGHWAY_UNIQUE] VARCHAR(16),
    [HIGHWAY_UNIQUE_NAME] VARCHAR(40),
    [HIGHWAY_UNIQUE_LENGTH] NUMERIC(25,20),
    [LANDMARK] VARCHAR(8),
    [OFFSET] NUMERIC(7,3),
    [NEAREST_TOWN] VARCHAR(150),
    [WILDLIFE_SIGN] VARCHAR(1),
    [QUANTITY] NUMERIC(18),
    [SPECIES] NUMERIC(18),
    [SEX] VARCHAR(1),
    [AGE] VARCHAR(1),
    [COMMENT] VARCHAR(1024),
    [GEOMETRY] GEOMETRY,
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


INSERT INTO [dbo].[HMR_WILDLIFE_REPORT_HIST_TMP]
    ([WILDLIFE_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[WILDLIFE_RECORD_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[ACCIDENT_DATE],[TIME_OF_KILL],[LATITUDE],[LONGITUDE],[HIGHWAY_UNIQUE],[LANDMARK],[OFFSET],[NEAREST_TOWN],[WILDLIFE_SIGN],[QUANTITY],[SPECIES],[SEX],[AGE],[COMMENT],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [WILDLIFE_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[WILDLIFE_RECORD_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[ACCIDENT_DATE],[TIME_OF_KILL],[LATITUDE],[LONGITUDE],[HIGHWAY_UNIQUE],[LANDMARK],[OFFSET],[NEAREST_TOWN],[WILDLIFE_SIGN],[QUANTITY],[SPECIES],[SEX],[AGE],[COMMENT],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_WILDLIFE_REPORT_HIST]
GO


DROP TABLE [dbo].[HMR_WILDLIFE_REPORT_HIST]
GO


EXEC sp_rename '[dbo].[HMR_WILDLIFE_REPORT_HIST_TMP]', 'HMR_WILDLIFE_REPORT_HIST', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT_HIST] ADD CONSTRAINT [HMR_WLDLF_H_PK] 
    PRIMARY KEY CLUSTERED ([WILDLIFE_REPORT_HIST_ID])
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT_HIST] ADD CONSTRAINT [HMR_WLDLF_H_UK] 
    UNIQUE ([WILDLIFE_REPORT_HIST_ID], [END_DATE_HIST])
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


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] ADD CONSTRAINT [HMR_RCKF_RRT_SUBM_RW_FK] 
    FOREIGN KEY ([ROW_ID]) REFERENCES [dbo].[HMR_SUBMISSION_ROW] ([ROW_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ADD CONSTRAINT [HMR_SUBM_RW_SUBM_STAT_FK] 
    FOREIGN KEY ([ROW_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_SUBMISSION_ROW] ADD CONSTRAINT [HMR_SUBM_RW_HMR_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RPT_HMR_SRV_ARA_FK] 
    FOREIGN KEY ([SERVICE_AREA]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RPT_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RRT_SUBM_STAT_FK] 
    FOREIGN KEY ([VALIDATION_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RRT_SUBM_RW_FK] 
    FOREIGN KEY ([ROW_ID]) REFERENCES [dbo].[HMR_SUBMISSION_ROW] ([ROW_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_RW_FK] 
    FOREIGN KEY ([ROW_ID]) REFERENCES [dbo].[HMR_SUBMISSION_ROW] ([ROW_ID])
GO


/* ---------------------------------------------------------------------- */
/* Repair/add views                                                       */
/* ---------------------------------------------------------------------- */

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
    insert into HMR_ROCKFALL_REPORT_HIST ([ROCKFALL_REPORT_ID], [SUBMISSION_OBJECT_ID], [ROW_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [MCRR_INCIDENT_NUMBER], [RECORD_TYPE], [SERVICE_AREA], [ESTIMATED_ROCKFALL_DATE], [ESTIMATED_ROCKFALL_TIME], [START_LATITUDE], [START_LONGITUDE], [END_LATITUDE], [END_LONGITUDE], [HIGHWAY_UNIQUE], [HIGHWAY_UNIQUE_NAME], [HIGHWAY_UNIQUE_LENGTH], [LANDMARK], [LANDMARK_NAME], [START_OFFSET], [END_OFFSET], [DIRECTION_FROM_LANDMARK], [LOCATION_DESCRIPTION], [DITCH_VOLUME], [TRAVELLED_LANES_VOLUME], [OTHER_TRAVELLED_LANES_VOLUME], [OTHER_DITCH_VOLUME], [HEAVY_PRECIP], [FREEZE_THAW], [DITCH_SNOW_ICE], [VEHICLE_DAMAGE], [COMMENTS], [REPORTER_NAME], [MC_PHONE_NUMBER], [REPORT_DATE], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], ROCKFALL_REPORT_HIST_ID, END_DATE_HIST, EFFECTIVE_DATE_HIST)
      select [ROCKFALL_REPORT_ID], [SUBMISSION_OBJECT_ID], [ROW_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [MCRR_INCIDENT_NUMBER], [RECORD_TYPE], [SERVICE_AREA], [ESTIMATED_ROCKFALL_DATE], [ESTIMATED_ROCKFALL_TIME], [START_LATITUDE], [START_LONGITUDE], [END_LATITUDE], [END_LONGITUDE], [HIGHWAY_UNIQUE], [HIGHWAY_UNIQUE_NAME], [HIGHWAY_UNIQUE_LENGTH], [LANDMARK], [LANDMARK_NAME], [START_OFFSET], [END_OFFSET], [DIRECTION_FROM_LANDMARK], [LOCATION_DESCRIPTION], [DITCH_VOLUME], [TRAVELLED_LANES_VOLUME], [OTHER_TRAVELLED_LANES_VOLUME], [OTHER_DITCH_VOLUME], [HEAVY_PRECIP], [FREEZE_THAW], [DITCH_SNOW_ICE], [VEHICLE_DAMAGE], [COMMENTS], [REPORTER_NAME], [MC_PHONE_NUMBER], [REPORT_DATE], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_ROCKFALL_REPORT_H_ID_SEQ]) as [ROCKFALL_REPORT_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

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
      "ROW_ID",
      "ROW_NUM",
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
      "HIGHWAY_UNIQUE_LENGTH",
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
      "GEOMETRY",
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
      "ROW_ID",
      "ROW_NUM",
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
      "HIGHWAY_UNIQUE_LENGTH",
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
      "GEOMETRY",
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
      "ROW_ID" = inserted."ROW_ID",
      "ROW_NUM" = inserted."ROW_NUM",
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
      "HIGHWAY_UNIQUE_LENGTH" = inserted."HIGHWAY_UNIQUE_LENGTH",
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
      "GEOMETRY" = inserted."GEOMETRY",
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


CREATE TRIGGER HMR_SUBM_RW_I_S_I_TR ON HMR_SUBMISSION_ROW INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_SUBMISSION_ROW ("ROW_ID",
      "SUBMISSION_OBJECT_ID",
      "ROW_STATUS_ID",
      "ROW_NUM",
      "RECORD_NUMBER",
      "ROW_VALUE",
      "ROW_HASH",
      "START_VARIANCE",
      "END_VARIANCE",
      "IS_RESUBMITTED",
      "ERROR_DETAIL",
      "WARNING_DETAIL",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "ROW_ID",
      "SUBMISSION_OBJECT_ID",
      "ROW_STATUS_ID",
      "ROW_NUM",
      "RECORD_NUMBER",
      "ROW_VALUE",
      "ROW_HASH",
      "START_VARIANCE",
      "END_VARIANCE",
      "IS_RESUBMITTED",
      "ERROR_DETAIL",
      "WARNING_DETAIL",
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
GO


CREATE TRIGGER HMR_SUBM_RW_I_S_U_TR ON HMR_SUBMISSION_ROW INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.ROW_ID = deleted.ROW_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SUBMISSION_ROW
    set "ROW_ID" = inserted."ROW_ID",
      "SUBMISSION_OBJECT_ID" = inserted."SUBMISSION_OBJECT_ID",
      "ROW_STATUS_ID" = inserted."ROW_STATUS_ID",
      "ROW_NUM" = inserted."ROW_NUM",
      "RECORD_NUMBER" = inserted."RECORD_NUMBER",
      "ROW_VALUE" = inserted."ROW_VALUE",
      "ROW_HASH" = inserted."ROW_HASH", 
      "START_VARIANCE" = inserted."START_VARIANCE",
      "END_VARIANCE" = inserted."END_VARIANCE",
      "IS_RESUBMITTED" = inserted."IS_RESUBMITTED",
      "ERROR_DETAIL" = inserted."ERROR_DETAIL",
      "WARNING_DETAIL" = inserted."WARNING_DETAIL",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SUBMISSION_ROW
    inner join inserted
    on (HMR_SUBMISSION_ROW.ROW_ID = inserted.ROW_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO
GO


CREATE TRIGGER [dbo].[HMR_WLDLF_RPT_A_S_IUD_TR] ON HMR_WILDLIFE_REPORT FOR INSERT, UPDATE, DELETE AS
SET NOCOUNT ON
BEGIN TRY
DECLARE @curr_date datetime;
SET @curr_date = getutcdate();
  IF NOT EXISTS(SELECT * FROM inserted) AND NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- historical
  IF EXISTS(SELECT * FROM deleted)
    update HMR_WILDLIFE_REPORT_HIST set END_DATE_HIST = @curr_date where WILDLIFE_RECORD_ID in (select WILDLIFE_RECORD_ID from deleted) and END_DATE_HIST is null;

  IF EXISTS(SELECT * FROM inserted)
    insert into HMR_WILDLIFE_REPORT_HIST ([WILDLIFE_RECORD_ID], [SUBMISSION_OBJECT_ID], [ROW_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [RECORD_TYPE], [SERVICE_AREA], [ACCIDENT_DATE], [TIME_OF_KILL], [LATITUDE], [LONGITUDE], [HIGHWAY_UNIQUE], [HIGHWAY_UNIQUE_NAME], [HIGHWAY_UNIQUE_LENGTH], [LANDMARK], [OFFSET], [NEAREST_TOWN], [WILDLIFE_SIGN], [QUANTITY], [SPECIES], [SEX], [AGE], [COMMENT], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], WILDLIFE_REPORT_HIST_ID, END_DATE_HIST, EFFECTIVE_DATE_HIST)
      select [WILDLIFE_RECORD_ID], [SUBMISSION_OBJECT_ID], [ROW_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [RECORD_TYPE], [SERVICE_AREA], [ACCIDENT_DATE], [TIME_OF_KILL], [LATITUDE], [LONGITUDE], [HIGHWAY_UNIQUE], [HIGHWAY_UNIQUE_NAME], [HIGHWAY_UNIQUE_LENGTH], [LANDMARK], [OFFSET], [NEAREST_TOWN], [WILDLIFE_SIGN], [QUANTITY], [SPECIES], [SEX], [AGE], [COMMENT], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_WILDLIFE_REPORT_H_ID_SEQ]) as [WILDLIFE_REPORT_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_WLDLF_RPT_I_S_I_TR] ON HMR_WILDLIFE_REPORT INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted)
    RETURN;


  insert into HMR_WILDLIFE_REPORT ("WILDLIFE_RECORD_ID",
      "SUBMISSION_OBJECT_ID", 
      "ROW_ID",
      "ROW_NUM",
      "VALIDATION_STATUS_ID",
      "RECORD_TYPE",
      "SERVICE_AREA",
      "ACCIDENT_DATE",
      "TIME_OF_KILL",
      "LATITUDE",
      "LONGITUDE",
      "HIGHWAY_UNIQUE",
      "HIGHWAY_UNIQUE_NAME",
      "HIGHWAY_UNIQUE_LENGTH",
      "LANDMARK",
      "OFFSET",
      "NEAREST_TOWN",
      "WILDLIFE_SIGN",
      "QUANTITY",
      "SPECIES",
      "SEX",
      "AGE",
      "COMMENT",
      "GEOMETRY",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "WILDLIFE_RECORD_ID",
      "SUBMISSION_OBJECT_ID", 
      "ROW_ID",
      "ROW_NUM",
      "VALIDATION_STATUS_ID",
      "RECORD_TYPE",
      "SERVICE_AREA",
      "ACCIDENT_DATE",
      "TIME_OF_KILL",
      "LATITUDE",
      "LONGITUDE",
      "HIGHWAY_UNIQUE",
      "HIGHWAY_UNIQUE_NAME",
      "HIGHWAY_UNIQUE_LENGTH",
      "LANDMARK",
      "OFFSET",
      "NEAREST_TOWN",
      "WILDLIFE_SIGN",
      "QUANTITY",
      "SPECIES",
      "SEX",
      "AGE",
      "COMMENT",
      "GEOMETRY",
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


CREATE TRIGGER [dbo].[HMR_WLDLF_RPT_I_S_U_TR] ON HMR_WILDLIFE_REPORT INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.WILDLIFE_RECORD_ID = deleted.WILDLIFE_RECORD_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_WILDLIFE_REPORT
    set "WILDLIFE_RECORD_ID" = inserted."WILDLIFE_RECORD_ID",
      "SUBMISSION_OBJECT_ID" = inserted."SUBMISSION_OBJECT_ID",
      "ROW_ID" = inserted."ROW_ID",
      "ROW_NUM" = inserted."ROW_NUM",
      "VALIDATION_STATUS_ID" = inserted."VALIDATION_STATUS_ID",
      "RECORD_TYPE" = inserted."RECORD_TYPE",
      "SERVICE_AREA" = inserted."SERVICE_AREA",
      "ACCIDENT_DATE" = inserted."ACCIDENT_DATE",
      "TIME_OF_KILL" = inserted."TIME_OF_KILL",
      "LATITUDE" = inserted."LATITUDE",
      "LONGITUDE" = inserted."LONGITUDE",
      "HIGHWAY_UNIQUE" = inserted."HIGHWAY_UNIQUE",
      "HIGHWAY_UNIQUE_NAME" = inserted."HIGHWAY_UNIQUE_NAME",
      "HIGHWAY_UNIQUE_LENGTH" = inserted."HIGHWAY_UNIQUE_LENGTH",
      "LANDMARK" = inserted."LANDMARK",
      "OFFSET" = inserted."OFFSET",
      "NEAREST_TOWN" = inserted."NEAREST_TOWN",
      "WILDLIFE_SIGN" = inserted."WILDLIFE_SIGN",
      "QUANTITY" = inserted."QUANTITY",
      "SPECIES" = inserted."SPECIES",
      "SEX" = inserted."SEX",
      "AGE" = inserted."AGE",
      "COMMENT" = inserted."COMMENT",
      "GEOMETRY" = inserted."GEOMETRY",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_WILDLIFE_REPORT
    inner join inserted
    on (HMR_WILDLIFE_REPORT.WILDLIFE_RECORD_ID = inserted.WILDLIFE_RECORD_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO

