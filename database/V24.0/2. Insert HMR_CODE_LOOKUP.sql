// **************************************************************
// Insert the Activity Rule codes into the HMR_CODE_LOOKUP table.
// **************************************************************

INSERT INTO [dbo].[HMR_CODE_LOOKUP]([CODE_SET], [CODE_NAME], [CODE_VALUE_TEXT], [CODE_VALUE_FORMAT]) VALUES('ACTIVITY_RULE', 'ROAD_LENGTH', 'Road Length', 'STRING');
GO
INSERT INTO [dbo].[HMR_CODE_LOOKUP]([CODE_SET], [CODE_NAME], [CODE_VALUE_TEXT], [CODE_VALUE_FORMAT]) VALUES('ACTIVITY_RULE', 'SURFACE_TYPE', 'Surface Type', 'STRING');
GO
INSERT INTO [dbo].[HMR_CODE_LOOKUP]([CODE_SET], [CODE_NAME], [CODE_VALUE_TEXT], [CODE_VALUE_FORMAT]) VALUES('ACTIVITY_RULE', 'ROAD_CLASS', 'Road Class', 'STRING');
GO
