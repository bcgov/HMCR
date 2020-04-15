/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 11.1.0                     */
/* Target DBMS:           MS SQL Server 2017                              */
/* Project file:          APP_HMR.dez                                     */
/* Project name:          Highway Maintenance Reporting - v18             */
/* Author:                Ben Driver                                      */
/* Script type:           Alter database script                           */
/* Created on:            2020-03-04 12:37                                */
/* ---------------------------------------------------------------------- */

-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-03-04
-- Updates: 
--	
-- 
-- Description:	Incremental DML in support of sprint 8.
--  - Revise join to optional between PARTY and CONTRACT_TERM to faciliate manual association
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


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_RW_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_STAT_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SUBM_OBJ_FK]
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] DROP CONSTRAINT [HMR_WRK_RRT_SRV_ARA_FK]
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
    [ROW_ID] NUMERIC(9) NOT NULL,
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
    [HIGHWAY_UNIQUE_NAME] VARCHAR(255),
    [HIGHWAY_UNIQUE_LENGTH] NUMERIC(25,20),
    [LANDMARK] VARCHAR(8),
    [START_OFFSET] NUMERIC(7,3),
    [END_OFFSET] NUMERIC(7,3),
    [START_LATITUDE] NUMERIC(16,8),
    [START_LONGITUDE] NUMERIC(16,8),
    [END_LATITUDE] NUMERIC(16,8),
    [END_LONGITUDE] NUMERIC(16,8),
    [WORK_LENGTH] NUMERIC(25,20),
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
    ([WORK_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[RECORD_NUMBER],[TASK_NUMBER],[ACTIVITY_NUMBER],[START_DATE],[END_DATE],[ACCOMPLISHMENT],[UNIT_OF_MEASURE],[POSTED_DATE],[HIGHWAY_UNIQUE],[LANDMARK],[START_OFFSET],[END_OFFSET],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[STRUCTURE_NUMBER],[SITE_NUMBER],[VALUE_OF_WORK],[COMMENTS],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [WORK_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[RECORD_NUMBER],[TASK_NUMBER],[ACTIVITY_NUMBER],[START_DATE],[END_DATE],[ACCOMPLISHMENT],[UNIT_OF_MEASURE],[POSTED_DATE],[HIGHWAY_UNIQUE],[LANDMARK],[START_OFFSET],[END_OFFSET],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[STRUCTURE_NUMBER],[SITE_NUMBER],[VALUE_OF_WORK],[COMMENTS],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
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


CREATE NONCLUSTERED INDEX [HMR_WRK_RRT_FK_I] ON [dbo].[HMR_WORK_REPORT] ([SUBMISSION_OBJECT_ID] ASC,[ROW_ID] ASC)
GO


EXECUTE sp_addextendedproperty N'MS_Description', N'Submission data regarding maintenance activities is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', NULL, NULL
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'A system generated unique identifier.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'WORK_REPORT_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for SUBMISSION OBJECT.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'SUBMISSION_OBJECT_ID'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Unique identifier for originating SUBMISSION ROW.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'ROW_ID'
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

EXECUTE sp_addextendedproperty N'MS_Description', N'Road or Highway description sourced from a road network data product (Road Feature Inventory [RFI] as of Dec 2019).  The name is derived from the HIGHWAY_UNIQUE value provided wtihin the submission.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'HIGHWAY_UNIQUE_NAME'
GO

EXECUTE sp_addextendedproperty N'MS_Description', N'Driven length in KM of the HIGHWAY_UNIQUE segment at the time of data submission.  ', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'HIGHWAY_UNIQUE_LENGTH'
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

EXECUTE sp_addextendedproperty N'MS_Description', N'Driven length in KM of the work distance as calculated from start and end coordinates and related HIGHWAY_UNIQUE segment at the time of data submission.', 'SCHEMA', N'dbo', 'TABLE', N'HMR_WORK_REPORT', 'COLUMN', N'WORK_LENGTH'
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
    [ROW_ID] NUMERIC(9) NOT NULL,
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
    [HIGHWAY_UNIQUE_NAME] VARCHAR(255),
    [HIGHWAY_UNIQUE_LENGTH] NUMERIC(25,20),
    [LANDMARK] VARCHAR(8),
    [START_OFFSET] NUMERIC(16,8),
    [END_OFFSET] NUMERIC(16,8),
    [START_LATITUDE] NUMERIC(16,8),
    [START_LONGITUDE] NUMERIC(16,8),
    [END_LATITUDE] NUMERIC(16,8),
    [END_LONGITUDE] NUMERIC(16,8),
    [WORK_LENGTH] NUMERIC(25,20),
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
    ([WORK_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[WORK_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[RECORD_NUMBER],[TASK_NUMBER],[ACTIVITY_NUMBER],[START_DATE],[END_DATE],[ACCOMPLISHMENT],[UNIT_OF_MEASURE],[POSTED_DATE],[HIGHWAY_UNIQUE],[LANDMARK],[START_OFFSET],[END_OFFSET],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[STRUCTURE_NUMBER],[SITE_NUMBER],[VALUE_OF_WORK],[COMMENTS],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP])
SELECT
    [WORK_REPORT_HIST_ID],[EFFECTIVE_DATE_HIST],[END_DATE_HIST],[WORK_REPORT_ID],[SUBMISSION_OBJECT_ID],[ROW_ID],[ROW_NUM],[VALIDATION_STATUS_ID],[RECORD_TYPE],[SERVICE_AREA],[RECORD_NUMBER],[TASK_NUMBER],[ACTIVITY_NUMBER],[START_DATE],[END_DATE],[ACCOMPLISHMENT],[UNIT_OF_MEASURE],[POSTED_DATE],[HIGHWAY_UNIQUE],[LANDMARK],[START_OFFSET],[END_OFFSET],[START_LATITUDE],[START_LONGITUDE],[END_LATITUDE],[END_LONGITUDE],[STRUCTURE_NUMBER],[SITE_NUMBER],[VALUE_OF_WORK],[COMMENTS],[GEOMETRY],[CONCURRENCY_CONTROL_NUMBER],[APP_CREATE_USERID],[APP_CREATE_TIMESTAMP],[APP_CREATE_USER_GUID],[APP_CREATE_USER_DIRECTORY],[APP_LAST_UPDATE_USERID],[APP_LAST_UPDATE_TIMESTAMP],[APP_LAST_UPDATE_USER_GUID],[APP_LAST_UPDATE_USER_DIRECTORY],[DB_AUDIT_CREATE_USERID],[DB_AUDIT_CREATE_TIMESTAMP],[DB_AUDIT_LAST_UPDATE_USERID],[DB_AUDIT_LAST_UPDATE_TIMESTAMP]
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


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_STAT_FK] 
    FOREIGN KEY ([VALIDATION_STATUS_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STATUS] ([STATUS_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_OBJ_FK] 
    FOREIGN KEY ([SUBMISSION_OBJECT_ID]) REFERENCES [dbo].[HMR_SUBMISSION_OBJECT] ([SUBMISSION_OBJECT_ID])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SRV_ARA_FK] 
    FOREIGN KEY ([SERVICE_AREA]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])
GO


ALTER TABLE [dbo].[HMR_WORK_REPORT] ADD CONSTRAINT [HMR_WRK_RRT_SUBM_RW_FK] 
    FOREIGN KEY ([ROW_ID]) REFERENCES [dbo].[HMR_SUBMISSION_ROW] ([ROW_ID])
GO


/* ---------------------------------------------------------------------- */
/* MODIFIED - Drop views                                                             */
/* ---------------------------------------------------------------------- */

GO


IF EXISTS(SELECT 1 FROM sys.views WHERE name='HMR_WORK_REPORT_VW' AND TYPE='v')
DROP VIEW [dbo].[HMR_WORK_REPORT_VW]
GO

IF EXISTS(SELECT 1 FROM sys.views WHERE name='HMR_WILDLIFE_REPORT_VW' AND TYPE='v')
DROP VIEW [dbo].[HMR_WILDLIFE_REPORT_VW]
GO

IF EXISTS(SELECT 1 FROM sys.views WHERE name='HMR_ROCKFALL_REPORT_VW' AND TYPE='v')
DROP VIEW [dbo].[HMR_ROCKFALL_REPORT_VW]
GO

-- drop abandoned views

IF EXISTS(SELECT 1 FROM sys.views WHERE name='HMR_ALL_VALIDATION_RULES_V' AND TYPE='v')
DROP VIEW [dbo].[HMR_ALL_VALIDATION_RULES_V]
GO

IF EXISTS(SELECT 1 FROM sys.views WHERE name='HMR_CODE_MAINTENANCE_TYPE_V' AND TYPE='v')
DROP VIEW [dbo].[HMR_CODE_MAINTENANCE_TYPE_V]
GO

IF EXISTS(SELECT 1 FROM sys.views WHERE name='HMR_CODE_TIME_OF_DAY_V' AND TYPE='v')
DROP VIEW [dbo].[HMR_CODE_TIME_OF_DAY_V]
GO

IF EXISTS(SELECT 1 FROM sys.views WHERE name='HMR_CODE_UNIT_OF_MEASURE_V' AND TYPE='v')
DROP VIEW [dbo].[HMR_CODE_UNIT_OF_MEASURE_V]
GO

/* ---------------------------------------------------------------------- */
/* Repair/add views                                                       */
/* ---------------------------------------------------------------------- */

GO


CREATE VIEW [dbo].[HMR_WORK_REPORT_VW] AS
      SELECT
      'WORK_REPORT' AS REPORT_TYPE
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
	  ,wrkrpt.WORK_REPORT_ID
      ,wrkrpt.ROW_NUM
      ,subm_stat.STATUS_CODE + ' - ' + subm_stat.DESCRIPTION AS VALIDATION_STATUS
      ,wrkrpt.APP_CREATE_TIMESTAMP
      ,wrkrpt.APP_LAST_UPDATE_TIMESTAMP
  FROM HMR_WORK_REPORT wrkrpt
  INNER JOIN HMR_SUBMISSION_OBJECT subm_obj ON wrkrpt.SUBMISSION_OBJECT_ID = subm_obj.SUBMISSION_OBJECT_ID
  LEFT OUTER JOIN HMR_ACTIVITY_CODE actcode ON wrkrpt.ACTIVITY_NUMBER = actcode.ACTIVITY_NUMBER
  LEFT OUTER JOIN HMR_SUBMISSION_ROW subm_rw ON wrkrpt.ROW_ID = subm_rw.ROW_ID
  LEFT OUTER JOIN HMR_SERVICE_AREA srv_ara ON wrkrpt.[SERVICE_AREA] = srv_ara.SERVICE_AREA_NUMBER
  LEFT OUTER JOIN HMR_SUBMISSION_STATUS subm_stat ON subm_rw.ROW_STATUS_ID = subm_stat.STATUS_ID
GO


CREATE VIEW [dbo].[HMR_WILDLIFE_REPORT_VW] AS
SELECT 
	  'WILDLIFE_REPORT' AS REPORT_TYPE
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
	  ,wldlfrpt.[WILDLIFE_RECORD_ID]
	  ,wldlfrpt.[ROW_NUM]
      ,subm_stat.[STATUS_CODE] + ' - ' + subm_stat.[DESCRIPTION] AS VALIDATION_STATUS
      ,wldlfrpt.[APP_CREATE_TIMESTAMP]
      ,wldlfrpt.[APP_LAST_UPDATE_TIMESTAMP]
  FROM [dbo].[HMR_WILDLIFE_REPORT] wldlfrpt
  INNER JOIN HMR_SUBMISSION_OBJECT subm_obj ON wldlfrpt.SUBMISSION_OBJECT_ID = subm_obj.SUBMISSION_OBJECT_ID
  LEFT OUTER JOIN HMR_SUBMISSION_ROW subm_rw ON wldlfrpt.ROW_ID = subm_rw.ROW_ID
  LEFT OUTER JOIN HMR_SERVICE_AREA srv_ara ON wldlfrpt.[SERVICE_AREA] = srv_ara.SERVICE_AREA_NUMBER
  LEFT OUTER JOIN HMR_SUBMISSION_STATUS subm_stat ON subm_rw.ROW_STATUS_ID = subm_stat.STATUS_ID
GO


CREATE VIEW [dbo].[HMR_ROCKFALL_REPORT_VW] AS
  SELECT 
      'ROCKFALL_REPORT' AS REPORT_TYPE
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
	  ,rckflrpt.[ROCKFALL_REPORT_ID]
      ,rckflrpt.[ROW_NUM]
      ,subm_stat.[STATUS_CODE] + ' - ' + subm_stat.[DESCRIPTION] AS VALIDATION_STATUS
      ,rckflrpt.[APP_CREATE_TIMESTAMP]
      ,rckflrpt.[APP_LAST_UPDATE_TIMESTAMP]
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
    insert into HMR_WORK_REPORT_HIST ([WORK_REPORT_ID], [SUBMISSION_OBJECT_ID], [ROW_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [RECORD_TYPE], [SERVICE_AREA], [RECORD_NUMBER], [TASK_NUMBER], [ACTIVITY_NUMBER], [START_DATE], [END_DATE], [ACCOMPLISHMENT], [UNIT_OF_MEASURE], [POSTED_DATE], [HIGHWAY_UNIQUE], [HIGHWAY_UNIQUE_NAME], [HIGHWAY_UNIQUE_LENGTH], [LANDMARK], [START_OFFSET], [END_OFFSET], [START_LATITUDE], [START_LONGITUDE], [END_LATITUDE], [END_LONGITUDE], [WORK_LENGTH], [STRUCTURE_NUMBER], [SITE_NUMBER], [VALUE_OF_WORK], [COMMENTS], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], [WORK_REPORT_HIST_ID], [END_DATE_HIST], [EFFECTIVE_DATE_HIST])
      select [WORK_REPORT_ID], [SUBMISSION_OBJECT_ID], [ROW_ID], [ROW_NUM], [VALIDATION_STATUS_ID], [RECORD_TYPE], [SERVICE_AREA], [RECORD_NUMBER], [TASK_NUMBER], [ACTIVITY_NUMBER], [START_DATE], [END_DATE], [ACCOMPLISHMENT], [UNIT_OF_MEASURE], [POSTED_DATE], [HIGHWAY_UNIQUE], [HIGHWAY_UNIQUE_NAME], [HIGHWAY_UNIQUE_LENGTH], [LANDMARK], [START_OFFSET], [END_OFFSET], [START_LATITUDE], [START_LONGITUDE], [END_LATITUDE], [END_LONGITUDE], [WORK_LENGTH],  [STRUCTURE_NUMBER], [SITE_NUMBER], [VALUE_OF_WORK], [COMMENTS], [GEOMETRY], [CONCURRENCY_CONTROL_NUMBER], [APP_CREATE_USERID], [APP_CREATE_TIMESTAMP], [APP_CREATE_USER_GUID], [APP_CREATE_USER_DIRECTORY], [APP_LAST_UPDATE_USERID], [APP_LAST_UPDATE_TIMESTAMP], [APP_LAST_UPDATE_USER_GUID], [APP_LAST_UPDATE_USER_DIRECTORY], [DB_AUDIT_CREATE_USERID], [DB_AUDIT_CREATE_TIMESTAMP], [DB_AUDIT_LAST_UPDATE_USERID], [DB_AUDIT_LAST_UPDATE_TIMESTAMP], (next value for [dbo].[HMR_WORK_REPORT_H_ID_SEQ]) as [WORK_REPORT_HIST_ID], null as [END_DATE_HIST], @curr_date as [EFFECTIVE_DATE_HIST] from inserted;

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
      "ROW_ID",
      "ROW_NUM",
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
      "HIGHWAY_UNIQUE_NAME",
      "HIGHWAY_UNIQUE_LENGTH",
      "LANDMARK",
      "START_OFFSET",
      "END_OFFSET",
      "START_LATITUDE",
      "START_LONGITUDE",
      "END_LATITUDE",
      "END_LONGITUDE",
      "WORK_LENGTH",
      "STRUCTURE_NUMBER",
      "SITE_NUMBER",
      "VALUE_OF_WORK",
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
      "ROW_ID",
      "ROW_NUM",
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
      "HIGHWAY_UNIQUE_NAME",
      "HIGHWAY_UNIQUE_LENGTH",
      "LANDMARK",
      "START_OFFSET",
      "END_OFFSET",
      "START_LATITUDE",
      "START_LONGITUDE",
      "END_LATITUDE",
      "END_LONGITUDE",
      "WORK_LENGTH",
      "STRUCTURE_NUMBER",
      "SITE_NUMBER",
      "VALUE_OF_WORK",
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
      "ROW_ID" = inserted."ROW_ID",
      "ROW_NUM" = inserted."ROW_NUM",
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
      "HIGHWAY_UNIQUE_NAME" = inserted."HIGHWAY_UNIQUE_NAME",
      "HIGHWAY_UNIQUE_LENGTH" = inserted."HIGHWAY_UNIQUE_LENGTH",
      "LANDMARK" = inserted."LANDMARK",
      "START_OFFSET" = inserted."START_OFFSET",
      "END_OFFSET" = inserted."END_OFFSET",
      "START_LATITUDE" = inserted."START_LATITUDE",
      "START_LONGITUDE" = inserted."START_LONGITUDE",
      "END_LATITUDE" = inserted."END_LATITUDE",
      "END_LONGITUDE" = inserted."END_LONGITUDE",
      "WORK_LENGTH" = inserted."WORK_LENGTH",
      "STRUCTURE_NUMBER" = inserted."STRUCTURE_NUMBER",
      "SITE_NUMBER" = inserted."SITE_NUMBER",
      "VALUE_OF_WORK" = inserted."VALUE_OF_WORK",
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

