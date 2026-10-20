/*
    HMCR-1192: Add a distinct file status for synchronous blackout rejection.
    Safe to rerun after the V30 schema.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;

BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[HMR_SUBMISSION_STATUS]
        WHERE [STATUS_CODE] = 'SB'
    )
    BEGIN
        DECLARE @Timestamp DATETIME = GETUTCDATE();
        DECLARE @UserId VARCHAR(30) = LEFT(COALESCE(CONVERT(VARCHAR(128), SUSER_SNAME()), USER_NAME(), 'MIGRATION'), 30);
        DECLARE @UserGuid UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000000';

        INSERT INTO [dbo].[HMR_SUBMISSION_STATUS]
        (
            [STATUS_CODE],
            [DESCRIPTION],
            [LONG_DESCRIPTION],
            [STATUS_TYPE],
            [STAGE],
            [APP_CREATE_USERID],
            [APP_CREATE_TIMESTAMP],
            [APP_CREATE_USER_GUID],
            [APP_CREATE_USER_DIRECTORY],
            [APP_LAST_UPDATE_USERID],
            [APP_LAST_UPDATE_TIMESTAMP],
            [APP_LAST_UPDATE_USER_GUID],
            [APP_LAST_UPDATE_USER_DIRECTORY]
        )
        VALUES
        (
            'SB',
            'Submission Blocked',
            'Reason: Blackout Period Threshold Exceeded',
            'F',
            1,
            @UserId,
            @Timestamp,
            @UserGuid,
            'DATABASE',
            @UserId,
            @Timestamp,
            @UserGuid,
            'DATABASE'
        );
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
