-- Script to update the activity code rules in the HMRS database.

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

-- Surface Type
-- Update Surface Type Rule to 'GPS Not on Unconstructed'
UPDATE code
SET    code.SURFACE_TYPE_RULE          = nwrl.ACTIVITY_CODE_RULE_ID 
     , code.CONCURRENCY_CONTROL_NUMBER = code.CONCURRENCY_CONTROL_NUMBER + 1
FROM   [dbo].[HMR_ACTIVITY_CODE]      code
  JOIN [dbo].[HMR_LOCATION_CODE]      locn ON locn.LOCATION_CODE_ID      = code.LOCATION_CODE_ID
  JOIN [dbo].[HMR_ACTIVITY_CODE_RULE] cdrl ON cdrl.ACTIVITY_CODE_RULE_ID = code.SURFACE_TYPE_RULE
  JOIN [dbo].[HMR_ACTIVITY_CODE_RULE] nwrl ON nwrl.ACTIVITY_RULE_SET     = 'SURFACE_TYPE'
                                          AND nwrl.ACTIVITY_RULE_NAME    = 'GPS Not on Unconstructed'
WHERE  code.END_DATE           IS NULL
   AND locn.LOCATION_CODE      =  'C'
   AND cdrl.ACTIVITY_RULE_SET  =  'SURFACE_TYPE'
   AND cdrl.ACTIVITY_RULE_NAME =  'Not Applicable'
GO

-- Road Class
-- Update Road Class Rule to 'Not Class 8 or F'
UPDATE code
SET    code.ROAD_CLASS_RULE            = nwrl.ACTIVITY_CODE_RULE_ID 
     , code.CONCURRENCY_CONTROL_NUMBER = code.CONCURRENCY_CONTROL_NUMBER + 1
FROM   [dbo].[HMR_ACTIVITY_CODE]      code
  JOIN [dbo].[HMR_LOCATION_CODE]      locn ON locn.LOCATION_CODE_ID      = code.LOCATION_CODE_ID
  JOIN [dbo].[HMR_ACTIVITY_CODE_RULE] cdrl ON cdrl.ACTIVITY_CODE_RULE_ID = code.ROAD_CLASS_RULE
  JOIN [dbo].[HMR_ACTIVITY_CODE_RULE] nwrl ON nwrl.ACTIVITY_RULE_SET     = 'ROAD_CLASS'
                                          AND nwrl.ACTIVITY_RULE_NAME    = 'Not Class 8 or F'
WHERE  code.END_DATE           IS NULL
   AND locn.LOCATION_CODE      =  'C'
   AND cdrl.ACTIVITY_RULE_SET  =  'ROAD_CLASS'
   AND cdrl.ACTIVITY_RULE_NAME =  'Not Applicable'
GO
