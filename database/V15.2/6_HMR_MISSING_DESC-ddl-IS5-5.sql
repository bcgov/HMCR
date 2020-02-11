
-- =============================================
-- Author:		Ben Driver
-- Create date: 2020-02-11
-- Updates: 
--	
-- 
-- Description:	Implementing missing field descriptions.
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO


-- HMR missing descriptions

EXEC sp_addextendedproperty 'MS_Description' , 'Full listing of validation errors in JSON format for the submitted row.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'ERROR_DETAIL' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'Unique identifier relecting the current status of the submission row.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'ROW_STATUS_ID' 
GO




EXEC sp_addextendedproperty 'MS_Description' , 'Descriptive name of the activity.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ACTIVITY_CODE' , 'COLUMN' , 'ACTIVITY_NAME' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'A system generated unique identifier.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_WORK_REPORT' , 'COLUMN' , 'WORK_REPORT_ID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'A system generated unique identifier.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SUBMISSION_ROW' , 'COLUMN' , 'ROW_ID' 
GO



EXEC sp_addextendedproperty 'MS_Description' , 'A system generated unique identifier.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_SYSTEM_VALIDATION' , 'COLUMN' , 'SYSTEM_VALIDATION_ID' 
GO


EXEC sp_addextendedproperty 'MS_Description' , 'A system generated unique identifier.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_STREAM_ELEMENT' , 'COLUMN' , 'SUBMISSION_STREAM_ID' 
GO

EXEC sp_addextendedproperty 'MS_Description' , 'A system generated unique identifier.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'ROCKFALL_REPORT_ID' 
GO


EXEC sp_addextendedproperty 'MS_Description' , 'Highway reference point (HRP) landmark.  This reference name reflects a valid landmark in the infrastructure asset management system (currenlty CHRIS as of 2019)' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'LANDMARK_NAME' 
GO

EXEC sp_addextendedproperty 'MS_Description' , 'Alpha identifier for a Rockfall report submission.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'RECORD_TYPE' 
GO

EXEC sp_addextendedproperty 'MS_Description' , 'Ditch volume total when the estimated volume in the ditch exceeds 5.0 m3.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'OTHER_DITCH_VOLUME' 
GO

EXEC sp_addextendedproperty 'MS_Description' , 'Travelled lanes volume total when the estimated volume in traveled lanes exceeds 5.0 m3.' , 'USER' , 'dbo' , 'TABLE' , 'HMR_ROCKFALL_REPORT' , 'COLUMN' , 'OTHER_TRAVELLED_LANES_VOLUME' 
GO

