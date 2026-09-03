/*
    HMCR V29.0 - Notification preferences and email delivery status

    This script is intentionally idempotent and does not select a database so
    that the deployment pipeline can apply it to each target environment.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    /* ------------------------------------------------------------------ */
    /* Email delivery status                                               */
    /* ------------------------------------------------------------------ */

    IF COL_LENGTH(N'dbo.HMR_FEEDBACK_MESSAGE', N'DELIVERY_STATUS') IS NULL
    BEGIN
        ALTER TABLE [dbo].[HMR_FEEDBACK_MESSAGE]
            ADD [DELIVERY_STATUS] VARCHAR(30) NULL;
    END;

    -- Both feedback triggers use explicit column lists, so they must include
    -- DELIVERY_STATUS before the existing records can be backfilled.
    EXEC(N'
CREATE OR ALTER TRIGGER [dbo].[HMR_FDBK_MSG_I_S_I_TR]
ON [dbo].[HMR_FEEDBACK_MESSAGE]
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM inserted)
            RETURN;

        INSERT INTO [dbo].[HMR_FEEDBACK_MESSAGE]
        (
            [FEEDBACK_MESSAGE_ID],
            [SUBMISSION_OBJECT_ID],
            [COMMUNICATION_SUBJECT],
            [COMMUNICATION_TEXT],
            [COMMUNICATION_DATE],
            [IS_SENT],
            [IS_ERROR],
            [SEND_ERROR_TEXT],
            [DELIVERY_STATUS],
            [CONCURRENCY_CONTROL_NUMBER]
        )
        SELECT
            [FEEDBACK_MESSAGE_ID],
            [SUBMISSION_OBJECT_ID],
            [COMMUNICATION_SUBJECT],
            [COMMUNICATION_TEXT],
            [COMMUNICATION_DATE],
            [IS_SENT],
            [IS_ERROR],
            [SEND_ERROR_TEXT],
            CASE
                WHEN [DELIVERY_STATUS] = ''SKIPPED_NO_RECIPIENTS'' THEN ''SKIPPED_NO_RECIPIENTS''
                WHEN [IS_SENT] = 1 THEN ''SENT''
                ELSE ''FAILED''
            END,
            [CONCURRENCY_CONTROL_NUMBER]
        FROM inserted;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        EXEC [dbo].[hmr_error_handling];
    END CATCH;
END;');

    EXEC(N'
CREATE OR ALTER TRIGGER [dbo].[HMR_FDBK_MSG_I_S_U_TR]
ON [dbo].[HMR_FEEDBACK_MESSAGE]
INSTEAD OF UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM deleted)
            RETURN;

        IF EXISTS
        (
            SELECT 1
            FROM inserted i
            INNER JOIN deleted d
                ON d.[FEEDBACK_MESSAGE_ID] = i.[FEEDBACK_MESSAGE_ID]
            WHERE i.[CONCURRENCY_CONTROL_NUMBER] <> d.[CONCURRENCY_CONTROL_NUMBER] + 1
        )
            RAISERROR(''CONCURRENCY FAILURE.'', 16, 1);

        UPDATE target
        SET
            [SUBMISSION_OBJECT_ID] = source.[SUBMISSION_OBJECT_ID],
            [COMMUNICATION_SUBJECT] = source.[COMMUNICATION_SUBJECT],
            [COMMUNICATION_TEXT] = source.[COMMUNICATION_TEXT],
            [COMMUNICATION_DATE] = source.[COMMUNICATION_DATE],
            [IS_SENT] = source.[IS_SENT],
            [IS_ERROR] = source.[IS_ERROR],
            [SEND_ERROR_TEXT] = source.[SEND_ERROR_TEXT],
            [DELIVERY_STATUS] = CASE
                WHEN source.[DELIVERY_STATUS] = ''SKIPPED_NO_RECIPIENTS'' THEN ''SKIPPED_NO_RECIPIENTS''
                WHEN source.[IS_SENT] = 1 THEN ''SENT''
                ELSE ''FAILED''
            END,
            [CONCURRENCY_CONTROL_NUMBER] = source.[CONCURRENCY_CONTROL_NUMBER],
            [DB_AUDIT_LAST_UPDATE_TIMESTAMP] = GETUTCDATE(),
            [DB_AUDIT_LAST_UPDATE_USERID] = USER_NAME()
        FROM [dbo].[HMR_FEEDBACK_MESSAGE] target
        INNER JOIN inserted source
            ON source.[FEEDBACK_MESSAGE_ID] = target.[FEEDBACK_MESSAGE_ID];
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        EXEC [dbo].[hmr_error_handling];
    END CATCH;
END;');

    UPDATE [dbo].[HMR_FEEDBACK_MESSAGE]
    SET
        [DELIVERY_STATUS] = CASE WHEN [IS_SENT] = 1 THEN 'SENT' ELSE 'FAILED' END,
        [CONCURRENCY_CONTROL_NUMBER] = [CONCURRENCY_CONTROL_NUMBER] + 1
    WHERE [DELIVERY_STATUS] IS NULL;

    IF EXISTS
    (
        SELECT 1
        FROM sys.columns
        WHERE [object_id] = OBJECT_ID(N'dbo.HMR_FEEDBACK_MESSAGE')
          AND [name] = N'DELIVERY_STATUS'
          AND [is_nullable] = 1
    )
    BEGIN
        ALTER TABLE [dbo].[HMR_FEEDBACK_MESSAGE]
            ALTER COLUMN [DELIVERY_STATUS] VARCHAR(30) NOT NULL;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.default_constraints dc
        INNER JOIN sys.columns c
            ON c.[object_id] = dc.[parent_object_id]
           AND c.[column_id] = dc.[parent_column_id]
        WHERE dc.[parent_object_id] = OBJECT_ID(N'dbo.HMR_FEEDBACK_MESSAGE')
          AND c.[name] = N'DELIVERY_STATUS'
    )
    BEGIN
        ALTER TABLE [dbo].[HMR_FEEDBACK_MESSAGE]
            ADD CONSTRAINT [HMR_FDBK_MSG_DLVRY_ST_DF]
            DEFAULT ('FAILED') FOR [DELIVERY_STATUS];
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE [parent_object_id] = OBJECT_ID(N'dbo.HMR_FEEDBACK_MESSAGE')
          AND [name] = N'HMR_FDBK_MSG_DLVRY_ST_CK'
    )
    BEGIN
        ALTER TABLE [dbo].[HMR_FEEDBACK_MESSAGE] WITH CHECK
            ADD CONSTRAINT [HMR_FDBK_MSG_DLVRY_ST_CK]
            CHECK ([DELIVERY_STATUS] IN ('SENT', 'FAILED', 'SKIPPED_NO_RECIPIENTS'));
    END;

    /* ------------------------------------------------------------------ */
    /* Notification preference table                                      */
    /* ------------------------------------------------------------------ */

    IF OBJECT_ID(N'dbo.HMR_NOTIF_PREF_ID_SEQ', N'SO') IS NULL
    BEGIN
        EXEC(N'
CREATE SEQUENCE [dbo].[HMR_NOTIF_PREF_ID_SEQ]
    AS BIGINT
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 999999999
    CACHE 50;');
    END;

    IF OBJECT_ID(N'dbo.HMR_NOTIFICATION_PREFERENCE', N'U') IS NULL
    BEGIN
        EXEC(N'
CREATE TABLE [dbo].[HMR_NOTIFICATION_PREFERENCE]
(
    [NOTIFICATION_PREFERENCE_ID] NUMERIC(9, 0) NOT NULL
        CONSTRAINT [HMR_NOTIF_PREF_ID_DF]
        DEFAULT (NEXT VALUE FOR [dbo].[HMR_NOTIF_PREF_ID_SEQ]),
    [SERVICE_AREA_USER_ID] NUMERIC(9, 0) NOT NULL,
    [SUBMISSION_STREAM_ID] NUMERIC(9, 0) NOT NULL,
    [SUCCESS_EMAIL_ENABLED] BIT NOT NULL
        CONSTRAINT [HMR_NOTIF_PREF_SUCCESS_DF] DEFAULT (1),
    [ERROR_EMAIL_ENABLED] BIT NOT NULL
        CONSTRAINT [HMR_NOTIF_PREF_ERROR_DF] DEFAULT (1),
    [CONCURRENCY_CONTROL_NUMBER] BIGINT NOT NULL
        CONSTRAINT [HMR_NOTIF_PREF_CC_DF] DEFAULT (1),
    [APP_CREATE_USERID] VARCHAR(30) NOT NULL,
    [APP_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_CREATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_CREATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [APP_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [APP_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_LAST_UPDATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_LAST_UPDATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_NOTIF_PREF_DB_CRT_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_NOTIF_PREF_DB_CRT_TS_DF] DEFAULT (GETUTCDATE()),
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_NOTIF_PREF_DB_UPD_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_NOTIF_PREF_DB_UPD_TS_DF] DEFAULT (GETUTCDATE())
);');
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.key_constraints
        WHERE [parent_object_id] = OBJECT_ID(N'dbo.HMR_NOTIFICATION_PREFERENCE')
          AND [type] = 'PK'
    )
    BEGIN
        ALTER TABLE [dbo].[HMR_NOTIFICATION_PREFERENCE]
            ADD CONSTRAINT [HMR_NOTIF_PREF_PK]
            PRIMARY KEY CLUSTERED ([NOTIFICATION_PREFERENCE_ID]);
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.key_constraints
        WHERE [parent_object_id] = OBJECT_ID(N'dbo.HMR_NOTIFICATION_PREFERENCE')
          AND [name] = N'HMR_NOTIF_PREF_UK'
    )
    BEGIN
        ALTER TABLE [dbo].[HMR_NOTIFICATION_PREFERENCE]
            ADD CONSTRAINT [HMR_NOTIF_PREF_UK]
            UNIQUE NONCLUSTERED ([SERVICE_AREA_USER_ID], [SUBMISSION_STREAM_ID]);
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.indexes
        WHERE [object_id] = OBJECT_ID(N'dbo.HMR_NOTIFICATION_PREFERENCE')
          AND [name] = N'HMR_NOTIF_PREF_STRM_FK_I'
    )
    BEGIN
        CREATE NONCLUSTERED INDEX [HMR_NOTIF_PREF_STRM_FK_I]
            ON [dbo].[HMR_NOTIFICATION_PREFERENCE] ([SUBMISSION_STREAM_ID]);
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.foreign_keys
        WHERE [parent_object_id] = OBJECT_ID(N'dbo.HMR_NOTIFICATION_PREFERENCE')
          AND [name] = N'HMR_NOTIF_PREF_SAU_FK'
    )
    BEGIN
        ALTER TABLE [dbo].[HMR_NOTIFICATION_PREFERENCE] WITH CHECK
            ADD CONSTRAINT [HMR_NOTIF_PREF_SAU_FK]
            FOREIGN KEY ([SERVICE_AREA_USER_ID])
            REFERENCES [dbo].[HMR_SERVICE_AREA_USER] ([SERVICE_AREA_USER_ID])
            ON DELETE CASCADE;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.foreign_keys
        WHERE [parent_object_id] = OBJECT_ID(N'dbo.HMR_NOTIFICATION_PREFERENCE')
          AND [name] = N'HMR_NOTIF_PREF_STRM_FK'
    )
    BEGIN
        ALTER TABLE [dbo].[HMR_NOTIFICATION_PREFERENCE] WITH CHECK
            ADD CONSTRAINT [HMR_NOTIF_PREF_STRM_FK]
            FOREIGN KEY ([SUBMISSION_STREAM_ID])
            REFERENCES [dbo].[HMR_SUBMISSION_STREAM] ([SUBMISSION_STREAM_ID]);
    END;

    DECLARE @MigrationTimestamp DATETIME = GETUTCDATE();
    DECLARE @MigrationUserId VARCHAR(30) = LEFT(COALESCE(CONVERT(VARCHAR(128), SUSER_SNAME()), USER_NAME(), 'MIGRATION'), 30);
    DECLARE @MigrationUserGuid UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000000';
    DECLARE @MigrationUserDirectory VARCHAR(12) = 'DATABASE';

    -- Include inactive assignments so every pre-existing INTERNAL assignment has
    -- the enabled default. Ended assignments remain hidden by application queries;
    -- deleting an assignment cascades these rows so a later reassignment resets.
    INSERT INTO [dbo].[HMR_NOTIFICATION_PREFERENCE]
    (
        [NOTIFICATION_PREFERENCE_ID],
        [SERVICE_AREA_USER_ID],
        [SUBMISSION_STREAM_ID],
        [SUCCESS_EMAIL_ENABLED],
        [ERROR_EMAIL_ENABLED],
        [CONCURRENCY_CONTROL_NUMBER],
        [APP_CREATE_USERID],
        [APP_CREATE_TIMESTAMP],
        [APP_CREATE_USER_GUID],
        [APP_CREATE_USER_DIRECTORY],
        [APP_LAST_UPDATE_USERID],
        [APP_LAST_UPDATE_TIMESTAMP],
        [APP_LAST_UPDATE_USER_GUID],
        [APP_LAST_UPDATE_USER_DIRECTORY]
    )
    SELECT
        NEXT VALUE FOR [dbo].[HMR_NOTIF_PREF_ID_SEQ],
        sau.[SERVICE_AREA_USER_ID],
        stream.[SUBMISSION_STREAM_ID],
        1,
        1,
        1,
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory,
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory
    FROM [dbo].[HMR_SERVICE_AREA_USER] sau
    INNER JOIN [dbo].[HMR_SYSTEM_USER] systemUser
        ON systemUser.[SYSTEM_USER_ID] = sau.[SYSTEM_USER_ID]
    CROSS JOIN [dbo].[HMR_SUBMISSION_STREAM] stream
    WHERE systemUser.[USER_TYPE] = 'INTERNAL'
      AND stream.[STAGING_TABLE_NAME] IN
          ('HMR_WORK_REPORT', 'HMR_ROCKFALL_REPORT', 'HMR_WILDLIFE_REPORT')
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[HMR_NOTIFICATION_PREFERENCE] existing
          WHERE existing.[SERVICE_AREA_USER_ID] = sau.[SERVICE_AREA_USER_ID]
            AND existing.[SUBMISSION_STREAM_ID] = stream.[SUBMISSION_STREAM_ID]
      );

    EXEC(N'
CREATE OR ALTER TRIGGER [dbo].[HMR_NOTIF_PREF_I_S_I_TR]
ON [dbo].[HMR_NOTIFICATION_PREFERENCE]
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM inserted)
            RETURN;

        INSERT INTO [dbo].[HMR_NOTIFICATION_PREFERENCE]
        (
            [NOTIFICATION_PREFERENCE_ID],
            [SERVICE_AREA_USER_ID],
            [SUBMISSION_STREAM_ID],
            [SUCCESS_EMAIL_ENABLED],
            [ERROR_EMAIL_ENABLED],
            [CONCURRENCY_CONTROL_NUMBER],
            [APP_CREATE_USERID],
            [APP_CREATE_TIMESTAMP],
            [APP_CREATE_USER_GUID],
            [APP_CREATE_USER_DIRECTORY],
            [APP_LAST_UPDATE_USERID],
            [APP_LAST_UPDATE_TIMESTAMP],
            [APP_LAST_UPDATE_USER_GUID],
            [APP_LAST_UPDATE_USER_DIRECTORY]
        )
        SELECT
            [NOTIFICATION_PREFERENCE_ID],
            [SERVICE_AREA_USER_ID],
            [SUBMISSION_STREAM_ID],
            [SUCCESS_EMAIL_ENABLED],
            [ERROR_EMAIL_ENABLED],
            [CONCURRENCY_CONTROL_NUMBER],
            [APP_CREATE_USERID],
            [APP_CREATE_TIMESTAMP],
            [APP_CREATE_USER_GUID],
            [APP_CREATE_USER_DIRECTORY],
            [APP_LAST_UPDATE_USERID],
            [APP_LAST_UPDATE_TIMESTAMP],
            [APP_LAST_UPDATE_USER_GUID],
            [APP_LAST_UPDATE_USER_DIRECTORY]
        FROM inserted;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        EXEC [dbo].[hmr_error_handling];
    END CATCH;
END;');

    EXEC(N'
CREATE OR ALTER TRIGGER [dbo].[HMR_NOTIF_PREF_I_S_U_TR]
ON [dbo].[HMR_NOTIFICATION_PREFERENCE]
INSTEAD OF UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM deleted)
            RETURN;

        IF EXISTS
        (
            SELECT 1
            FROM inserted i
            INNER JOIN deleted d
                ON d.[NOTIFICATION_PREFERENCE_ID] = i.[NOTIFICATION_PREFERENCE_ID]
            WHERE i.[CONCURRENCY_CONTROL_NUMBER] <> d.[CONCURRENCY_CONTROL_NUMBER] + 1
        )
            RAISERROR(''CONCURRENCY FAILURE.'', 16, 1);

        UPDATE target
        SET
            [SERVICE_AREA_USER_ID] = source.[SERVICE_AREA_USER_ID],
            [SUBMISSION_STREAM_ID] = source.[SUBMISSION_STREAM_ID],
            [SUCCESS_EMAIL_ENABLED] = source.[SUCCESS_EMAIL_ENABLED],
            [ERROR_EMAIL_ENABLED] = source.[ERROR_EMAIL_ENABLED],
            [CONCURRENCY_CONTROL_NUMBER] = source.[CONCURRENCY_CONTROL_NUMBER],
            [APP_LAST_UPDATE_USERID] = source.[APP_LAST_UPDATE_USERID],
            [APP_LAST_UPDATE_TIMESTAMP] = source.[APP_LAST_UPDATE_TIMESTAMP],
            [APP_LAST_UPDATE_USER_GUID] = source.[APP_LAST_UPDATE_USER_GUID],
            [APP_LAST_UPDATE_USER_DIRECTORY] = source.[APP_LAST_UPDATE_USER_DIRECTORY],
            [DB_AUDIT_LAST_UPDATE_TIMESTAMP] = GETUTCDATE(),
            [DB_AUDIT_LAST_UPDATE_USERID] = USER_NAME()
        FROM [dbo].[HMR_NOTIFICATION_PREFERENCE] target
        INNER JOIN inserted source
            ON source.[NOTIFICATION_PREFERENCE_ID] = target.[NOTIFICATION_PREFERENCE_ID];
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        EXEC [dbo].[hmr_error_handling];
    END CATCH;
END;');

    /* ------------------------------------------------------------------ */
    /* Descriptions                                                        */
    /* ------------------------------------------------------------------ */

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.extended_properties
        WHERE [major_id] = OBJECT_ID(N'dbo.HMR_NOTIFICATION_PREFERENCE')
          AND [minor_id] = 0
          AND [name] = N'MS_Description'
    )
    BEGIN
        EXEC sys.sp_addextendedproperty
            @name = N'MS_Description',
            @value = N'Email notification preferences for an internal user service area assignment and submission stream.',
            @level0type = N'SCHEMA', @level0name = N'dbo',
            @level1type = N'TABLE', @level1name = N'HMR_NOTIFICATION_PREFERENCE';
    END;

    DECLARE @PreferenceDescriptions TABLE
    (
        [ColumnName] SYSNAME NOT NULL,
        [Description] NVARCHAR(4000) NOT NULL
    );

    INSERT INTO @PreferenceDescriptions ([ColumnName], [Description])
    VALUES
        (N'NOTIFICATION_PREFERENCE_ID', N'Unique identifier for a notification preference.'),
        (N'SERVICE_AREA_USER_ID', N'Service area assignment governed by this preference.'),
        (N'SUBMISSION_STREAM_ID', N'Submission stream governed by this preference.'),
        (N'SUCCESS_EMAIL_ENABLED', N'Indicates whether successful upload emails are enabled.'),
        (N'ERROR_EMAIL_ENABLED', N'Indicates whether upload error emails are enabled.'),
        (N'CONCURRENCY_CONTROL_NUMBER', N'Record under edit indicator used for optimistic record contention management.'),
        (N'APP_CREATE_USERID', N'Unique identifier of user who created record.'),
        (N'APP_CREATE_TIMESTAMP', N'Date and time of record creation.'),
        (N'APP_CREATE_USER_GUID', N'Unique identifier of user who created record.'),
        (N'APP_CREATE_USER_DIRECTORY', N'Identity directory of the user who created record.'),
        (N'APP_LAST_UPDATE_USERID', N'Unique identifier of user who last updated record.'),
        (N'APP_LAST_UPDATE_TIMESTAMP', N'Date and time of last record update.'),
        (N'APP_LAST_UPDATE_USER_GUID', N'Unique identifier of user who last updated record.'),
        (N'APP_LAST_UPDATE_USER_DIRECTORY', N'Identity directory of the user who last updated record.'),
        (N'DB_AUDIT_CREATE_USERID', N'Named database user who created record.'),
        (N'DB_AUDIT_CREATE_TIMESTAMP', N'Date and time record was created in the database.'),
        (N'DB_AUDIT_LAST_UPDATE_USERID', N'Named database user who last updated record.'),
        (N'DB_AUDIT_LAST_UPDATE_TIMESTAMP', N'Date and time record was last updated in the database.');

    DECLARE @ColumnName SYSNAME;
    DECLARE @Description NVARCHAR(4000);

    DECLARE preference_description_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT [ColumnName], [Description]
        FROM @PreferenceDescriptions;

    OPEN preference_description_cursor;
    FETCH NEXT FROM preference_description_cursor INTO @ColumnName, @Description;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS
        (
            SELECT 1
            FROM sys.extended_properties ep
            INNER JOIN sys.columns c
                ON c.[object_id] = ep.[major_id]
               AND c.[column_id] = ep.[minor_id]
            WHERE ep.[major_id] = OBJECT_ID(N'dbo.HMR_NOTIFICATION_PREFERENCE')
              AND c.[name] = @ColumnName
              AND ep.[name] = N'MS_Description'
        )
        BEGIN
            EXEC sys.sp_addextendedproperty
                @name = N'MS_Description', @value = @Description,
                @level0type = N'SCHEMA', @level0name = N'dbo',
                @level1type = N'TABLE', @level1name = N'HMR_NOTIFICATION_PREFERENCE',
                @level2type = N'COLUMN', @level2name = @ColumnName;
        END;

        FETCH NEXT FROM preference_description_cursor INTO @ColumnName, @Description;
    END;

    CLOSE preference_description_cursor;
    DEALLOCATE preference_description_cursor;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.extended_properties ep
        INNER JOIN sys.columns c
            ON c.[object_id] = ep.[major_id]
           AND c.[column_id] = ep.[minor_id]
        WHERE ep.[major_id] = OBJECT_ID(N'dbo.HMR_FEEDBACK_MESSAGE')
          AND c.[name] = N'DELIVERY_STATUS'
          AND ep.[name] = N'MS_Description'
    )
    BEGIN
        EXEC sys.sp_addextendedproperty
            @name = N'MS_Description',
            @value = N'Final application delivery outcome: SENT, FAILED, or SKIPPED_NO_RECIPIENTS.',
            @level0type = N'SCHEMA', @level0name = N'dbo',
            @level1type = N'TABLE', @level1name = N'HMR_FEEDBACK_MESSAGE',
            @level2type = N'COLUMN', @level2name = N'DELIVERY_STATUS';
    END;

    /* ------------------------------------------------------------------ */
    /* Database role grants                                                */
    /* ------------------------------------------------------------------ */

    IF DATABASE_PRINCIPAL_ID(N'HMR_APPLICATION_PROXY') IS NOT NULL
    BEGIN
        GRANT SELECT, INSERT, UPDATE, DELETE
            ON OBJECT::[dbo].[HMR_NOTIFICATION_PREFERENCE]
            TO [HMR_APPLICATION_PROXY];

        GRANT UPDATE
            ON OBJECT::[dbo].[HMR_NOTIF_PREF_ID_SEQ]
            TO [HMR_APPLICATION_PROXY];
    END;

    IF DATABASE_PRINCIPAL_ID(N'HMR_READ_ONLY') IS NOT NULL
    BEGIN
        GRANT SELECT
            ON OBJECT::[dbo].[HMR_NOTIFICATION_PREFERENCE]
            TO [HMR_READ_ONLY];
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
