/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting - v15             */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-02-24 06:27                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-24
-- Updates: 
--	
-- 
-- Description:	Incremnetal DML in support of sprint 7.
--  - Addition of geometry fields in staging tables
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


DROP TRIGGER [dbo].[HMR_WLDLF_RPT_A_S_IUD_TR]
GO


DROP TRIGGER [dbo].[HMR_WLDLF_RPT_I_S_I_TR]
GO


DROP TRIGGER [dbo].[HMR_WLDLF_RPT_I_S_U_TR]
GO


DROP TRIGGER [dbo].[HMR_WRK_RPT_A_S_IUD_TR]
GO


DROP TRIGGER [dbo].[HMR_WRK_RPT_I_S_I_TR]
GO


DROP TRIGGER [dbo].[HMR_WRK_RPT_I_S_U_TR]
GO


/* ---------------------------------------------------------------------- */
/* Drop foreign key constraints                                           */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RCKFL_RPT_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RCKFL_RPT_HMR_SRV_ARA_FK]
GO


ALTER TABLE [dbo].[HMR_ROCKFALL_REPORT] DROP CONSTRAINT [HMR_RKFLL_RRT_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RPT_HMR_SRV_ARA_FK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RPT_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] DROP CONSTRAINT [HMR_WLDLF_RRT_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SRV_ARA_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_STAT_FK]
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
    ([ROCKFALL_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[MCRR_INCIDENT_NUMBER],[RECORD_TYPE],[SERVICE_AREA],[ESTIMATED_ROCKFALL_DATE],[ESTIMATED_ROCKFALL_TIME],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[HIGHWAY_UNIQUE],[HIGHWAY_UNIQUE_NAME],[LANDMARK],[LANDMARK_NAME],[START_OFFSET],[END_OFFSET],[DIRECTION_FROM_LANDMARK],[LOCATION_DESCRIPTION],[DITCH_VOLUME],[TRAVELLED_LANES_VOLUME],[OTHER_TRAVELLED_LANES_VOLUME],[OTHER_DITCH_VOLUME],[HEAVY_PRECIP],[FREEZE_THAW],[DITCH_SNOW_ICE],[VEHICLE_DAMAGE],[COMMENTS],[REPORTER_NAME],[MC_PHONE_NUMBER],[REPORT_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [ROCKFALL_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[MCRR_INCIDENT_NUMBER],[RECORD_TYPE],[SERVICE_AREA],[ESTIMATED_ROCKFALL_DATE],[ESTIMATED_ROCKFALL_TIME],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[HIGHWAY_UNIQUE],[HIGHWAY_UNIQUE_NAME],[LANDMARK],[LANDMARK_NAME],[START_OFFSET],[END_OFFSET],[DIRECTION_FROM_LANDMARK],[LOCATION_DESCRIPTION],[DITCH_VOLUME],[TRAVELLED_LANES_VOLUME],[OTHER_TRAVELLED_LANES_VOLUME],[OTHER_DITCH_VOLUME],[HEAVY_PRECIP],[FREEZE_THAW],[DITCH_SNOW_ICE],[VEHICLE_DAMAGE],[COMMENTS],[REPORTER_NAME],[MC_PHONE_NUMBER],[REPORT_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
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


CREATE NONCLUSTERED INDEX [HMR_RCKFL_RPT_FK_I] ON [dbo].[HMR_ROCKFALL_REPORT] ([SUBMISSION_OBJECT_ID] ASC)
GO

/* ---------------------------------------------------------------------- */
/* MODIFIED - Restore data definitions                                    */
/* ---------------------------------------------------------------------- */


EXECUTE sp_addextendedproperty N'MS_Description', N'Submission data regarding rockfall incidents is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'A system generated unique identifier.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'ROCKFALL_REPORT_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for SUBMISSION OBJECT.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'SUBMISSION_OBJECT_ID'
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

EXECUTE sp_addextendedproperty N'MS_Description', N'Road or Highway name sourced from a road network data product (RFI as of Dec 2019)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_ROCKFALL_REPORT', 'COLUMN', N'HIGHWAY_UNIQUE_NAME'
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
    ([ROCKFALL_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[ROCKFALL_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[MCRR_INCIDENT_NUMBER],[RECORD_TYPE],[SERVICE_AREA],[ESTIMATED_ROCKFALL_DATE],[ESTIMATED_ROCKFALL_TIME],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[HIGHWAY_UNIQUE],[HIGHWAY_UNIQUE_NAME],[LANDMARK],[LANDMARK_NAME],[START_OFFSET],[END_OFFSET],[DIRECTION_FROM_LANDMARK],[LOCATION_DESCRIPTION],[DITCH_VOLUME],[TRAVELLED_LANES_VOLUME],[OTHER_TRAVELLED_LANES_VOLUME],[OTHER_DITCH_VOLUME],[HEAVY_PRECIP],[FREEZE_THAW],[DITCH_SNOW_ICE],[VEHICLE_DAMAGE],[COMMENTS],[REPORTER_NAME],[MC_PHONE_NUMBER],[REPORT_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [ROCKFALL_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[ROCKFALL_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[MCRR_INCIDENT_NUMBER],[RECORD_TYPE],[SERVICE_AREA],[ESTIMATED_ROCKFALL_DATE],[ESTIMATED_ROCKFALL_TIME],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[HIGHWAY_UNIQUE],[HIGHWAY_UNIQUE_NAME],[LANDMARK],[LANDMARK_NAME],[START_OFFSET],[END_OFFSET],[DIRECTION_FROM_LANDMARK],[LOCATION_DESCRIPTION],[DITCH_VOLUME],[TRAVELLED_LANES_VOLUME],[OTHER_TRAVELLED_LANES_VOLUME],[OTHER_DITCH_VOLUME],[HEAVY_PRECIP],[FREEZE_THAW],[DITCH_SNOW_ICE],[VEHICLE_DAMAGE],[COMMENTS],[REPORTER_NAME],[MC_PHONE_NUMBER],[REPORT_DATE],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
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
    [ROW_NUM] NUMERIC(9),
    [VALIDATION_STATUS_ID] NUMERIC(9),
    [RECORD_TYPE] VARCHAR(1),
    [SERVICE_AREA] NUMERIC(9) NOT NULL,
    [ACCIDENT_DATE] DATETIME,
    [TIME_OF_KILL] VARCHAR(1),
    [LATITUDE] NUMERIC(16,8),
    [LONGITUDE] NUMERIC(16,8),
    [HIGHWAY_UNIQUE] VARCHAR(16),
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
    ([WILDLIFE_RECORD_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[ACCIDENT_DATE],[TIME_OF_KILL],[LATITUDE],[LONGITUDE],[HIGHWAY_UNIQUE],[LANDMARK],[OFFSET],[NEAREST_TOWN],[WILDLIFE_SIGN],[QUANTITY],[SPECIES],[SEX],[AGE],[COMMENT],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [WILDLIFE_RECORD_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[ACCIDENT_DATE],[TIME_OF_KILL],[LATITUDE],[LONGITUDE],[HIGHWAY_UNIQUE],[LANDMARK],[OFFSET],[NEAREST_TOWN],[WILDLIFE_SIGN],[QUANTITY],[SPECIES],[SEX],[AGE],[COMMENT],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
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


CREATE NONCLUSTERED INDEX [HMR_WLDLF_RPT_SUBM_FK_I] ON [dbo].[HMR_WILDLIFE_REPORT] ([SUBMISSION_OBJECT_ID] ASC)
GO


CREATE NONCLUSTERED INDEX [WLDLF_RPT_CNT_ARA_FK_I] ON [dbo].[HMR_WILDLIFE_REPORT] ([SERVICE_AREA] ASC)
GO

/* ---------------------------------------------------------------------- */
/* MODIFIED - Restore data definitions                                    */
/* ---------------------------------------------------------------------- */

EXECUTE sp_addextendedproperty N'MS_Description', N'Submission data regarding wildlife incidents is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for a record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'WILDLIFE_RECORD_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for SUBMISSION OBJECT.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WILDLIFE_REPORT', 'COLUMN', N'SUBMISSION_OBJECT_ID'
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
    [ROW_NUM] NUMERIC(18),
    [VALIDATION_STATUS_ID] NUMERIC(18),
    [RECORD_TYPE] VARCHAR(1),
    [SERVICE_AREA] NUMERIC(18) NOT NULL,
    [ACCIDENT_DATE] DATETIME,
    [TIME_OF_KILL] VARCHAR(1),
    [LATITUDE] NUMERIC(16,8),
    [LONGITUDE] NUMERIC(16,8),
    [HIGHWAY_UNIQUE] VARCHAR(16),
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
    ([WILDLIFE_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[WILDLIFE_RECORD_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[ACCIDENT_DATE],[TIME_OF_KILL],[LATITUDE],[LONGITUDE],[HIGHWAY_UNIQUE],[LANDMARK],[OFFSET],[NEAREST_TOWN],[WILDLIFE_SIGN],[QUANTITY],[SPECIES],[SEX],[AGE],[COMMENT],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [WILDLIFE_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[WILDLIFE_RECORD_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[ACCIDENT_DATE],[TIME_OF_KILL],[LATITUDE],[LONGITUDE],[HIGHWAY_UNIQUE],[LANDMARK],[OFFSET],[NEAREST_TOWN],[WILDLIFE_SIGN],[QUANTITY],[SPECIES],[SEX],[AGE],[COMMENT],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
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
/* Drop and recreate table "dbo.HMR_WORK_REPORT"                          */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RPT_PK]
GO


CREATE TABLE [dbo].[HMR_WORK_REPORT_TMP] (
    [WORK_REPORT_ID] NUMERIC(9) DEFAULT NEXT VALUE FOR [HMR_WRK_RPT_ID_SEQ] NOT NULL,
    [SUBMISSION_OBJECT_ID] NUMERIC(9) NOT NULL,
    [ROW_NUM] NUMERIC(9),
    [VALIDATION_STATUS_ID] NUMERIC(9),
    [RECORD_TYPE] VARCHAR(1),
    [SERVICE_AREA] NUMERIC(9) NOT NULL,
    [RECORD_NUMBER] VARCHAR(30),
    [TASK_NUMBER] VARCHAR(30),
    [ACTIVITY_NUMBER] VARCHAR(30),
    [START_DATE] DATE,
    [END_DATE] DATE,
    [ACCOMPLISHMENT] NUMERIC(9,2),
    [UNIT_OF_MEASURE] VARCHAR(12),
    [POSTED_DATE] DATE,
    [HIGHWAY_UNIQUE] VARCHAR(16),
    [LANDMARK] VARCHAR(8),
    [START_OFFSET] NUMERIC(7,3),
    [END_OFFSET] NUMERIC(7,3),
    [START_LATITUDE] NUMERIC(16,8),
    [START_LONGITUDE] NUMERIC(16,8),
    [END_LATITUDE] NUMERIC(16,8),
    [END_LONGITUDE] NUMERIC(16,8),
    [STRUCTURE_NUMBER] VARCHAR(5),
    [SITE_NUMBER] VARCHAR(8),
    [VALUE_OF_WORK] NUMERIC(9,2),
    [COMMENTS] VARCHAR(1024),
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


INSERT INTO [dbo].[HMR_WORK_REPORT_TMP]
    ([WORK_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[RECORD_NUMBER],[TASK_NUMBER],[ACTIVITY_NUMBER],[START_DATE],[END_DATE],[ACCOMPLISHMENT],[UNIT_OF_MEASURE],[POSTED_DATE],[HIGHWAY_UNIQUE],[LANDMARK],[START_OFFSET],[END_OFFSET],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[STRUCTURE_NUMBER],[SITE_NUMBER],[VALUE_OF_WORK],[COMMENTS],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [WORK_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[RECORD_NUMBER],[TASK_NUMBER],[ACTIVITY_NUMBER],[START_DATE],[END_DATE],[ACCOMPLISHMENT],[UNIT_OF_MEASURE],[POSTED_DATE],[HIGHWAY_UNIQUE],[LANDMARK],[START_OFFSET],[END_OFFSET],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[STRUCTURE_NUMBER],[SITE_NUMBER],[VALUE_OF_WORK],[COMMENTS],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_WORK_REPORT]
GO


DROP INDEX [dbo].[HMR_WORK_REPORT].[HMR_WRK_RRT_FK_I]
GO


DROP TABLE [dbo].[HMR_WORK_REPORT]
GO


EXEC sp_rename '[dbo].[HMR_WORK_REPORT_TMP]', 'HMR_WORK_REPORT', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RPT_PK] 
    PRIMARY KEY CLUSTERED ([WORK_REPORT_ID])
GO


CREATE NONCLUSTERED INDEX [HMR_WRK_RRT_FK_I] ON [dbo].[HMR_WORK_REPORT] ([SUBMISSION_OBJECT_ID] ASC)
GO

/* ---------------------------------------------------------------------- */
/* MODIFIED - Restore data definitions                                    */
/* ---------------------------------------------------------------------- */

EXECUTE sp_addextendedproperty N'MS_Description', N'Submission data regarding maintenance activities is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'A system generated unique identifier.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'WORK_REPORT_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for SUBMISSION OBJECT.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'SUBMISSION_OBJECT_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Relative row number the record was located within a submission.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'ROW_NUM'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for VALIDATION STATUS.  Indicates the overall status of the submitted row of data.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'VALIDATION_STATUS_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'This field describes the type of work the associated record is reporting on. This is restricted to specific set of values - Q - Quantified, R - Routine, E - Major Event, A - Additional', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'RECORD_TYPE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'The Ministry Contract Service Area number in which the activity occured.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'SERVICE_AREA'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique work report record number from the maintainence contractor. This is uniquely identifies each record submission for a contractor. <Service Area><Record Number> will uniquely identify each record in the application for a particular contractor.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'RECORD_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Contractor Task Number', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'TASK_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Code which uniquely identifies the activity performed.  The number reflects a a classificaiton hierarchy comprised of three levels: ABBCCC  A - the first digit represents Specification Category (eg:2 - Drainage ) BB - the second two digits represent Activity Category (eg: 02 - Drainage Appliance Maintenance) CCC - the last three digits represent Activity Type and Detail (eg: 310 - Boring, Augering.  300 series reflects Quantified value, which would be linear meters in this case.)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'ACTIVITY_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date when work commenced', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'START_DATE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date when work was completed', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'END_DATE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'The number of units of work completed for the activity corresponding to the activity number.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'ACCOMPLISHMENT'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'The code which represents the unit of measure for the specified activity. ', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'UNIT_OF_MEASURE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date the data is posted into the contractor management system', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'POSTED_DATE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'This identifies the section of road on which the activity occurred. This is a value in the in the format: [Service Area]-[area manager area]-[subarea]-[highway number] This should be a value found in RFI (CHRIS)', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'HIGHWAY_UNIQUE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'This field needed for location reference: Landmarks provided should be those listed in the CHRIS HRP report for each Highway or road within the Service Area', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'LANDMARK'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'This field is needed for linear referencing for location specific reports.  Offset from beginning of segment.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'START_OFFSET'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'This field is needed for linear referencing for location specific reports. If the work is less than 30 m, this field is not mandatory Offset from beginning of segment', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'END_OFFSET'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'The M (northing) portion of the activity start coordinate. Specified as a latitude in decimal degrees with six decimal places of precision. Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'START_LATITUDE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'The X (easting) portion of the activity start coordinate. Specified as a longitude in decimal degrees with six decimal places of precision. Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'START_LONGITUDE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'The M (northing) portion of the activity end coordinate. Specified as a latitude in decimal degrees with six decimal places of precision. Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum. For point activity if this field is not provided it can be defaulted to same as START LATITUDE', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'END_LATITUDE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'The X (easting) portion of the activity end coordinate. Specified as a longitude in decimal degrees with six decimal places of precision. Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum. For point activity if this field is not provided it can be defaulted to same as START LONGITUDE.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'END_LONGITUDE'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'From list of Bridge Structure Road (BSR) structures provided by the Province. Is only applicable at defined BSR structures.  BSR structures include; bridges, culverts over 3m, retaining walls.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'STRUCTURE_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Contains a site type code followed by a The Ministry site number. Site types are provided by the Province, are four to six digits preceded by: A – Avalanche B – Arrestor Bed/Dragnet Barrier D – Debris and/or Rockfall L – Landscape R – Rest Area S – Signalized Intersection T – Traffic Patrol W – Weather Station X – Railway Crossing', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'SITE_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Total dollar value of the work activity being reported, for each activity.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'VALUE_OF_WORK'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Text field for comments and/or notes pertinent to the specified activity.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'COMMENTS'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Spatial geometry where the activity occured, as conformed to the road network.   Provided start and end coordinates are used to derive the best-fit road segment.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'GEOMETRY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'CONCURRENCY_CONTROL_NUMBER'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'APP_CREATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of record creation', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'APP_CREATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'APP_CREATE_USER_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'APP_CREATE_USER_DIRECTORY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'APP_LAST_UPDATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time of last record update', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'APP_LAST_UPDATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique idenifier of user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'APP_LAST_UPDATE_USER_GUID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Active Directory which retains source of truth for user idenifiers.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'APP_LAST_UPDATE_USER_DIRECTORY'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who created record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'DB_AUDIT_CREATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record created in the database', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'DB_AUDIT_CREATE_TIMESTAMP'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Named database user who last updated record', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_USERID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Date and time record was last updated in the database.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'DB_AUDIT_LAST_UPDATE_TIMESTAMP'
GO


/* ---------------------------------------------------------------------- */
/* Drop and recreate table "dbo.HMR_WORK_REPORT_HIST"                     */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[HMR_WORK_REPORT_HIST] DROP CONSTRAINT [HMR_WRK_R_H_PK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT_HIST] DROP CONSTRAINT [HMR_WRK_R_H_UK]
GO


CREATE TABLE [dbo].[HMR_WORK_REPORT_HIST_TMP] (
    [WORK_REPORT_HIST_ID] BIGINT DEFAULT NEXT VALUE FOR [HMR_WORK_REPORT_H_ID_SEQ] NOT NULL,
    [EFFECTIVE_DATE_HIST] DATETIME DEFAULT getutcdate() NOT NULL,
    [END_DATE_HIST] DATETIME,
    [WORK_REPORT_ID] NUMERIC(18) NOT NULL,
    [SUBMISSION_OBJECT_ID] NUMERIC(18) NOT NULL,
    [ROW_NUM] NUMERIC(18),
    [VALIDATION_STATUS_ID] NUMERIC(18),
    [RECORD_TYPE] VARCHAR(1),
    [SERVICE_AREA] NUMERIC(18) NOT NULL,
    [RECORD_NUMBER] VARCHAR(30),
    [TASK_NUMBER] VARCHAR(30),
    [ACTIVITY_NUMBER] VARCHAR(30),
    [START_DATE] DATE,
    [END_DATE] DATE,
    [ACCOMPLISHMENT] NUMERIC(9,2),
    [UNIT_OF_MEASURE] VARCHAR(12),
    [POSTED_DATE] DATE,
    [HIGHWAY_UNIQUE] VARCHAR(16),
    [LANDMARK] VARCHAR(8),
    [START_OFFSET] NUMERIC(16,8),
    [END_OFFSET] NUMERIC(16,8),
    [START_LATITUDE] NUMERIC(16,8),
    [START_LONGITUDE] NUMERIC(16,8),
    [END_LATITUDE] NUMERIC(16,8),
    [END_LONGITUDE] NUMERIC(16,8),
    [STRUCTURE_NUMBER] VARCHAR(5),
    [SITE_NUMBER] VARCHAR(8),
    [VALUE_OF_WORK] NUMERIC(9,2),
    [COMMENTS] VARCHAR(1024),
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


INSERT INTO [dbo].[HMR_WORK_REPORT_HIST_TMP]
    ([WORK_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[WORK_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[RECORD_NUMBER],[TASK_NUMBER],[ACTIVITY_NUMBER],[START_DATE],[END_DATE],[ACCOMPLISHMENT],[UNIT_OF_MEASURE],[POSTED_DATE],[HIGHWAY_UNIQUE],[LANDMARK],[START_OFFSET],[END_OFFSET],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[STRUCTURE_NUMBER],[SITE_NUMBER],[VALUE_OF_WORK],[COMMENTS],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [WORK_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[WORK_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[RECORD_NUMBER],[TASK_NUMBER],[ACTIVITY_NUMBER],[START_DATE],[END_DATE],[ACCOMPLISHMENT],[UNIT_OF_MEASURE],[POSTED_DATE],[HIGHWAY_UNIQUE],[LANDMARK],[START_OFFSET],[END_OFFSET],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[STRUCTURE_NUMBER],[SITE_NUMBER],[VALUE_OF_WORK],[COMMENTS],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
FROM [dbo].[HMR_WORK_REPORT_HIST]
GO


DROP TABLE [dbo].[HMR_WORK_REPORT_HIST]
GO


EXEC sp_rename '[dbo].[HMR_WORK_REPORT_HIST_TMP]', 'HMR_WORK_REPORT_HIST', 'OBJECT'
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT_HIST] ADD CONSTRAINT [HMR_WRK_R_H_PK] 
    PRIMARY KEY CLUSTERED ([WORK_REPORT_HIST_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT_HIST] ADD CONSTRAINT [HMR_WRK_R_H_UK] 
    UNIQUE ([WORK_REPORT_HIST_ID], [END_DATE_HIST])
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


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RPT_HMR_SRV_ARA_FK] 
    FOREIGN KEY ([SERVICE_AREA]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RPT_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_WILDLIFE_REPORT] ADD CONSTRAINT [HMR_WLDLF_RRT_SUBM_STAT_FK] 
    FOREIGN KEY ([VALIDATION_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_STAT_FK] 
    FOREIGN KEY ([VALIDATION_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SRV_ARA_FK] 
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
    insert into HMR_ROCKFALL_REPORT_HIST ([ROCKFALL_REPORT_ID], [SUBMISSION_OBJECT_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [MCRR_INCIDENT_NUMBER], [RECORD_TYPE], [SERVICE_AREA], [ESTIMATED_ROCKFALL_DATE], [ESTIMATED_ROCKFALL_TIME], [START_LATITUDE], [START_LONGITUDE], [END_LATITUDE], [END_LONGITUDE], [HIGHWAY_UNIQUE], [HIGHWAY_UNIQUE_NAME], [LANDMARK], [LANDMARK_NAME], [START_OFFSET], [END_OFFSET], [DIRECTION_FROM_LANDMARK], [LOCATION_DESCRIPTION], [DITCH_VOLUME], [TRAVELLED_LANES_VOLUME], [OTHER_TRAVELLED_LANES_VOLUME], [OTHER_DITCH_VOLUME], [HEAVY_PRECIP], [FREEZE_THAW], [DITCH_SNOW_ICE], [VEHICLE_DAMAGE], [COMMENTS], [REPORTER_NAME], [MC_PHONE_NUMBER], [REPORT_DATE], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], ROCKFALL_REPORT_HIST_ID, END_DATE_HIST, EFFECTIVE_DATE_HIST)
      select [ROCKFALL_REPORT_ID], [SUBMISSION_OBJECT_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [MCRR_INCIDENT_NUMBER], [RECORD_TYPE], [SERVICE_AREA], [ESTIMATED_ROCKFALL_DATE], [ESTIMATED_ROCKFALL_TIME], [START_LATITUDE], [START_LONGITUDE], [END_LATITUDE], [END_LONGITUDE], [HIGHWAY_UNIQUE], [HIGHWAY_UNIQUE_NAME], [LANDMARK], [LANDMARK_NAME], [START_OFFSET], [END_OFFSET], [DIRECTION_FROM_LANDMARK], [LOCATION_DESCRIPTION], [DITCH_VOLUME], [TRAVELLED_LANES_VOLUME], [OTHER_TRAVELLED_LANES_VOLUME], [OTHER_DITCH_VOLUME], [HEAVY_PRECIP], [FREEZE_THAW], [DITCH_SNOW_ICE], [VEHICLE_DAMAGE], [COMMENTS], [REPORTER_NAME], [MC_PHONE_NUMBER], [REPORT_DATE], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_ROCKFALL_REPORT_H_ID_SEQ]) as [ROCKFALL_REPORT_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

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
    insert into HMR_WILDLIFE_REPORT_HIST ([WILDLIFE_RECORD_ID], [SUBMISSION_OBJECT_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [RECORD_TYPE], [SERVICE_AREA], [ACCIDENT_DATE], [TIME_OF_KILL], [LATITUDE], [LONGITUDE], [HIGHWAY_UNIQUE], [LANDMARK], [OFFSET], [NEAREST_TOWN], [WILDLIFE_SIGN], [QUANTITY], [SPECIES], [SEX], [AGE], [COMMENT], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], WILDLIFE_REPORT_HIST_ID, END_DATE_HIST, EFFECTIVE_DATE_HIST)
      select [WILDLIFE_RECORD_ID], [SUBMISSION_OBJECT_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [RECORD_TYPE], [SERVICE_AREA], [ACCIDENT_DATE], [TIME_OF_KILL], [LATITUDE], [LONGITUDE], [HIGHWAY_UNIQUE], [LANDMARK], [OFFSET], [NEAREST_TOWN], [WILDLIFE_SIGN], [QUANTITY], [SPECIES], [SEX], [AGE], [COMMENT], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_WILDLIFE_REPORT_H_ID_SEQ]) as [WILDLIFE_REPORT_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

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
      "VALIDATION_STATUS_ID",
      "RECORD_TYPE",
      "SERVICE_AREA",
      "ACCIDENT_DATE",
      "TIME_OF_KILL",
      "LATITUDE",
      "LONGITUDE",
      "HIGHWAY_UNIQUE",
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
      "VALIDATION_STATUS_ID",
      "RECORD_TYPE",
      "SERVICE_AREA",
      "ACCIDENT_DATE",
      "TIME_OF_KILL",
      "LATITUDE",
      "LONGITUDE",
      "HIGHWAY_UNIQUE",
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
      "VALIDATION_STATUS_ID" = inserted."VALIDATION_STATUS_ID",
      "RECORD_TYPE" = inserted."RECORD_TYPE",
      "SERVICE_AREA" = inserted."SERVICE_AREA",
      "ACCIDENT_DATE" = inserted."ACCIDENT_DATE",
      "TIME_OF_KILL" = inserted."TIME_OF_KILL",
      "LATITUDE" = inserted."LATITUDE",
      "LONGITUDE" = inserted."LONGITUDE",
      "HIGHWAY_UNIQUE" = inserted."HIGHWAY_UNIQUE",
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


CREATE TRIGGER [dbo].[HMR_WRK_RPT_A_S_IUD_TR] ON HMR_WORK_REPORT FOR INSERT, UPDATE, DELETE AS
SET NOCOUNT ON
BEGIN TRY
DECLARE @curr_date datetime;
SET @curr_date = getutcdate();
  IF NOT EXISTS(SELECT * FROM inserted) AND NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- historical
  IF EXISTS(SELECT * FROM deleted)
    update HMR_WORK_REPORT_HIST set END_DATE_HIST = @curr_date where WORK_REPORT_ID in (select WORK_REPORT_ID from deleted) and END_DATE_HIST is null;

  IF EXISTS(SELECT * FROM inserted)
    insert into HMR_WORK_REPORT_HIST ([WORK_REPORT_ID], [SUBMISSION_OBJECT_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [RECORD_TYPE], [SERVICE_AREA], [RECORD_NUMBER], [TASK_NUMBER], [ACTIVITY_NUMBER], [START_DATE], [END_DATE], [ACCOMPLISHMENT], [UNIT_OF_MEASURE], [POSTED_DATE], [HIGHWAY_UNIQUE], [LANDMARK], [START_OFFSET], [END_OFFSET], [START_LATITUDE], [START_LONGITUDE], [END_LATITUDE], [END_LONGITUDE], [STRUCTURE_NUMBER], [SITE_NUMBER], [VALUE_OF_WORK], [COMMENTS], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], WORK_REPORT_HIST_ID, END_DATE_HIST, EFFECTIVE_DATE_HIST)
      select [WORK_REPORT_ID], [SUBMISSION_OBJECT_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [RECORD_TYPE], [SERVICE_AREA], [RECORD_NUMBER], [TASK_NUMBER], [ACTIVITY_NUMBER], [START_DATE], [END_DATE], [ACCOMPLISHMENT], [UNIT_OF_MEASURE], [POSTED_DATE], [HIGHWAY_UNIQUE], [LANDMARK], [START_OFFSET], [END_OFFSET], [START_LATITUDE], [START_LONGITUDE], [END_LATITUDE], [END_LONGITUDE], [STRUCTURE_NUMBER], [SITE_NUMBER], [VALUE_OF_WORK], [COMMENTS], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_WORK_REPORT_H_ID_SEQ]) as [WORK_REPORT_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO


CREATE TRIGGER [dbo].[HMR_WRK_RPT_I_S_I_TR] ON HMR_WORK_REPORT INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted)
    RETURN;


  insert into HMR_WORK_REPORT ("WORK_REPORT_ID",
      "SUBMISSION_OBJECT_ID",
      "VALIDATION_STATUS_ID",
      "RECORD_TYPE",
      "SERVICE_AREA",
      "RECORD_NUMBER",
      "TASK_NUMBER",
      "ACTIVITY_NUMBER",
      "START_DATE",
      "END_DATE",
      "ACCOMPLISHMENT",
      "UNIT_OF_MEASURE",
      "POSTED_DATE",
      "HIGHWAY_UNIQUE",
      "LANDMARK",
      "START_OFFSET",
      "END_OFFSET",
      "START_LATITUDE",
      "START_LONGITUDE",
      "END_LATITUDE",
      "END_LONGITUDE",
      "STRUCTURE_NUMBER",
      "SITE_NUMBER",
      "VALUE_OF_WORK ",
      "COMMENTS",
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
    select "WORK_REPORT_ID",
      "SUBMISSION_OBJECT_ID",
      "VALIDATION_STATUS_ID",
      "RECORD_TYPE",
      "SERVICE_AREA",
      "RECORD_NUMBER",
      "TASK_NUMBER",
      "ACTIVITY_NUMBER",
      "START_DATE",
      "END_DATE",
      "ACCOMPLISHMENT",
      "UNIT_OF_MEASURE",
      "POSTED_DATE",
      "HIGHWAY_UNIQUE",
      "LANDMARK",
      "START_OFFSET",
      "END_OFFSET",
      "START_LATITUDE",
      "START_LONGITUDE",
      "END_LATITUDE",
      "END_LONGITUDE",
      "STRUCTURE_NUMBER",
      "SITE_NUMBER",
      "VALUE_OF_WORK ",
      "COMMENTS",
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


CREATE TRIGGER [dbo].[HMR_WRK_RPT_I_S_U_TR] ON HMR_WORK_REPORT INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted)
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.WORK_REPORT_ID = deleted.WORK_REPORT_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_WORK_REPORT
    set "WORK_REPORT_ID" = inserted."WORK_REPORT_ID",
      "SUBMISSION_OBJECT_ID" = inserted."SUBMISSION_OBJECT_ID",
      "VALIDATION_STATUS_ID" = inserted."VALIDATION_STATUS_ID",
      "RECORD_TYPE" = inserted."RECORD_TYPE",
      "SERVICE_AREA" = inserted."SERVICE_AREA",
      "RECORD_NUMBER" = inserted."RECORD_NUMBER",
      "TASK_NUMBER" = inserted."TASK_NUMBER",
      "ACTIVITY_NUMBER" = inserted."ACTIVITY_NUMBER",
      "START_DATE" = inserted."START_DATE",
      "END_DATE" = inserted."END_DATE",
      "ACCOMPLISHMENT" = inserted."ACCOMPLISHMENT",
      "UNIT_OF_MEASURE" = inserted."UNIT_OF_MEASURE",
      "POSTED_DATE" = inserted."POSTED_DATE",
      "HIGHWAY_UNIQUE" = inserted."HIGHWAY_UNIQUE",
      "LANDMARK" = inserted."LANDMARK",
      "START_OFFSET" = inserted."START_OFFSET",
      "END_OFFSET" = inserted."END_OFFSET",
      "START_LATITUDE" = inserted."START_LATITUDE",
      "START_LONGITUDE" = inserted."START_LONGITUDE",
      "END_LATITUDE" = inserted."END_LATITUDE",
      "END_LONGITUDE" = inserted."END_LONGITUDE",
      "STRUCTURE_NUMBER" = inserted."STRUCTURE_NUMBER",
      "SITE_NUMBER" = inserted."SITE_NUMBER",
      "VALUE_OF_WORK " = inserted."VALUE_OF_WORK ",
      "COMMENTS" = inserted."COMMENTS", 
      "GEOMETRY" = inserted."GEOMETRY",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_WORK_REPORT
    inner join inserted
    on (HMR_WORK_REPORT.WORK_REPORT_ID = inserted.WORK_REPORT_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
GO

