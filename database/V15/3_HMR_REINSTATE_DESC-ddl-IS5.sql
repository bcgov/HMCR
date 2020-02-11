
-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-01-31
-- Updates: 
--	
-- 
-- Description:	v0.15 Reinstating lost attribute descriptions due to v0.14 incremental DDL
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO


-- HMR_ROCKFALL_REPORT reinstate descriptions

EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifier for SUBMISSION OBJECT.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'SUBMISSION_OBJECT_ID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifier for validation STATUS.  Indicates the overall status of the submitted row of data.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'VALIDATION_STATUS_ID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Rockfall reporting incident number. Unique work report record number from the Contractor maintenance management system.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'MCRR_INCIDENT_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Estimated date of occurrence.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'ESTIMATED_ROCKFALL_DATE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Estimated time of occurrence using the 24-hour clock' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'ESTIMATED_ROCKFALL_TIME' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The M (northing) portion of the activity start coordinate. Specified as a latitude in decimal degrees with six decimal places of precision.
Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum.
' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'START_LATITUDE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The X (easting) portion of the activity start coordinate. Specified as a longitude in decimal degrees with six decimal places of precision.
Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'START_LONGITUDE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The M (northing) portion of the activity end coordinate. Specified as a latitude in decimal degrees with six decimal places of precision.
Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum.
For point activity if this field is not provided it can be defaulted to same as START LATITUDE' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'END_LATITUDE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The X (easting) portion of the activity end coordinate. Specified as a longitude in decimal degrees with six decimal places of precision.
Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum.
For point activity if this field is not provided it can be defaulted to same as START LONGITUDE.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'END_LONGITUDE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'This identifies the section of road on which the activity occurred.  Road or Highway number sourced from a road network data product (RFI as of  2019)
This is a value in the in the format: [Service Area]-[area manager area]-[subarea]-[highway number]' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'HIGHWAY_UNIQUE_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Road or Highway name sourced from a road network data product (RFI as of Dec 2019)' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'HIGHWAY_UNIQUE_NAME' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'This field needed for location reference:
Landmarks provided should be those listed in the CHRIS HRP report for each Highway or road within the Service Area' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'LANDMARK' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Highway reference point (HRP) landmark name or textual description ' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'LAND_MARK_NAME' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'This field is needed for linear referencing for location specific reports. 
Offset from beginning of segment.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'START_OFFSET' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'This field is needed for linear referencing for location specific reports.
If the work is less than 30 m, this field is not mandatory
Offset from beginning of segment' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'END_OFFSET' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Direction of travel from Landmark to START_OFFSET' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'DIRECTION_FROM_LANDMARK' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Text field for comments and/or notes pertinent to the specified activity.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'LOCATION_DESCRIPTION' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Range of estimated volume of material in ditch (m cubed). if volume exceeds 5.0 m3 and report value in the other volume field.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'DITCH_VOLUME' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Range of estimated volume of material on the road (m cubed).' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'TRAVELLED_LANES_VOLUME' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Estimated volume of material.  Should be populated if  ditch and travelled lanes estimated ranges exceed 5.0 m cubed.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'OTHER_VOLUME' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Heavy precipitation conditions present at rockfall site. Enter “Y” or leave blank.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'HEAVY_PRECIP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Freezing/thawing conditions present at rockfall site. Enter “Y” or leave blank.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'FREEZE_THAW' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Ditch snow or ice conditions present at rockfall site. Enter “Y” or leave blank.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'DITCH_SNOW_ICE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Vehicle damage present at rockfall site. Enter “Y” or leave blank.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'VEHICLE_DAMAGE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Comments of occurrence' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'COMMENTS' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Name of person reporting occurrence' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'REPORTER_NAME' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Phone number of person reporting' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'MC_PHONE_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date reported' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'REPORT_DATE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'CONCURRENCY_CONTROL_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'APP_CREATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time of record creation' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'APP_CREATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'APP_CREATE_USER_GUID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Active Directory which retains source of truth for user idenifiers.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'APP_CREATE_USER_DIRECTORY' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time of last record update' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_USER_GUID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Active Directory which retains source of truth for user idenifiers.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_USER_DIRECTORY' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Named database user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'DB_AUDIT_CREATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time record created in the database' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'DB_AUDIT_CREATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Named database user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'DB_AUDIT_LAST_UPDATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time record was last updated in the database.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'DB_AUDIT_LAST_UPDATE_TIMESTAMP' 
GO


-- HMR_WILDLIFE_REPORT reinstate descriptions


EXEC sp_addextendedproperty 'MS_Description' , 'Submission data regarding wildlife incidents is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifier for a record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'WILDLIFE_RECORD_ID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifier for SUBMISSION OBJECT.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'SUBMISSION_OBJECT_ID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifier for VALIDATION STATUS.  Indicates the overall status of the submitted row of data.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'VALIDATION_STATUS_ID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Identifies the type of record.  WARS = W / Allowed Values: W' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'RECORD_TYPE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The Ministry Contract Service Area number in which the incident occured.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'SERVICE_AREA' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date of accident. ' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'ACCIDENT_DATE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'General  light conditions at time the incident occured. (eg: 1=Dawn, 2=Dusk)
' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'TIME_OF_KILL' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The Y (northing) portion of the accident coordinate. Coordinate is to be reported using the WGS84 datum.
' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'LATITUDE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The X (easting)  portion of the accident coordinate. Coordinate is to be reported using the WGS84 datum.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'LONGITUDE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'This identifies the section of road on which the incident occurred.
This is a value in the in the format:
[Service Area]-[area manager area]-[subarea]-[highway number]
This reference number reflects a valid reference in the road network (currenltyRFI within  CHRIS as of 2019)' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'HIGHWAY_UNIQUE_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Highway reference point (HRP) landmark.  This reference number reflects a valid landmark in the asset management system (currenlty CHRIS as of 2019)' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'LANDMARK' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'This field is needed for linear referencing for location specific reports. 
Offset from beginning of segment.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'START_OFFSET' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Name of nearest town to wildlife accident' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'NEAREST_TOWN' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Is Wildlife sign within 100m (Y, N or Unknown)' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'WILDLIFE_SIGN' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Number of animals injured or killed' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'QUANTITY' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifier for animal species. (eg: 2 = Moose)' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'SPECIES' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifer for sex of involved animal.  Allowed values: M, F, U' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'SEX' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifer for age range of involved animal.  (eg: A=Adult, Y=Young,U=unknown) ' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'AGE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Text field for comments and/or notes pertinent to the specified occurance.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'COMMENT' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'CONCURRENCY_CONTROL_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'APP_CREATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time of record creation' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'APP_CREATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'APP_CREATE_USER_GUID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Active Directory which retains source of truth for user idenifiers.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'APP_CREATE_USER_DIRECTORY' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time of last record update' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_USER_GUID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Active Directory which retains source of truth for user idenifiers.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_USER_DIRECTORY' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Named database user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'DB_AUDIT_CREATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time record created in the database' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'DB_AUDIT_CREATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Named database user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'DB_AUDIT_LAST_UPDATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time record was last updated in the database.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WILDLIFE_REPORT' , 'COLUMN' , 'DB_AUDIT_LAST_UPDATE_TIMESTAMP' 
GO

-- HMR_WORK_REPORT reinstate descriptions


EXEC sp_addextendedproperty 'MS_Description' , 'Submission data regarding maintenance activities is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' 
GO



-- EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifier for a record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'WORK_REPORT_ID' 
-- GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifier for SUBMISSION OBJECT.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'SUBMISSION_OBJECT_ID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifier for VALIDATION STATUS.  Indicates the overall status of the submitted row of data.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'VALIDATION_STATUS_ID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'This field describes the type of work the associated record is reporting on.
This is restricted to specific set of values -
Q - Quantified,
R - Routine,
E - Major Event,
A - Additional' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'RECORD_TYPE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The Ministry Contract Service Area number in which the activity occured.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'SERVICE_AREA' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique work report record number from the maintainence contractor.
This is uniquely identifies each record submission for a contractor.
<Service Area><Record Number> will uniquely identify each record in the application for a particular contractor.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'RECORD_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Contractor Task Number' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'TASK_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Code which uniquely identifies the activity performed.  The number reflects a a classificaiton hierarchy comprised of three levels: ABBCCC

A - the first digit represents Specification Category (eg:2 - Drainage )
BB - the second two digits represent Activity Category (eg: 02 - Drainage Appliance Maintenance)
CCC - the last three digits represent Activity Type and Detail (eg: 310 - Boring, Augering.  300 series reflects Quantified value, which would be linear meters in this case.)' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'ACTIVITY_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date when work commenced' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'START_DATE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date when work was completed' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'END_DATE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The number of units of work completed for the activity corresponding to the activity number.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'ACCOMPLISHMENT' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The code which represents the unit of measure for the specified activity. ' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'UNIT_OF_MEASURE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date the data is posted into the contractor management system' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'POSTED_DATE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'This identifies the section of road on which the activity occurred.
This is a value in the in the format:
[Service Area]-[area manager area]-[subarea]-[highway number]
This should be a value found in RFI (CHRIS)' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'HIGHWAY_UNIQUE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'This field needed for location reference:
Landmarks provided should be those listed in the CHRIS HRP report for each Highway or road within the Service Area' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'LANDMARK' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'This field is needed for linear referencing for location specific reports. 
Offset from beginning of segment.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'START_OFFSET' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'This field is needed for linear referencing for location specific reports.
If the work is less than 30 m, this field is not mandatory
Offset from beginning of segment' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'END_OFFSET' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The M (northing) portion of the activity start coordinate. Specified as a latitude in decimal degrees with six decimal places of precision.
Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum.
' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'START_LATITUDE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The X (easting) portion of the activity start coordinate. Specified as a longitude in decimal degrees with six decimal places of precision.
Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'START_LONGITUDE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The M (northing) portion of the activity end coordinate. Specified as a latitude in decimal degrees with six decimal places of precision.
Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum.
For point activity if this field is not provided it can be defaulted to same as START LATITUDE' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'END_LATITUDE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'The X (easting) portion of the activity end coordinate. Specified as a longitude in decimal degrees with six decimal places of precision.
Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum.
For point activity if this field is not provided it can be defaulted to same as START LONGITUDE.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'END_LONGITUDE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'From list of Bridge Structure Road (BSR) structures provided by the Province.
Is only applicable at defined BSR structures.  BSR structures include; bridges, culverts over 3m, retaining walls.
' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'STRUCTURE_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Contains a site type code followed by a The Ministry site number. Site types are provided by the Province, are four to six digits preceded by:
A – Avalanche
B – Arrestor Bed/Dragnet Barrier
D – Debris and/or Rockfall
L – Landscape
R – Rest Area
S – Signalized Intersection
T – Traffic Patrol
W – Weather Station
X – Railway Crossing' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'SITE_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Total dollar value of the work activity being reported, for each activity.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'VALUE_OF_WORK ' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Text field for comments and/or notes pertinent to the specified activity.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'COMMENTS' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'CONCURRENCY_CONTROL_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'APP_CREATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time of record creation' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'APP_CREATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'APP_CREATE_USER_GUID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Active Directory which retains source of truth for user idenifiers.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'APP_CREATE_USER_DIRECTORY' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time of last record update' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_USER_GUID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Active Directory which retains source of truth for user idenifiers.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'APP_LAST_UPDATE_USER_DIRECTORY' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Named database user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'DB_AUDIT_CREATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time record created in the database' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'DB_AUDIT_CREATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Named database user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'DB_AUDIT_LAST_UPDATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time record was last updated in the database.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'DB_AUDIT_LAST_UPDATE_TIMESTAMP' 
GO

-- HMR_SUBMISSION_ROW reinstate descriptions

EXEC sp_addextendedproperty 'MS_Description' , 'Each row of data within a  SUBMISSION OBJECT for each file submission that  passes basic file validation.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifier for a SUBMISSION OBJECT record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'SUBMISSION_OBJECT_ID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique work report record number from the maintainence contractor.
This is uniquely identifies each record submission for a contractor.
<Service Area><Record Number> will uniquely identify each record in the application for a particular contractor.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'RECORD_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Contains a complete row of submitted data, including delimiters (ie: comma) and text qualifiers (ie: quote).  The row value is used to queue data for validation and loading.  This is staged data used to queue and compare data before loading it within the appropriate tables for reporting.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'ROW_VALUE' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Cryptographic hash for each row of data received. The hash total is is used to compared with subsequently submitted data to check for duplicate submissions. If a match exists, newly matched data is not processed further.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'ROW_HASH' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'CONCURRENCY_CONTROL_NUMBER' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'APP_CREATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time of record creation' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'APP_CREATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'APP_CREATE_USER_GUID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Active Directory which retains source of truth for user idenifiers.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'APP_CREATE_USER_DIRECTORY' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'APP_LAST_UPDATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time of last record update' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'APP_LAST_UPDATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique idenifier of user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'APP_LAST_UPDATE_USER_GUID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Active Directory which retains source of truth for user idenifiers.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'APP_LAST_UPDATE_USER_DIRECTORY' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Named database user who created record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'DB_AUDIT_CREATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time record created in the database' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'DB_AUDIT_CREATE_TIMESTAMP' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Named database user who last updated record' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'DB_AUDIT_LAST_UPDATE_USERID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Date and time record was last updated in the database.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'DB_AUDIT_LAST_UPDATE_TIMESTAMP'

GO