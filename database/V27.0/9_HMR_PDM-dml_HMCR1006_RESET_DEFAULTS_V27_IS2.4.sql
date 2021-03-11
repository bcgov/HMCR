-- *****************************************************************************
-- Script to update default 0 values to their 'Not Allicable' settivalue, 
-- relative to their rule type.
 -- *****************************************************************************

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

SELECT 'ROAD_CLASS_RULE', COUNT(*) AS 'Row Count'
FROM   HMR_ACTIVITY_CODE
WHERE  ROAD_CLASS_RULE = 0
UNION
SELECT 'ROAD_LENGTH_RULE', COUNT(*)
FROM   HMR_ACTIVITY_CODE
WHERE  ROAD_LENGTH_RULE = 0
UNION
SELECT 'SURFACE_TYPE_RULE', COUNT(*)
FROM   HMR_ACTIVITY_CODE
WHERE  SURFACE_TYPE_RULE = 0

-- Set all 0-value ROAD_CLASS_RULE columns to their 'Not Applicable' value
UPDATE code
SET    code.ROAD_CLASS_RULE            = nwrl.ACTIVITY_CODE_RULE_ID
     , code.CONCURRENCY_CONTROL_NUMBER = code.CONCURRENCY_CONTROL_NUMBER + 1
FROM   [dbo].[HMR_ACTIVITY_CODE]      code
  JOIN [dbo].[HMR_ACTIVITY_CODE_RULE] nwrl ON nwrl.ACTIVITY_RULE_SET     = 'ROAD_CLASS'
                                          AND nwrl.ACTIVITY_RULE_NAME    = 'Not Applicable'
WHERE  code.ROAD_CLASS_RULE = 0
GO

-- Set all 0-value ROAD_LENGTH_RULE columns to their 'Not Applicable' value
UPDATE code
SET    code.ROAD_LENGTH_RULE           = nwrl.ACTIVITY_CODE_RULE_ID
     , code.CONCURRENCY_CONTROL_NUMBER = code.CONCURRENCY_CONTROL_NUMBER + 1
FROM   [dbo].[HMR_ACTIVITY_CODE]      code
  JOIN [dbo].[HMR_ACTIVITY_CODE_RULE] nwrl ON nwrl.ACTIVITY_RULE_SET     = 'ROAD_LENGTH'
                                          AND nwrl.ACTIVITY_RULE_NAME    = 'Not Applicable'
WHERE  code.ROAD_LENGTH_RULE = 0
GO

-- Set all 0-value SURFACE_TYPE_RULE columns to their 'Not Applicable' value
UPDATE code
SET    code.SURFACE_TYPE_RULE          = nwrl.ACTIVITY_CODE_RULE_ID
     , code.CONCURRENCY_CONTROL_NUMBER = code.CONCURRENCY_CONTROL_NUMBER + 1
FROM   [dbo].[HMR_ACTIVITY_CODE]      code
  JOIN [dbo].[HMR_ACTIVITY_CODE_RULE] nwrl ON nwrl.ACTIVITY_RULE_SET     = 'SURFACE_TYPE'
                                          AND nwrl.ACTIVITY_RULE_NAME    = 'Not Applicable'
WHERE  code.SURFACE_TYPE_RULE = 0
GO

SELECT 'ROAD_CLASS_RULE', COUNT(*) AS 'Row Count'
FROM   HMR_ACTIVITY_CODE
WHERE  ROAD_CLASS_RULE = 0
UNION
SELECT 'ROAD_LENGTH_RULE', COUNT(*)
FROM   HMR_ACTIVITY_CODE
WHERE  ROAD_LENGTH_RULE = 0
UNION
SELECT 'SURFACE_TYPE_RULE', COUNT(*)
FROM   HMR_ACTIVITY_CODE
WHERE  SURFACE_TYPE_RULE = 0
GO
