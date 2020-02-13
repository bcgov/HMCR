-- =============================================
-- Author:		Ben Driver
-- Create date: 2019-11-13
-- Updates: 2020-01-03 - updated to correct case inconsistancies (BD)
-- Updates: 2020-01-02 - updated to reflect changes for Spring 3 & 4. (BD)
-- Updates: 2019-11-27 - updates in support of sprint 2 & 3. (BD)
-- 
-- Description:	T-SQL generated Triggers for DB_LAST_UPDATE% columns.  All dates set to UTC time, to be consistent throughout the database.
-- =============================================

USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO


CREATE TRIGGER HMR_ACT_CODE_I_S_U_TR ON HMR_ACTIVITY_CODE INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.ACTIVITY_CODE_ID = deleted.ACTIVITY_CODE_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_ACTIVITY_CODE
    set "ACTIVITY_CODE_ID" = inserted."ACTIVITY_CODE_ID",
      "ACTIVITY_NUMBER" = inserted."ACTIVITY_NUMBER",
      "ACTIVITY_NAME" = inserted."ACTIVITY_NAME",
      "UNIT_OF_MEASURE" = inserted."UNIT_OF_MEASURE",
      "MAINTENANCE_TYPE" = inserted."MAINTENANCE_TYPE",
      "LOCATION_CODE_ID" = inserted."LOCATION_CODE_ID",
      "POINT_LINE_FEATURE" = inserted."POINT_LINE_FEATURE",
      "ACTIVITY_APPLICATION" = inserted."ACTIVITY_APPLICATION",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_ACTIVITY_CODE
    inner join inserted
    on (HMR_ACTIVITY_CODE.ACTIVITY_CODE_ID = inserted.ACTIVITY_CODE_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_CODE_LKUP_I_S_U_TR ON HMR_CODE_LOOKUP INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.CODE_LOOKUP_ID = deleted.CODE_LOOKUP_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_CODE_LOOKUP
    set "CODE_LOOKUP_ID" = inserted."CODE_LOOKUP_ID",
      "CODE_SET" = inserted."CODE_SET",
      "CODE_NAME" = inserted."CODE_NAME",
      "CODE_VALUE_TEXT" = inserted."CODE_VALUE_TEXT",
      "CODE_VALUE_NUM" = inserted."CODE_VALUE_NUM",
      "CODE_VALUE_FORMAT" = inserted."CODE_VALUE_FORMAT",
      "DISPLAY_ORDER" = inserted."DISPLAY_ORDER",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_CODE_LOOKUP
    inner join inserted
    on (HMR_CODE_LOOKUP.CODE_LOOKUP_ID = inserted.CODE_LOOKUP_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_CNRT_TRM_I_S_U_TR ON HMR_CONTRACT_TERM INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.CONTRACT_TERM_ID = deleted.CONTRACT_TERM_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_CONTRACT_TERM
    set "CONTRACT_TERM_ID" = inserted."CONTRACT_TERM_ID",
      "CONTRACT_NAME" = inserted."CONTRACT_NAME",
      "PARTY_ID" = inserted."PARTY_ID",
      "SERVICE_AREA_NUMBER" = inserted."SERVICE_AREA_NUMBER",
      "START_DATE" = inserted."START_DATE",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_CONTRACT_TERM
    inner join inserted
    on (HMR_CONTRACT_TERM.CONTRACT_TERM_ID = inserted.CONTRACT_TERM_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_DIST_I_S_U_TR ON HMR_DISTRICT INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.DISTRICT_NUMBER = deleted.DISTRICT_NUMBER)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_DISTRICT
    set "DISTRICT_ID" = inserted."DISTRICT_ID",
      "DISTRICT_NUMBER" = inserted."DISTRICT_NUMBER",
      "DISTRICT_NAME" = inserted."DISTRICT_NAME",
      "REGION_NUMBER" = inserted."REGION_NUMBER",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_DISTRICT
    inner join inserted
    on (HMR_DISTRICT.DISTRICT_NUMBER = inserted.DISTRICT_NUMBER);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER FM5_I_S_U_TR ON HMR_FEEDBACK_MESSAGE INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.FEEDBACK_MESSAGE_ID = deleted.FEEDBACK_MESSAGE_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_FEEDBACK_MESSAGE
    set "FEEDBACK_MESSAGE_ID" = inserted."FEEDBACK_MESSAGE_ID",
      "SUBMISSION_OBJECT_ID" = inserted."SUBMISSION_OBJECT_ID",
      "COMMUNICATION_SUBJECT" = inserted."COMMUNICATION_SUBJECT",
      "COMMUNICATION_TEXT" = inserted."COMMUNICATION_TEXT",
      "COMMUNICATION_DATE" = inserted."COMMUNICATION_DATE",
      "IS_SENT" = inserted."IS_SENT",
      "IS_ERROR" = inserted."IS_ERROR",
      "SEND_ERROR_TEXT" = inserted."SEND_ERROR_TEXT",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_FEEDBACK_MESSAGE
    inner join inserted
    on (HMR_FEEDBACK_MESSAGE.FEEDBACK_MESSAGE_ID = inserted.FEEDBACK_MESSAGE_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_LOC_CODE_I_S_U_TR ON HMR_LOCATION_CODE INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.LOCATION_CODE_ID = deleted.LOCATION_CODE_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_LOCATION_CODE
    set "LOCATION_CODE_ID" = inserted."LOCATION_CODE_ID",
      "LOCATION_CODE" = inserted."LOCATION_CODE",
      "REQUIRED_LOCATION_DETAILS" = inserted."REQUIRED_LOCATION_DETAILS",
      "ADDITIONAL_INFO" = inserted."ADDITIONAL_INFO",
      "REPORTING_FREQUENCY" = inserted."REPORTING_FREQUENCY",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_LOCATION_CODE
    inner join inserted
    on (HMR_LOCATION_CODE.LOCATION_CODE_ID = inserted.LOCATION_CODE_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER MT7_I_S_U_TR ON HMR_MIME_TYPE INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.MIME_TYPE_ID = deleted.MIME_TYPE_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_MIME_TYPE
    set "MIME_TYPE_ID" = inserted."MIME_TYPE_ID",
      "MIME_TYPE_CODE" = inserted."MIME_TYPE_CODE",
      "DESCRIPTION" = inserted."DESCRIPTION",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_MIME_TYPE
    inner join inserted
    on (HMR_MIME_TYPE.MIME_TYPE_ID = inserted.MIME_TYPE_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_PRTY_I_S_U_TR ON HMR_PARTY INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.PARTY_ID = deleted.PARTY_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_PARTY
    set "PARTY_ID" = inserted."PARTY_ID",
      "BUSINESS_GUID" = inserted."BUSINESS_GUID",
      "BUSINESS_LEGAL_NAME" = inserted."BUSINESS_LEGAL_NAME",
      "DISPLAY_NAME" = inserted."DISPLAY_NAME",
      "EMAIL" = inserted."EMAIL",
      "TELEPHONE" = inserted."TELEPHONE",
      "BUSINESS_NUMBER" = inserted."BUSINESS_NUMBER",
      "PARTY_TYPE" = inserted."PARTY_TYPE",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_PARTY
    inner join inserted
    on (HMR_PARTY.PARTY_ID = inserted.PARTY_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_PERM_I_S_U_TR ON HMR_PERMISSION INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.PERMISSION_ID = deleted.PERMISSION_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_PERMISSION
    set "PERMISSION_ID" = inserted."PERMISSION_ID",
      "NAME" = inserted."NAME",
      "DESCRIPTION" = inserted."DESCRIPTION",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_PERMISSION
    inner join inserted
    on (HMR_PERMISSION.PERMISSION_ID = inserted.PERMISSION_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_RGN_I_S_U_TR ON HMR_REGION INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.REGION_NUMBER = deleted.REGION_NUMBER)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_REGION
    set "REGION_ID" = inserted."REGION_ID",
      "REGION_NUMBER" = inserted."REGION_NUMBER",
      "REGION_NAME" = inserted."REGION_NAME",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_REGION
    inner join inserted
    on (HMR_REGION.REGION_NUMBER = inserted.REGION_NUMBER);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_RCKFL_RPT_I_S_U_TR ON HMR_ROCKFALL_REPORT INSTEAD OF UPDATE AS
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
      "MAJOR_INCIDENT_NUMBER" = inserted."MAJOR_INCIDENT_NUMBER",
      "ESTIMATED_ROCKFALL_DATE" = inserted."ESTIMATED_ROCKFALL_DATE",
      "ESTIMATED_ROCKFALL_TIME" = inserted."ESTIMATED_ROCKFALL_TIME",
      "START_LATITUDE" = inserted."START_LATITUDE",
      "START_LONGITUDE" = inserted."START_LONGITUDE",
      "END_LATITUDE" = inserted."END_LATITUDE",
      "END_LONGITUDE" = inserted."END_LONGITUDE",
      "HIGHWAY_UNIQUE_NUMBER" = inserted."HIGHWAY_UNIQUE_NUMBER",
      "HIGHWAY_UNIQUE_NAME" = inserted."HIGHWAY_UNIQUE_NAME",
      "LANDMARK" = inserted."LANDMARK",
      "LAND_MARK_NAME" = inserted."LAND_MARK_NAME",
      "START_OFFSET" = inserted."START_OFFSET",
      "END_OFFSET" = inserted."END_OFFSET",
      "DIRECTION_FROM_LANDMARK" = inserted."DIRECTION_FROM_LANDMARK",
      "LOCATION_DESCRIPTION" = inserted."LOCATION_DESCRIPTION",
      "DITCH_VOLUME" = inserted."DITCH_VOLUME",
      "TRAVELLED_LANES_VOLUME" = inserted."TRAVELLED_LANES_VOLUME",
      "OTHER_VOLUME" = inserted."OTHER_VOLUME",
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
go

CREATE TRIGGER HMR_RL_I_S_U_TR ON HMR_ROLE INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.ROLE_ID = deleted.ROLE_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_ROLE
    set "ROLE_ID" = inserted."ROLE_ID",
      "NAME" = inserted."NAME",
      "DESCRIPTION" = inserted."DESCRIPTION",
      "IS_INTERNAL" = inserted."IS_INTERNAL",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_ROLE
    inner join inserted
    on (HMR_ROLE.ROLE_ID = inserted.ROLE_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_RL_PERM_I_S_U_TR ON HMR_ROLE_PERMISSION INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.ROLE_PERMISSION_ID = deleted.ROLE_PERMISSION_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_ROLE_PERMISSION
    set "ROLE_PERMISSION_ID" = inserted."ROLE_PERMISSION_ID",
      "ROLE_ID" = inserted."ROLE_ID",
      "PERMISSION_ID" = inserted."PERMISSION_ID",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_ROLE_PERMISSION
    inner join inserted
    on (HMR_ROLE_PERMISSION.ROLE_PERMISSION_ID = inserted.ROLE_PERMISSION_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_SRV_ARA_I_S_U_TR ON HMR_SERVICE_AREA INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.SERVICE_AREA_NUMBER = deleted.SERVICE_AREA_NUMBER)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SERVICE_AREA
    set "SERVICE_AREA_ID" = inserted."SERVICE_AREA_ID",
      "SERVICE_AREA_NUMBER" = inserted."SERVICE_AREA_NUMBER",
      "SERVICE_AREA_NAME" = inserted."SERVICE_AREA_NAME",
      "DISTRICT_NUMBER" = inserted."DISTRICT_NUMBER",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SERVICE_AREA
    inner join inserted
    on (HMR_SERVICE_AREA.SERVICE_AREA_NUMBER = inserted.SERVICE_AREA_NUMBER);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_SRV_ARA_USR_I_S_U_TR ON HMR_SERVICE_AREA_USER INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.SERVICE_AREA_USER_ID = deleted.SERVICE_AREA_USER_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SERVICE_AREA_USER
    set "SERVICE_AREA_USER_ID" = inserted."SERVICE_AREA_USER_ID",
      "SERVICE_AREA_NUMBER" = inserted."SERVICE_AREA_NUMBER",
      "SYSTEM_USER_ID" = inserted."SYSTEM_USER_ID",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SERVICE_AREA_USER
    inner join inserted
    on (HMR_SERVICE_AREA_USER.SERVICE_AREA_USER_ID = inserted.SERVICE_AREA_USER_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_STR_ELMT_I_S_U_TR ON HMR_STREAM_ELEMENT INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.STREAM_ELEMENT_ID = deleted.STREAM_ELEMENT_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_STREAM_ELEMENT
    set "STREAM_ELEMENT_ID" = inserted."STREAM_ELEMENT_ID",
      "SUBMISSION_STREAM_ID" = inserted."SUBMISSION_STREAM_ID",
      "ELEMENT_NAME" = inserted."ELEMENT_NAME",
      "ELEMENT_TYPE" = inserted."ELEMENT_TYPE",
      "STAGING_COLUMN_NAME" = inserted."STAGING_COLUMN_NAME",
      "IS_REQUIRED" = inserted."IS_REQUIRED",
      "MAX_LENGTH" = inserted."MAX_LENGTH",
      "MIN_LENGTH" = inserted."MIN_LENGTH",
      "MAX_VALUE" = inserted."MAX_VALUE",
      "MIN_VALUE" = inserted."MIN_VALUE",
      "MAX_DATE" = inserted."MAX_DATE",
      "MIN_DATE" = inserted."MIN_DATE",
      "REG_EXP" = inserted."REG_EXP",
      "CODE_SET" = inserted."CODE_SET",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_STREAM_ELEMENT
    inner join inserted
    on (HMR_STREAM_ELEMENT.STREAM_ELEMENT_ID = inserted.STREAM_ELEMENT_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_SUBM_OBJ_I_S_U_TR ON HMR_SUBMISSION_OBJECT INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.SUBMISSION_OBJECT_ID = deleted.SUBMISSION_OBJECT_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SUBMISSION_OBJECT
    set "SUBMISSION_OBJECT_ID" = inserted."SUBMISSION_OBJECT_ID",
      "FILE_NAME" = inserted."FILE_NAME",
      "DIGITAL_REPRESENTATION" = inserted."DIGITAL_REPRESENTATION",
      "MIME_TYPE_ID" = inserted."MIME_TYPE_ID",
      "SUBMISSION_STATUS_ID" = inserted."SUBMISSION_STATUS_ID",
      "SERVICE_AREA_NUMBER" = inserted."SERVICE_AREA_NUMBER",
      "PARTY_ID" = inserted."PARTY_ID",
      "FILE_HASH" = inserted."FILE_HASH",
      "ERROR_DETAIL" = inserted."ERROR_DETAIL",
      "SUBMISSION_STREAM_ID" = inserted."SUBMISSION_STREAM_ID",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SUBMISSION_OBJECT
    inner join inserted
    on (HMR_SUBMISSION_OBJECT.SUBMISSION_OBJECT_ID = inserted.SUBMISSION_OBJECT_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

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
      "RECORD_NUMBER" = inserted."RECORD_NUMBER",
      "ROW_VALUE" = inserted."ROW_VALUE",
      "ROW_HASH" = inserted."ROW_HASH",
      "ERROR_DETAIL" = inserted."ERROR_DETAIL",
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
go

CREATE TRIGGER SS19_I_S_U_TR ON HMR_SUBMISSION_STATUS INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.STATUS_ID = deleted.STATUS_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SUBMISSION_STATUS
    set "STATUS_ID" = inserted."STATUS_ID",
      "STATUS_CODE" = inserted."STATUS_CODE",
      "DESCRIPTION" = inserted."DESCRIPTION",
      "STATUS_TYPE" = inserted."STATUS_TYPE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SUBMISSION_STATUS
    inner join inserted
    on (HMR_SUBMISSION_STATUS.STATUS_ID = inserted.STATUS_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_SUBM_STR_I_S_U_TR ON HMR_SUBMISSION_STREAM INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.SUBMISSION_STREAM_ID = deleted.SUBMISSION_STREAM_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SUBMISSION_STREAM
    set "SUBMISSION_STREAM_ID" = inserted."SUBMISSION_STREAM_ID",
      "STREAM_NAME" = inserted."STREAM_NAME",
      "END_DATE" = inserted."END_DATE",
      "FILE_SIZE_LIMIT" = inserted."FILE_SIZE_LIMIT",
      "STAGING_TABLE_NAME" = inserted."STAGING_TABLE_NAME",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SUBMISSION_STREAM
    inner join inserted
    on (HMR_SUBMISSION_STREAM.SUBMISSION_STREAM_ID = inserted.SUBMISSION_STREAM_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_SYS_USR_I_S_U_TR ON HMR_SYSTEM_USER INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.SYSTEM_USER_ID = deleted.SYSTEM_USER_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SYSTEM_USER
    set "SYSTEM_USER_ID" = inserted."SYSTEM_USER_ID",
      "PARTY_ID" = inserted."PARTY_ID",
      "USER_GUID" = inserted."USER_GUID",
      "USERNAME" = inserted."USERNAME",
      "USER_DIRECTORY" = inserted."USER_DIRECTORY",
      "USER_TYPE" = inserted."USER_TYPE",
      "FIRST_NAME" = inserted."FIRST_NAME",
      "LAST_NAME" = inserted."LAST_NAME",
      "EMAIL" = inserted."EMAIL",
      "BUSINESS_GUID" = inserted."BUSINESS_GUID",
      "BUSINESS_LEGAL_NAME" = inserted."BUSINESS_LEGAL_NAME",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SYSTEM_USER
    inner join inserted
    on (HMR_SYSTEM_USER.SYSTEM_USER_ID = inserted.SYSTEM_USER_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER SV22_I_S_U_TR ON HMR_SYSTEM_VALIDATION INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.SYSTEM_VALIDATION_ID = deleted.SYSTEM_VALIDATION_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_SYSTEM_VALIDATION
    set "SYSTEM_VALIDATION_ID" = inserted."SYSTEM_VALIDATION_ID",
      "ENTITY_NAME" = inserted."ENTITY_NAME",
      "ATTRIBUTE_NAME" = inserted."ATTRIBUTE_NAME",
      "ATTRIBUTE_TYPE" = inserted."ATTRIBUTE_TYPE",
      "IS_REQUIRED" = inserted."IS_REQUIRED",
      "MAX_LENGTH" = inserted."MAX_LENGTH",
      "MIN_LENGTH" = inserted."MIN_LENGTH",
      "MAX_VALUE" = inserted."MAX_VALUE",
      "MIN_VALUE" = inserted."MIN_VALUE",
      "MAX_DATE" = inserted."MAX_DATE",
      "MIN_DATE" = inserted."MIN_DATE",
      "REG_EXP" = inserted."REG_EXP",
      "CODE_SET" = inserted."CODE_SET",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_SYSTEM_VALIDATION
    inner join inserted
    on (HMR_SYSTEM_VALIDATION.SYSTEM_VALIDATION_ID = inserted.SYSTEM_VALIDATION_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_USR_RL_I_S_U_TR ON HMR_USER_ROLE INSTEAD OF UPDATE AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM deleted) 
    RETURN;

  -- validate concurrency control
  if exists (select 1 from inserted, deleted where inserted.CONCURRENCY_CONTROL_NUMBER != deleted.CONCURRENCY_CONTROL_NUMBER+1 AND inserted.USER_ROLE_ID = deleted.USER_ROLE_ID)
    raiserror('CONCURRENCY FAILURE.',16,1)


  -- update statement
  update HMR_USER_ROLE
    set "USER_ROLE_ID" = inserted."USER_ROLE_ID",
      "ROLE_ID" = inserted."ROLE_ID",
      "SYSTEM_USER_ID" = inserted."SYSTEM_USER_ID",
      "END_DATE" = inserted."END_DATE",
      "CONCURRENCY_CONTROL_NUMBER" = inserted."CONCURRENCY_CONTROL_NUMBER",
      "APP_LAST_UPDATE_USERID" = inserted."APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP" = inserted."APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID" = inserted."APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY" = inserted."APP_LAST_UPDATE_USER_DIRECTORY"
    , DB_AUDIT_LAST_UPDATE_TIMESTAMP = getutcdate()
    , DB_AUDIT_LAST_UPDATE_USERID = user_name()
    from HMR_USER_ROLE
    inner join inserted
    on (HMR_USER_ROLE.USER_ROLE_ID = inserted.USER_ROLE_ID);

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_WLDLF_RPT_I_S_U_TR ON HMR_WILDLIFE_REPORT INSTEAD OF UPDATE AS
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
      "HIGHWAY_UNIQUE_NUMBER" = inserted."HIGHWAY_UNIQUE_NUMBER",
      "LANDMARK" = inserted."LANDMARK",
      "START_OFFSET" = inserted."START_OFFSET",
      "NEAREST_TOWN" = inserted."NEAREST_TOWN",
      "WILDLIFE_SIGN" = inserted."WILDLIFE_SIGN",
      "QUANTITY" = inserted."QUANTITY",
      "SPECIES" = inserted."SPECIES",
      "SEX" = inserted."SEX",
      "AGE" = inserted."AGE",
      "COMMENT" = inserted."COMMENT",
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
go

CREATE TRIGGER HMR_WRK_RPT_I_S_U_TR ON HMR_WORK_REPORT INSTEAD OF UPDATE AS
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
go

CREATE TRIGGER HMR_ACT_CODE_I_S_I_TR ON HMR_ACTIVITY_CODE INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_ACTIVITY_CODE ("ACTIVITY_CODE_ID",
      "ACTIVITY_NUMBER",
      "ACTIVITY_NAME",
      "UNIT_OF_MEASURE",
      "MAINTENANCE_TYPE",
      "LOCATION_CODE_ID",
      "POINT_LINE_FEATURE",
      "ACTIVITY_APPLICATION",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "ACTIVITY_CODE_ID",
      "ACTIVITY_NUMBER",
      "ACTIVITY_NAME",
      "UNIT_OF_MEASURE",
      "MAINTENANCE_TYPE",
      "LOCATION_CODE_ID",
      "POINT_LINE_FEATURE",
      "ACTIVITY_APPLICATION",
      "END_DATE",
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
go

CREATE TRIGGER HMR_CODE_LKUP_I_S_I_TR ON HMR_CODE_LOOKUP INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_CODE_LOOKUP ("CODE_LOOKUP_ID",
      "CODE_SET",
      "CODE_NAME",
      "CODE_VALUE_TEXT",
      "CODE_VALUE_NUM",
      "CODE_VALUE_FORMAT",
      "DISPLAY_ORDER",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER")
    select "CODE_LOOKUP_ID",
      "CODE_SET",
      "CODE_NAME",
      "CODE_VALUE_TEXT",
      "CODE_VALUE_NUM",
      "CODE_VALUE_FORMAT",
      "DISPLAY_ORDER",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_CNRT_TRM_I_S_I_TR ON HMR_CONTRACT_TERM INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_CONTRACT_TERM ("CONTRACT_TERM_ID",
      "CONTRACT_NAME",
      "PARTY_ID",
      "SERVICE_AREA_NUMBER",
      "START_DATE",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "CONTRACT_TERM_ID",
      "CONTRACT_NAME",
      "PARTY_ID",
      "SERVICE_AREA_NUMBER",
      "START_DATE",
      "END_DATE",
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
go

CREATE TRIGGER HMR_DIST_I_S_I_TR ON HMR_DISTRICT INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_DISTRICT ("DISTRICT_ID",
      "DISTRICT_NUMBER",
      "DISTRICT_NAME",
      "REGION_NUMBER",
      "CONCURRENCY_CONTROL_NUMBER")
    select "DISTRICT_ID",
      "DISTRICT_NUMBER",
      "DISTRICT_NAME",
      "REGION_NUMBER",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER FM5_I_S_I_TR ON HMR_FEEDBACK_MESSAGE INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_FEEDBACK_MESSAGE ("FEEDBACK_MESSAGE_ID",
      "SUBMISSION_OBJECT_ID",
      "COMMUNICATION_SUBJECT",
      "COMMUNICATION_TEXT",
      "COMMUNICATION_DATE",
      "IS_SENT",
      "IS_ERROR",
      "SEND_ERROR_TEXT",
      "CONCURRENCY_CONTROL_NUMBER")
    select "FEEDBACK_MESSAGE_ID",
      "SUBMISSION_OBJECT_ID",
      "COMMUNICATION_SUBJECT",
      "COMMUNICATION_TEXT",
      "COMMUNICATION_DATE",
      "IS_SENT",
      "IS_ERROR",
      "SEND_ERROR_TEXT",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_LOC_CODE_I_S_I_TR ON HMR_LOCATION_CODE INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_LOCATION_CODE ("LOCATION_CODE_ID",
      "LOCATION_CODE",
      "REQUIRED_LOCATION_DETAILS",
      "ADDITIONAL_INFO",
      "REPORTING_FREQUENCY",
      "CONCURRENCY_CONTROL_NUMBER")
    select "LOCATION_CODE_ID",
      "LOCATION_CODE",
      "REQUIRED_LOCATION_DETAILS",
      "ADDITIONAL_INFO",
      "REPORTING_FREQUENCY",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER MT7_I_S_I_TR ON HMR_MIME_TYPE INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_MIME_TYPE ("MIME_TYPE_ID",
      "MIME_TYPE_CODE",
      "DESCRIPTION",
      "CONCURRENCY_CONTROL_NUMBER")
    select "MIME_TYPE_ID",
      "MIME_TYPE_CODE",
      "DESCRIPTION",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_PRTY_I_S_I_TR ON HMR_PARTY INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_PARTY ("PARTY_ID",
      "BUSINESS_GUID",
      "BUSINESS_LEGAL_NAME",
      "DISPLAY_NAME",
      "EMAIL",
      "TELEPHONE",
      "BUSINESS_NUMBER",
      "PARTY_TYPE",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "PARTY_ID",
      "BUSINESS_GUID",
      "BUSINESS_LEGAL_NAME",
      "DISPLAY_NAME",
      "EMAIL",
      "TELEPHONE",
      "BUSINESS_NUMBER",
      "PARTY_TYPE",
      "END_DATE",
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
go

CREATE TRIGGER HMR_PERM_I_S_I_TR ON HMR_PERMISSION INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_PERMISSION ("PERMISSION_ID",
      "NAME",
      "DESCRIPTION",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "PERMISSION_ID",
      "NAME",
      "DESCRIPTION",
      "END_DATE",
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
go

CREATE TRIGGER HMR_RGN_I_S_I_TR ON HMR_REGION INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_REGION ("REGION_ID",
      "REGION_NUMBER",
      "REGION_NAME",
      "CONCURRENCY_CONTROL_NUMBER")
    select "REGION_ID",
      "REGION_NUMBER",
      "REGION_NAME",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_RCKFL_RPT_I_S_I_TR ON HMR_ROCKFALL_REPORT INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_ROCKFALL_REPORT ("ROCKFALL_REPORT_ID",
      "SUBMISSION_OBJECT_ID",
      "VALIDATION_STATUS_ID",
      "MAJOR_INCIDENT_NUMBER",
      "ESTIMATED_ROCKFALL_DATE",
      "ESTIMATED_ROCKFALL_TIME",
      "START_LATITUDE",
      "START_LONGITUDE",
      "END_LATITUDE",
      "END_LONGITUDE",
      "HIGHWAY_UNIQUE_NUMBER",
      "HIGHWAY_UNIQUE_NAME",
      "LANDMARK",
      "LAND_MARK_NAME",
      "START_OFFSET",
      "END_OFFSET",
      "DIRECTION_FROM_LANDMARK",
      "LOCATION_DESCRIPTION",
      "DITCH_VOLUME",
      "TRAVELLED_LANES_VOLUME",
      "OTHER_VOLUME",
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
      "MAJOR_INCIDENT_NUMBER",
      "ESTIMATED_ROCKFALL_DATE",
      "ESTIMATED_ROCKFALL_TIME",
      "START_LATITUDE",
      "START_LONGITUDE",
      "END_LATITUDE",
      "END_LONGITUDE",
      "HIGHWAY_UNIQUE_NUMBER",
      "HIGHWAY_UNIQUE_NAME",
      "LANDMARK",
      "LAND_MARK_NAME",
      "START_OFFSET",
      "END_OFFSET",
      "DIRECTION_FROM_LANDMARK",
      "LOCATION_DESCRIPTION",
      "DITCH_VOLUME",
      "TRAVELLED_LANES_VOLUME",
      "OTHER_VOLUME",
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
go

CREATE TRIGGER HMR_RL_I_S_I_TR ON HMR_ROLE INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_ROLE ("ROLE_ID",
      "NAME",
      "DESCRIPTION",
      "IS_INTERNAL",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "ROLE_ID",
      "NAME",
      "DESCRIPTION",
      "IS_INTERNAL",
      "END_DATE",
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
go

CREATE TRIGGER HMR_RL_PERM_I_S_I_TR ON HMR_ROLE_PERMISSION INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_ROLE_PERMISSION ("ROLE_PERMISSION_ID",
      "ROLE_ID",
      "PERMISSION_ID",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "ROLE_PERMISSION_ID",
      "ROLE_ID",
      "PERMISSION_ID",
      "END_DATE",
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
go

CREATE TRIGGER HMR_SRV_ARA_I_S_I_TR ON HMR_SERVICE_AREA INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_SERVICE_AREA ("SERVICE_AREA_ID",
      "SERVICE_AREA_NUMBER",
      "SERVICE_AREA_NAME",
      "DISTRICT_NUMBER",
      "CONCURRENCY_CONTROL_NUMBER")
    select "SERVICE_AREA_ID",
      "SERVICE_AREA_NUMBER",
      "SERVICE_AREA_NAME",
      "DISTRICT_NUMBER",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_SRV_ARA_USR_I_S_I_TR ON HMR_SERVICE_AREA_USER INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_SERVICE_AREA_USER ("SERVICE_AREA_USER_ID",
      "SERVICE_AREA_NUMBER",
      "SYSTEM_USER_ID",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "SERVICE_AREA_USER_ID",
      "SERVICE_AREA_NUMBER",
      "SYSTEM_USER_ID",
      "END_DATE",
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
go

CREATE TRIGGER HMR_STR_ELMT_I_S_I_TR ON HMR_STREAM_ELEMENT INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_STREAM_ELEMENT ("STREAM_ELEMENT_ID",
      "SUBMISSION_STREAM_ID",
      "ELEMENT_NAME",
      "ELEMENT_TYPE",
      "STAGING_COLUMN_NAME",
      "IS_REQUIRED",
      "MAX_LENGTH",
      "MIN_LENGTH",
      "MAX_VALUE",
      "MIN_VALUE",
      "MAX_DATE",
      "MIN_DATE",
      "REG_EXP",
      "CODE_SET",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "STREAM_ELEMENT_ID",
      "SUBMISSION_STREAM_ID",
      "ELEMENT_NAME",
      "ELEMENT_TYPE",
      "STAGING_COLUMN_NAME",
      "IS_REQUIRED",
      "MAX_LENGTH",
      "MIN_LENGTH",
      "MAX_VALUE",
      "MIN_VALUE",
      "MAX_DATE",
      "MIN_DATE",
      "REG_EXP",
      "CODE_SET",
      "END_DATE",
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
go

CREATE TRIGGER HMR_SUBM_OBJ_I_S_I_TR ON HMR_SUBMISSION_OBJECT INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_SUBMISSION_OBJECT ("SUBMISSION_OBJECT_ID",
      "FILE_NAME",
      "DIGITAL_REPRESENTATION",
      "MIME_TYPE_ID",
      "SUBMISSION_STATUS_ID",
      "SERVICE_AREA_NUMBER",
      "PARTY_ID",
      "FILE_HASH",
      "ERROR_DETAIL",
      "SUBMISSION_STREAM_ID",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "SUBMISSION_OBJECT_ID",
      "FILE_NAME",
      "DIGITAL_REPRESENTATION",
      "MIME_TYPE_ID",
      "SUBMISSION_STATUS_ID",
      "SERVICE_AREA_NUMBER",
      "PARTY_ID",
      "FILE_HASH",
      "ERROR_DETAIL",
      "SUBMISSION_STREAM_ID",
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
go

CREATE TRIGGER HMR_SUBM_RW_I_S_I_TR ON HMR_SUBMISSION_ROW INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_SUBMISSION_ROW ("ROW_ID",
      "SUBMISSION_OBJECT_ID",
      "ROW_STATUS_ID",
      "RECORD_NUMBER",
      "ROW_VALUE",
      "ROW_HASH",
      "ERROR_DETAIL",
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
      "RECORD_NUMBER",
      "ROW_VALUE",
      "ROW_HASH",
      "ERROR_DETAIL",
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
go

CREATE TRIGGER SS19_I_S_I_TR ON HMR_SUBMISSION_STATUS INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_SUBMISSION_STATUS ("STATUS_ID",
      "STATUS_CODE",
      "DESCRIPTION",
      "STATUS_TYPE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "STATUS_ID",
      "STATUS_CODE",
      "DESCRIPTION",
      "STATUS_TYPE",
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
go

CREATE TRIGGER HMR_SUBM_STR_I_S_I_TR ON HMR_SUBMISSION_STREAM INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_SUBMISSION_STREAM ("SUBMISSION_STREAM_ID",
      "STREAM_NAME",
      "END_DATE",
      "FILE_SIZE_LIMIT",
      "STAGING_TABLE_NAME",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "SUBMISSION_STREAM_ID",
      "STREAM_NAME",
      "END_DATE",
      "FILE_SIZE_LIMIT",
      "STAGING_TABLE_NAME",
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
go

CREATE TRIGGER HMR_SYS_USR_I_S_I_TR ON HMR_SYSTEM_USER INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_SYSTEM_USER ("SYSTEM_USER_ID",
      "PARTY_ID",
      "USER_GUID",
      "USERNAME",
      "USER_DIRECTORY",
      "USER_TYPE",
      "FIRST_NAME",
      "LAST_NAME",
      "EMAIL",
      "BUSINESS_GUID",
      "BUSINESS_LEGAL_NAME",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "SYSTEM_USER_ID",
      "PARTY_ID",
      "USER_GUID",
      "USERNAME",
      "USER_DIRECTORY",
      "USER_TYPE",
      "FIRST_NAME",
      "LAST_NAME",
      "EMAIL",
      "BUSINESS_GUID",
      "BUSINESS_LEGAL_NAME",
      "END_DATE",
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
go

CREATE TRIGGER SV22_I_S_I_TR ON HMR_SYSTEM_VALIDATION INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_SYSTEM_VALIDATION ("SYSTEM_VALIDATION_ID",
      "ENTITY_NAME",
      "ATTRIBUTE_NAME",
      "ATTRIBUTE_TYPE",
      "IS_REQUIRED",
      "MAX_LENGTH",
      "MIN_LENGTH",
      "MAX_VALUE",
      "MIN_VALUE",
      "MAX_DATE",
      "MIN_DATE",
      "REG_EXP",
      "CODE_SET",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER")
    select "SYSTEM_VALIDATION_ID",
      "ENTITY_NAME",
      "ATTRIBUTE_NAME",
      "ATTRIBUTE_TYPE",
      "IS_REQUIRED",
      "MAX_LENGTH",
      "MIN_LENGTH",
      "MAX_VALUE",
      "MIN_VALUE",
      "MAX_DATE",
      "MIN_DATE",
      "REG_EXP",
      "CODE_SET",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER"
    from inserted;

END TRY
BEGIN CATCH
   IF @@trancount > 0 ROLLBACK TRANSACTION
   EXEC hmr_error_handling
END CATCH;
go

CREATE TRIGGER HMR_USR_RL_I_S_I_TR ON HMR_USER_ROLE INSTEAD OF INSERT AS
SET NOCOUNT ON
BEGIN TRY
  IF NOT EXISTS(SELECT * FROM inserted) 
    RETURN;

  
  insert into HMR_USER_ROLE ("USER_ROLE_ID",
      "ROLE_ID",
      "SYSTEM_USER_ID",
      "END_DATE",
      "CONCURRENCY_CONTROL_NUMBER",
      "APP_CREATE_USERID",
      "APP_CREATE_TIMESTAMP",
      "APP_CREATE_USER_GUID",
      "APP_CREATE_USER_DIRECTORY",
      "APP_LAST_UPDATE_USERID",
      "APP_LAST_UPDATE_TIMESTAMP",
      "APP_LAST_UPDATE_USER_GUID",
      "APP_LAST_UPDATE_USER_DIRECTORY")
    select "USER_ROLE_ID",
      "ROLE_ID",
      "SYSTEM_USER_ID",
      "END_DATE",
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
go

CREATE TRIGGER HMR_WLDLF_RPT_I_S_I_TR ON HMR_WILDLIFE_REPORT INSTEAD OF INSERT AS
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
      "HIGHWAY_UNIQUE_NUMBER",
      "LANDMARK",
      "START_OFFSET",
      "NEAREST_TOWN",
      "WILDLIFE_SIGN",
      "QUANTITY",
      "SPECIES",
      "SEX",
      "AGE",
      "COMMENT",
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
      "HIGHWAY_UNIQUE_NUMBER",
      "LANDMARK",
      "START_OFFSET",
      "NEAREST_TOWN",
      "WILDLIFE_SIGN",
      "QUANTITY",
      "SPECIES",
      "SEX",
      "AGE",
      "COMMENT",
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
go

CREATE TRIGGER HMR_WRK_RPT_I_S_I_TR ON HMR_WORK_REPORT INSTEAD OF INSERT AS
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
go

