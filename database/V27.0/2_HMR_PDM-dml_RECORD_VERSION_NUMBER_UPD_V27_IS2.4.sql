-- *****************************************************************************
-- Script to initially load the RECORD_VERSION_NUMBER field from the contents of
-- the CONCURRENCY_CONTROL_NUMBER in the following tables:
-- * HMR_ROCKFALL_REPORT
-- * HMR_ROCKFALL_REPORT_HIST
-- * HMR_WORK_REPORT
-- * HMR_WORK_REPORT_HIST
-- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-- Date         Author        Comment
-- -----------  ------------  --------------------------------------------------
-- 2020-Nov-16  Doug Filteau  Initial Version
-- *****************************************************************************

USE HMR_DEV
GO

-- *****************************************************************************
-- Update the RECORD_VERSION_NUMBER column on the HMR_ROCKFALL_REPORT table
-- *****************************************************************************
UPDATE [dbo].[HMR_ROCKFALL_REPORT]
SET    RECORD_VERSION_NUMBER = CONCURRENCY_CONTROL_NUMBER,
 CONCURRENCY_CONTROL_NUMBER =  CONCURRENCY_CONTROL_NUMBER+1
GO

-- *****************************************************************************
-- Update the RECORD_VERSION_NUMBER column on the HMR_ROCKFALL_REPORT_HIST table
-- *****************************************************************************
UPDATE [dbo].[HMR_ROCKFALL_REPORT_HIST]
SET    RECORD_VERSION_NUMBER = CONCURRENCY_CONTROL_NUMBER,
 CONCURRENCY_CONTROL_NUMBER =  CONCURRENCY_CONTROL_NUMBER+1
GO

-- *****************************************************************************
-- Update the RECORD_VERSION_NUMBER column on the HMR_WORK_REPORT table
-- *****************************************************************************
UPDATE [dbo].[HMR_WORK_REPORT]
SET    RECORD_VERSION_NUMBER = CONCURRENCY_CONTROL_NUMBER,
 CONCURRENCY_CONTROL_NUMBER =  CONCURRENCY_CONTROL_NUMBER+1
GO

-- *****************************************************************************
-- Update the RECORD_VERSION_NUMBER column on the HMR_WORK_REPORT_HIST table
-- *****************************************************************************
UPDATE [dbo].[HMR_WORK_REPORT_HIST]
SET    RECORD_VERSION_NUMBER = CONCURRENCY_CONTROL_NUMBER,
 CONCURRENCY_CONTROL_NUMBER =  CONCURRENCY_CONTROL_NUMBER+1
GO
