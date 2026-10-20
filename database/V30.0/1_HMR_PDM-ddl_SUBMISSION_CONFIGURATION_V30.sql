/*
    HMCR V30.0 - Submission blackout configuration

    This script is intentionally idempotent and does not select a database so
    that the deployment pipeline can apply it to each target environment.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @MigrationTimestamp DATETIME = GETUTCDATE();
    DECLARE @MigrationUserId VARCHAR(30) = LEFT(COALESCE(CONVERT(VARCHAR(128), SUSER_SNAME()), USER_NAME(), 'MIGRATION'), 30);
    DECLARE @MigrationUserGuid UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000000';
    DECLARE @MigrationUserDirectory VARCHAR(12) = 'DATABASE';

    /* ------------------------------------------------------------------ */
    /* Sequences                                                           */
    /* ------------------------------------------------------------------ */

    IF OBJECT_ID(N'dbo.HMR_SUBM_CFG_ID_SEQ', N'SO') IS NULL
    BEGIN
        EXEC(N'
CREATE SEQUENCE [dbo].[HMR_SUBM_CFG_ID_SEQ]
    AS BIGINT START WITH 1 INCREMENT BY 1
    MINVALUE 1 MAXVALUE 999999999 CACHE 50;');
    END;

    IF OBJECT_ID(N'dbo.HMR_SUBM_CFG_WIN_ID_SEQ', N'SO') IS NULL
    BEGIN
        EXEC(N'
CREATE SEQUENCE [dbo].[HMR_SUBM_CFG_WIN_ID_SEQ]
    AS BIGINT START WITH 1 INCREMENT BY 1
    MINVALUE 1 MAXVALUE 999999999 CACHE 50;');
    END;

    IF OBJECT_ID(N'dbo.HMR_SUBM_CFG_RULE_ID_SEQ', N'SO') IS NULL
    BEGIN
        EXEC(N'
CREATE SEQUENCE [dbo].[HMR_SUBM_CFG_RULE_ID_SEQ]
    AS BIGINT START WITH 1 INCREMENT BY 1
    MINVALUE 1 MAXVALUE 999999999 CACHE 50;');
    END;

    IF OBJECT_ID(N'dbo.HMR_SUBM_CFG_RACT_ID_SEQ', N'SO') IS NULL
    BEGIN
        EXEC(N'
CREATE SEQUENCE [dbo].[HMR_SUBM_CFG_RACT_ID_SEQ]
    AS BIGINT START WITH 1 INCREMENT BY 1
    MINVALUE 1 MAXVALUE 999999999 CACHE 50;');
    END;

    IF OBJECT_ID(N'dbo.HMR_SUBM_CFG_AUD_ID_SEQ', N'SO') IS NULL
    BEGIN
        EXEC(N'
CREATE SEQUENCE [dbo].[HMR_SUBM_CFG_AUD_ID_SEQ]
    AS BIGINT START WITH 1 INCREMENT BY 1
    MINVALUE 1 MAXVALUE 999999999 CACHE 50;');
    END;

    IF OBJECT_ID(N'dbo.HMR_SUBM_CFG_SA_ID_SEQ', N'SO') IS NULL
    BEGIN
        EXEC(N'
CREATE SEQUENCE [dbo].[HMR_SUBM_CFG_SA_ID_SEQ]
    AS BIGINT START WITH 1 INCREMENT BY 1
    MINVALUE 1 MAXVALUE 999999999 CACHE 50;');
    END;

    /* ------------------------------------------------------------------ */
    /* Tables                                                              */
    /* ------------------------------------------------------------------ */

    IF OBJECT_ID(N'dbo.HMR_SUBMISSION_CONFIGURATION', N'U') IS NULL
    BEGIN
        EXEC(N'
CREATE TABLE [dbo].[HMR_SUBMISSION_CONFIGURATION]
(
    [SUBMISSION_CONFIGURATION_ID] NUMERIC(9, 0) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_ID_DF]
        DEFAULT (NEXT VALUE FOR [dbo].[HMR_SUBM_CFG_ID_SEQ]),
    [CONFIGURATION_KEY] VARCHAR(50) NOT NULL,
    [CONFIGURATION_NAME] VARCHAR(150) NOT NULL,
    [SUBMISSION_STREAM_ID] NUMERIC(9, 0) NOT NULL,
    [IS_ACTIVE] BIT NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_ACTIVE_DF] DEFAULT (0),
    [EFFECTIVE_FROM_DATE] DATE NOT NULL,
    [EFFECTIVE_TO_DATE] DATE NULL,
    [TIME_ZONE_ID] VARCHAR(64) NOT NULL,
    [SCOPE_TYPE] VARCHAR(20) NOT NULL,
    [CONCURRENCY_CONTROL_NUMBER] BIGINT NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_CC_DF] DEFAULT (1),
    [APP_CREATE_USERID] VARCHAR(30) NOT NULL,
    [APP_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_CREATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_CREATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [APP_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [APP_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_LAST_UPDATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_LAST_UPDATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_DB_CRT_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_DB_CRT_TS_DF] DEFAULT (GETUTCDATE()),
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_DB_UPD_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_DB_UPD_TS_DF] DEFAULT (GETUTCDATE())
);');
    END;

    IF OBJECT_ID(N'dbo.HMR_SUBMISSION_CONFIG_WINDOW', N'U') IS NULL
    BEGIN
        EXEC(N'
CREATE TABLE [dbo].[HMR_SUBMISSION_CONFIG_WINDOW]
(
    [SUBMISSION_CONFIG_WINDOW_ID] NUMERIC(9, 0) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_WIN_ID_DF]
        DEFAULT (NEXT VALUE FOR [dbo].[HMR_SUBM_CFG_WIN_ID_SEQ]),
    [SUBMISSION_CONFIGURATION_ID] NUMERIC(9, 0) NOT NULL,
    [WINDOW_TYPE] VARCHAR(20) NOT NULL,
    [START_MONTH] TINYINT NOT NULL,
    [START_DAY] TINYINT NOT NULL,
    [END_MONTH] TINYINT NOT NULL,
    [END_DAY] TINYINT NOT NULL,
    [CONCURRENCY_CONTROL_NUMBER] BIGINT NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_WIN_CC_DF] DEFAULT (1),
    [APP_CREATE_USERID] VARCHAR(30) NOT NULL,
    [APP_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_CREATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_CREATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [APP_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [APP_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_LAST_UPDATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_LAST_UPDATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_WIN_DB_CRT_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_WIN_DB_CRT_TS_DF] DEFAULT (GETUTCDATE()),
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_WIN_DB_UPD_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_WIN_DB_UPD_TS_DF] DEFAULT (GETUTCDATE())
);');
    END;

    IF OBJECT_ID(N'dbo.HMR_SUBMISSION_CONFIG_RULE', N'U') IS NULL
    BEGIN
        EXEC(N'
CREATE TABLE [dbo].[HMR_SUBMISSION_CONFIG_RULE]
(
    [SUBMISSION_CONFIG_RULE_ID] NUMERIC(9, 0) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RULE_ID_DF]
        DEFAULT (NEXT VALUE FOR [dbo].[HMR_SUBM_CFG_RULE_ID_SEQ]),
    [SUBMISSION_CONFIGURATION_ID] NUMERIC(9, 0) NOT NULL,
    [RULE_TYPE] VARCHAR(30) NOT NULL,
    [DISPLAY_LABEL] VARCHAR(150) NOT NULL,
    [COMPARISON_OPERATOR] VARCHAR(3) NOT NULL,
    [THRESHOLD_VALUE] DECIMAL(18, 4) NOT NULL,
    [UNIT_OF_MEASURE] VARCHAR(30) NOT NULL,
    [DISPLAY_ORDER] SMALLINT NOT NULL,
    [CONCURRENCY_CONTROL_NUMBER] BIGINT NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RULE_CC_DF] DEFAULT (1),
    [APP_CREATE_USERID] VARCHAR(30) NOT NULL,
    [APP_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_CREATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_CREATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [APP_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [APP_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_LAST_UPDATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_LAST_UPDATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RULE_DB_CRT_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RULE_DB_CRT_TS_DF] DEFAULT (GETUTCDATE()),
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RULE_DB_UPD_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RULE_DB_UPD_TS_DF] DEFAULT (GETUTCDATE())
);');
    END;

    IF OBJECT_ID(N'dbo.HMR_SUBMISSION_CONFIG_RULE_ACTIVITY', N'U') IS NULL
    BEGIN
        EXEC(N'
CREATE TABLE [dbo].[HMR_SUBMISSION_CONFIG_RULE_ACTIVITY]
(
    [SUBMISSION_CONFIG_RULE_ACTIVITY_ID] NUMERIC(9, 0) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RACT_ID_DF]
        DEFAULT (NEXT VALUE FOR [dbo].[HMR_SUBM_CFG_RACT_ID_SEQ]),
    [SUBMISSION_CONFIG_RULE_ID] NUMERIC(9, 0) NOT NULL,
    [ACTIVITY_CODE_ID] NUMERIC(9, 0) NOT NULL,
    [CONCURRENCY_CONTROL_NUMBER] BIGINT NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RACT_CC_DF] DEFAULT (1),
    [APP_CREATE_USERID] VARCHAR(30) NOT NULL,
    [APP_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_CREATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_CREATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [APP_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [APP_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_LAST_UPDATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_LAST_UPDATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RACT_DB_CRT_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RACT_DB_CRT_TS_DF] DEFAULT (GETUTCDATE()),
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RACT_DB_UPD_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_RACT_DB_UPD_TS_DF] DEFAULT (GETUTCDATE())
);');
    END;

    IF OBJECT_ID(N'dbo.HMR_SUBMISSION_CONFIG_AUDIENCE', N'U') IS NULL
    BEGIN
        EXEC(N'
CREATE TABLE [dbo].[HMR_SUBMISSION_CONFIG_AUDIENCE]
(
    [SUBMISSION_CONFIG_AUDIENCE_ID] NUMERIC(9, 0) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_AUD_ID_DF]
        DEFAULT (NEXT VALUE FOR [dbo].[HMR_SUBM_CFG_AUD_ID_SEQ]),
    [SUBMISSION_CONFIGURATION_ID] NUMERIC(9, 0) NOT NULL,
    [AUDIENCE_TYPE] VARCHAR(30) NOT NULL,
    [AUDIENCE_VALUE] VARCHAR(50) NOT NULL,
    [CONCURRENCY_CONTROL_NUMBER] BIGINT NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_AUD_CC_DF] DEFAULT (1),
    [APP_CREATE_USERID] VARCHAR(30) NOT NULL,
    [APP_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_CREATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_CREATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [APP_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [APP_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_LAST_UPDATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_LAST_UPDATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_AUD_DB_CRT_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_AUD_DB_CRT_TS_DF] DEFAULT (GETUTCDATE()),
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_AUD_DB_UPD_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_AUD_DB_UPD_TS_DF] DEFAULT (GETUTCDATE())
);');
    END;

    IF OBJECT_ID(N'dbo.HMR_SUBMISSION_CONFIG_SERVICE_AREA', N'U') IS NULL
    BEGIN
        EXEC(N'
CREATE TABLE [dbo].[HMR_SUBMISSION_CONFIG_SERVICE_AREA]
(
    [SUBMISSION_CONFIG_SERVICE_AREA_ID] NUMERIC(9, 0) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_SA_ID_DF]
        DEFAULT (NEXT VALUE FOR [dbo].[HMR_SUBM_CFG_SA_ID_SEQ]),
    [SUBMISSION_CONFIGURATION_ID] NUMERIC(9, 0) NOT NULL,
    [SERVICE_AREA_NUMBER] NUMERIC(9, 0) NOT NULL,
    [CONCURRENCY_CONTROL_NUMBER] BIGINT NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_SA_CC_DF] DEFAULT (1),
    [APP_CREATE_USERID] VARCHAR(30) NOT NULL,
    [APP_CREATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_CREATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_CREATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [APP_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL,
    [APP_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL,
    [APP_LAST_UPDATE_USER_GUID] UNIQUEIDENTIFIER NOT NULL,
    [APP_LAST_UPDATE_USER_DIRECTORY] VARCHAR(12) NOT NULL,
    [DB_AUDIT_CREATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_SA_DB_CRT_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_CREATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_SA_DB_CRT_TS_DF] DEFAULT (GETUTCDATE()),
    [DB_AUDIT_LAST_UPDATE_USERID] VARCHAR(30) NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_SA_DB_UPD_USR_DF] DEFAULT (USER_NAME()),
    [DB_AUDIT_LAST_UPDATE_TIMESTAMP] DATETIME NOT NULL
        CONSTRAINT [HMR_SUBM_CFG_SA_DB_UPD_TS_DF] DEFAULT (GETUTCDATE())
);');
    END;

    /* ------------------------------------------------------------------ */
    /* Keys, checks, foreign keys, and indexes                            */
    /* ------------------------------------------------------------------ */

    DECLARE @TableName SYSNAME;
    DECLARE @ConstraintName SYSNAME;
    DECLARE @Definition NVARCHAR(2000);
    DECLARE @Sql NVARCHAR(MAX);

    DECLARE @PrimaryKeys TABLE
    (
        [TableName] SYSNAME NOT NULL,
        [ConstraintName] SYSNAME NOT NULL,
        [ColumnName] SYSNAME NOT NULL
    );

    INSERT INTO @PrimaryKeys ([TableName], [ConstraintName], [ColumnName])
    VALUES
        (N'HMR_SUBMISSION_CONFIGURATION', N'HMR_SUBM_CFG_PK', N'SUBMISSION_CONFIGURATION_ID'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'HMR_SUBM_CFG_WIN_PK', N'SUBMISSION_CONFIG_WINDOW_ID'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'HMR_SUBM_CFG_RULE_PK', N'SUBMISSION_CONFIG_RULE_ID'),
        (N'HMR_SUBMISSION_CONFIG_RULE_ACTIVITY', N'HMR_SUBM_CFG_RACT_PK', N'SUBMISSION_CONFIG_RULE_ACTIVITY_ID'),
        (N'HMR_SUBMISSION_CONFIG_AUDIENCE', N'HMR_SUBM_CFG_AUD_PK', N'SUBMISSION_CONFIG_AUDIENCE_ID'),
        (N'HMR_SUBMISSION_CONFIG_SERVICE_AREA', N'HMR_SUBM_CFG_SA_PK', N'SUBMISSION_CONFIG_SERVICE_AREA_ID');

    DECLARE @ColumnName SYSNAME;
    DECLARE primary_key_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT [TableName], [ConstraintName], [ColumnName] FROM @PrimaryKeys;

    OPEN primary_key_cursor;
    FETCH NEXT FROM primary_key_cursor INTO @TableName, @ConstraintName, @ColumnName;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS
        (
            SELECT 1 FROM sys.key_constraints
            WHERE [parent_object_id] = OBJECT_ID(N'dbo.' + @TableName)
              AND [type] = 'PK'
        )
        BEGIN
            SET @Sql = N'ALTER TABLE [dbo].' + QUOTENAME(@TableName)
                + N' ADD CONSTRAINT ' + QUOTENAME(@ConstraintName)
                + N' PRIMARY KEY CLUSTERED (' + QUOTENAME(@ColumnName) + N');';
            EXEC sys.sp_executesql @Sql;
        END;
        FETCH NEXT FROM primary_key_cursor INTO @TableName, @ConstraintName, @ColumnName;
    END;
    CLOSE primary_key_cursor;
    DEALLOCATE primary_key_cursor;

    DECLARE @UniqueKeys TABLE
    (
        [TableName] SYSNAME NOT NULL,
        [ConstraintName] SYSNAME NOT NULL,
        [ColumnList] NVARCHAR(1000) NOT NULL
    );

    INSERT INTO @UniqueKeys ([TableName], [ConstraintName], [ColumnList])
    VALUES
        (N'HMR_SUBMISSION_CONFIGURATION', N'HMR_SUBM_CFG_KEY_UK', N'[CONFIGURATION_KEY]'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'HMR_SUBM_CFG_WIN_UK', N'[SUBMISSION_CONFIGURATION_ID], [WINDOW_TYPE]'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'HMR_SUBM_CFG_RULE_ORDER_UK', N'[SUBMISSION_CONFIGURATION_ID], [DISPLAY_ORDER]'),
        (N'HMR_SUBMISSION_CONFIG_RULE_ACTIVITY', N'HMR_SUBM_CFG_RACT_UK', N'[SUBMISSION_CONFIG_RULE_ID], [ACTIVITY_CODE_ID]'),
        (N'HMR_SUBMISSION_CONFIG_AUDIENCE', N'HMR_SUBM_CFG_AUD_UK', N'[SUBMISSION_CONFIGURATION_ID], [AUDIENCE_TYPE], [AUDIENCE_VALUE]'),
        (N'HMR_SUBMISSION_CONFIG_SERVICE_AREA', N'HMR_SUBM_CFG_SA_UK', N'[SUBMISSION_CONFIGURATION_ID], [SERVICE_AREA_NUMBER]');

    DECLARE unique_key_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT [TableName], [ConstraintName], [ColumnList] FROM @UniqueKeys;

    OPEN unique_key_cursor;
    FETCH NEXT FROM unique_key_cursor INTO @TableName, @ConstraintName, @Definition;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS
        (
            SELECT 1 FROM sys.key_constraints
            WHERE [parent_object_id] = OBJECT_ID(N'dbo.' + @TableName)
              AND [name] = @ConstraintName
        )
        BEGIN
            SET @Sql = N'ALTER TABLE [dbo].' + QUOTENAME(@TableName)
                + N' ADD CONSTRAINT ' + QUOTENAME(@ConstraintName)
                + N' UNIQUE NONCLUSTERED (' + @Definition + N');';
            EXEC sys.sp_executesql @Sql;
        END;
        FETCH NEXT FROM unique_key_cursor INTO @TableName, @ConstraintName, @Definition;
    END;
    CLOSE unique_key_cursor;
    DEALLOCATE unique_key_cursor;

    DECLARE @Checks TABLE
    (
        [TableName] SYSNAME NOT NULL,
        [ConstraintName] SYSNAME NOT NULL,
        [Predicate] NVARCHAR(2000) NOT NULL
    );

    INSERT INTO @Checks ([TableName], [ConstraintName], [Predicate])
    VALUES
        (N'HMR_SUBMISSION_CONFIGURATION', N'HMR_SUBM_CFG_DATE_CK', N'[EFFECTIVE_TO_DATE] IS NULL OR [EFFECTIVE_TO_DATE] >= [EFFECTIVE_FROM_DATE]'),
        (N'HMR_SUBMISSION_CONFIGURATION', N'HMR_SUBM_CFG_SCOPE_CK', N'[SCOPE_TYPE] IN (''GLOBAL'', ''SERVICE_AREA'')'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'HMR_SUBM_CFG_WIN_TYPE_CK', N'[WINDOW_TYPE] IN (''REMINDER'', ''BLACKOUT'')'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'HMR_SUBM_CFG_WIN_START_CK', N'[START_MONTH] BETWEEN 1 AND 12 AND [START_DAY] BETWEEN 1 AND CASE WHEN [START_MONTH] = 2 THEN 29 WHEN [START_MONTH] IN (4, 6, 9, 11) THEN 30 ELSE 31 END'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'HMR_SUBM_CFG_WIN_END_CK', N'[END_MONTH] BETWEEN 1 AND 12 AND [END_DAY] BETWEEN 1 AND CASE WHEN [END_MONTH] = 2 THEN 29 WHEN [END_MONTH] IN (4, 6, 9, 11) THEN 30 ELSE 31 END'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'HMR_SUBM_CFG_RULE_TYPE_CK', N'[RULE_TYPE] IN (''ACTIVITY_ACCOMPLISHMENT'')'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'HMR_SUBM_CFG_RULE_OP_CK', N'[COMPARISON_OPERATOR] IN (''GT'', ''GTE'')'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'HMR_SUBM_CFG_RULE_THRESH_CK', N'[THRESHOLD_VALUE] >= 0'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'HMR_SUBM_CFG_RULE_ORDER_CK', N'[DISPLAY_ORDER] > 0'),
        (N'HMR_SUBMISSION_CONFIG_AUDIENCE', N'HMR_SUBM_CFG_AUD_TYPE_CK', N'[AUDIENCE_TYPE] IN (''USER_TYPE'', ''ROLE'')');

    DECLARE check_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT [TableName], [ConstraintName], [Predicate] FROM @Checks;

    OPEN check_cursor;
    FETCH NEXT FROM check_cursor INTO @TableName, @ConstraintName, @Definition;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS
        (
            SELECT 1 FROM sys.check_constraints
            WHERE [parent_object_id] = OBJECT_ID(N'dbo.' + @TableName)
              AND [name] = @ConstraintName
        )
        BEGIN
            SET @Sql = N'ALTER TABLE [dbo].' + QUOTENAME(@TableName)
                + N' WITH CHECK ADD CONSTRAINT ' + QUOTENAME(@ConstraintName)
                + N' CHECK (' + @Definition + N');';
            EXEC sys.sp_executesql @Sql;
        END;
        FETCH NEXT FROM check_cursor INTO @TableName, @ConstraintName, @Definition;
    END;
    CLOSE check_cursor;
    DEALLOCATE check_cursor;

    DECLARE @ForeignKeys TABLE
    (
        [TableName] SYSNAME NOT NULL,
        [ConstraintName] SYSNAME NOT NULL,
        [Definition] NVARCHAR(2000) NOT NULL
    );

    INSERT INTO @ForeignKeys ([TableName], [ConstraintName], [Definition])
    VALUES
        (N'HMR_SUBMISSION_CONFIGURATION', N'HMR_SUBM_CFG_STREAM_FK', N'FOREIGN KEY ([SUBMISSION_STREAM_ID]) REFERENCES [dbo].[HMR_SUBMISSION_STREAM] ([SUBMISSION_STREAM_ID])'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'HMR_SUBM_CFG_WIN_CFG_FK', N'FOREIGN KEY ([SUBMISSION_CONFIGURATION_ID]) REFERENCES [dbo].[HMR_SUBMISSION_CONFIGURATION] ([SUBMISSION_CONFIGURATION_ID])'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'HMR_SUBM_CFG_RULE_CFG_FK', N'FOREIGN KEY ([SUBMISSION_CONFIGURATION_ID]) REFERENCES [dbo].[HMR_SUBMISSION_CONFIGURATION] ([SUBMISSION_CONFIGURATION_ID])'),
        (N'HMR_SUBMISSION_CONFIG_RULE_ACTIVITY', N'HMR_SUBM_CFG_RACT_RULE_FK', N'FOREIGN KEY ([SUBMISSION_CONFIG_RULE_ID]) REFERENCES [dbo].[HMR_SUBMISSION_CONFIG_RULE] ([SUBMISSION_CONFIG_RULE_ID])'),
        (N'HMR_SUBMISSION_CONFIG_RULE_ACTIVITY', N'HMR_SUBM_CFG_RACT_ACT_FK', N'FOREIGN KEY ([ACTIVITY_CODE_ID]) REFERENCES [dbo].[HMR_ACTIVITY_CODE] ([ACTIVITY_CODE_ID])'),
        (N'HMR_SUBMISSION_CONFIG_AUDIENCE', N'HMR_SUBM_CFG_AUD_CFG_FK', N'FOREIGN KEY ([SUBMISSION_CONFIGURATION_ID]) REFERENCES [dbo].[HMR_SUBMISSION_CONFIGURATION] ([SUBMISSION_CONFIGURATION_ID])'),
        (N'HMR_SUBMISSION_CONFIG_SERVICE_AREA', N'HMR_SUBM_CFG_SA_CFG_FK', N'FOREIGN KEY ([SUBMISSION_CONFIGURATION_ID]) REFERENCES [dbo].[HMR_SUBMISSION_CONFIGURATION] ([SUBMISSION_CONFIGURATION_ID])'),
        (N'HMR_SUBMISSION_CONFIG_SERVICE_AREA', N'HMR_SUBM_CFG_SA_AREA_FK', N'FOREIGN KEY ([SERVICE_AREA_NUMBER]) REFERENCES [dbo].[HMR_SERVICE_AREA] ([SERVICE_AREA_NUMBER])');

    DECLARE foreign_key_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT [TableName], [ConstraintName], [Definition] FROM @ForeignKeys;

    OPEN foreign_key_cursor;
    FETCH NEXT FROM foreign_key_cursor INTO @TableName, @ConstraintName, @Definition;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS
        (
            SELECT 1 FROM sys.foreign_keys
            WHERE [parent_object_id] = OBJECT_ID(N'dbo.' + @TableName)
              AND [name] = @ConstraintName
        )
        BEGIN
            SET @Sql = N'ALTER TABLE [dbo].' + QUOTENAME(@TableName)
                + N' WITH CHECK ADD CONSTRAINT ' + QUOTENAME(@ConstraintName)
                + N' ' + @Definition + N';';
            EXEC sys.sp_executesql @Sql;
        END;
        FETCH NEXT FROM foreign_key_cursor INTO @TableName, @ConstraintName, @Definition;
    END;
    CLOSE foreign_key_cursor;
    DEALLOCATE foreign_key_cursor;

    DECLARE @Indexes TABLE
    (
        [TableName] SYSNAME NOT NULL,
        [IndexName] SYSNAME NOT NULL,
        [ColumnList] NVARCHAR(1000) NOT NULL
    );

    INSERT INTO @Indexes ([TableName], [IndexName], [ColumnList])
    VALUES
        (N'HMR_SUBMISSION_CONFIGURATION', N'HMR_SUBM_CFG_STREAM_FK_I', N'[SUBMISSION_STREAM_ID], [IS_ACTIVE]'),
        (N'HMR_SUBMISSION_CONFIG_RULE_ACTIVITY', N'HMR_SUBM_CFG_RACT_ACT_FK_I', N'[ACTIVITY_CODE_ID]'),
        (N'HMR_SUBMISSION_CONFIG_SERVICE_AREA', N'HMR_SUBM_CFG_SA_AREA_FK_I', N'[SERVICE_AREA_NUMBER]');

    DECLARE index_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT [TableName], [IndexName], [ColumnList] FROM @Indexes;

    OPEN index_cursor;
    FETCH NEXT FROM index_cursor INTO @TableName, @ConstraintName, @Definition;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS
        (
            SELECT 1 FROM sys.indexes
            WHERE [object_id] = OBJECT_ID(N'dbo.' + @TableName)
              AND [name] = @ConstraintName
        )
        BEGIN
            SET @Sql = N'CREATE NONCLUSTERED INDEX ' + QUOTENAME(@ConstraintName)
                + N' ON [dbo].' + QUOTENAME(@TableName)
                + N' (' + @Definition + N');';
            EXEC sys.sp_executesql @Sql;
        END;
        FETCH NEXT FROM index_cursor INTO @TableName, @ConstraintName, @Definition;
    END;
    CLOSE index_cursor;
    DEALLOCATE index_cursor;

    /* ------------------------------------------------------------------ */
    /* Audit and optimistic-concurrency triggers                          */
    /* ------------------------------------------------------------------ */

    DECLARE @TriggerTables TABLE
    (
        [TableName] SYSNAME NOT NULL,
        [PrimaryKeyColumn] SYSNAME NOT NULL,
        [InsertTriggerName] SYSNAME NOT NULL,
        [UpdateTriggerName] SYSNAME NOT NULL
    );

    INSERT INTO @TriggerTables
        ([TableName], [PrimaryKeyColumn], [InsertTriggerName], [UpdateTriggerName])
    VALUES
        (N'HMR_SUBMISSION_CONFIGURATION', N'SUBMISSION_CONFIGURATION_ID', N'HMR_SUBM_CFG_I_S_I_TR', N'HMR_SUBM_CFG_I_S_U_TR'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'SUBMISSION_CONFIG_WINDOW_ID', N'HMR_SUBM_CFG_WIN_I_S_I_TR', N'HMR_SUBM_CFG_WIN_I_S_U_TR'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'SUBMISSION_CONFIG_RULE_ID', N'HMR_SUBM_CFG_RULE_I_S_I_TR', N'HMR_SUBM_CFG_RULE_I_S_U_TR'),
        (N'HMR_SUBMISSION_CONFIG_RULE_ACTIVITY', N'SUBMISSION_CONFIG_RULE_ACTIVITY_ID', N'HMR_SUBM_CFG_RACT_I_S_I_TR', N'HMR_SUBM_CFG_RACT_I_S_U_TR'),
        (N'HMR_SUBMISSION_CONFIG_AUDIENCE', N'SUBMISSION_CONFIG_AUDIENCE_ID', N'HMR_SUBM_CFG_AUD_I_S_I_TR', N'HMR_SUBM_CFG_AUD_I_S_U_TR'),
        (N'HMR_SUBMISSION_CONFIG_SERVICE_AREA', N'SUBMISSION_CONFIG_SERVICE_AREA_ID', N'HMR_SUBM_CFG_SA_I_S_I_TR', N'HMR_SUBM_CFG_SA_I_S_U_TR');

    DECLARE @PrimaryKeyColumn SYSNAME;
    DECLARE @InsertTriggerName SYSNAME;
    DECLARE @UpdateTriggerName SYSNAME;
    DECLARE @InsertColumns NVARCHAR(MAX);
    DECLARE @UpdateAssignments NVARCHAR(MAX);

    DECLARE trigger_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT [TableName], [PrimaryKeyColumn], [InsertTriggerName], [UpdateTriggerName]
        FROM @TriggerTables;

    OPEN trigger_cursor;
    FETCH NEXT FROM trigger_cursor
        INTO @TableName, @PrimaryKeyColumn, @InsertTriggerName, @UpdateTriggerName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SELECT @InsertColumns = STUFF
        (
            (
                SELECT N', ' + QUOTENAME(c.[name])
                FROM sys.columns c
                WHERE c.[object_id] = OBJECT_ID(N'dbo.' + @TableName)
                  AND c.[name] NOT IN
                  (
                      N'DB_AUDIT_CREATE_USERID',
                      N'DB_AUDIT_CREATE_TIMESTAMP',
                      N'DB_AUDIT_LAST_UPDATE_USERID',
                      N'DB_AUDIT_LAST_UPDATE_TIMESTAMP'
                  )
                ORDER BY c.[column_id]
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'),
            1,
            2,
            N''
        );

        SELECT @UpdateAssignments = STUFF
        (
            (
                SELECT N', ' + QUOTENAME(c.[name])
                    + N' = source.' + QUOTENAME(c.[name])
                FROM sys.columns c
                WHERE c.[object_id] = OBJECT_ID(N'dbo.' + @TableName)
                  AND c.[name] <> @PrimaryKeyColumn
                  AND c.[name] NOT IN
                  (
                      N'APP_CREATE_USERID',
                      N'APP_CREATE_TIMESTAMP',
                      N'APP_CREATE_USER_GUID',
                      N'APP_CREATE_USER_DIRECTORY',
                      N'DB_AUDIT_CREATE_USERID',
                      N'DB_AUDIT_CREATE_TIMESTAMP',
                      N'DB_AUDIT_LAST_UPDATE_USERID',
                      N'DB_AUDIT_LAST_UPDATE_TIMESTAMP'
                  )
                ORDER BY c.[column_id]
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'),
            1,
            2,
            N''
        );

        SET @Sql = N'
CREATE OR ALTER TRIGGER [dbo].' + QUOTENAME(@InsertTriggerName) + N'
ON [dbo].' + QUOTENAME(@TableName) + N'
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM inserted)
            RETURN;

        INSERT INTO [dbo].' + QUOTENAME(@TableName) + N'
        (' + @InsertColumns + N')
        SELECT ' + @InsertColumns + N'
        FROM inserted;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        EXEC [dbo].[hmr_error_handling];
    END CATCH;
END;';
        EXEC sys.sp_executesql @Sql;

        SET @Sql = N'
CREATE OR ALTER TRIGGER [dbo].' + QUOTENAME(@UpdateTriggerName) + N'
ON [dbo].' + QUOTENAME(@TableName) + N'
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
                ON d.' + QUOTENAME(@PrimaryKeyColumn) + N' = i.' + QUOTENAME(@PrimaryKeyColumn) + N'
            WHERE i.[CONCURRENCY_CONTROL_NUMBER] <> d.[CONCURRENCY_CONTROL_NUMBER] + 1
        )
            RAISERROR(''CONCURRENCY FAILURE.'', 16, 1);

        UPDATE target
        SET ' + @UpdateAssignments + N',
            [DB_AUDIT_LAST_UPDATE_TIMESTAMP] = GETUTCDATE(),
            [DB_AUDIT_LAST_UPDATE_USERID] = USER_NAME()
        FROM [dbo].' + QUOTENAME(@TableName) + N' target
        INNER JOIN inserted source
            ON source.' + QUOTENAME(@PrimaryKeyColumn) + N' = target.' + QUOTENAME(@PrimaryKeyColumn) + N';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        EXEC [dbo].[hmr_error_handling];
    END CATCH;
END;';
        EXEC sys.sp_executesql @Sql;

        FETCH NEXT FROM trigger_cursor
            INTO @TableName, @PrimaryKeyColumn, @InsertTriggerName, @UpdateTriggerName;
    END;

    CLOSE trigger_cursor;
    DEALLOCATE trigger_cursor;

    /* ------------------------------------------------------------------ */
    /* Initial capitalized hard-surfacing configuration                   */
    /* ------------------------------------------------------------------ */

    DECLARE @WorkReportStreamId NUMERIC(9, 0);
    DECLARE @WorkReportStreamCount INT;

    SELECT
        @WorkReportStreamId = MIN([SUBMISSION_STREAM_ID]),
        @WorkReportStreamCount = COUNT(*)
    FROM [dbo].[HMR_SUBMISSION_STREAM]
    WHERE [STAGING_TABLE_NAME] = 'HMR_WORK_REPORT';

    IF @WorkReportStreamCount <> 1
        THROW 50001, 'Expected exactly one HMR_WORK_REPORT submission stream.', 1;

    IF
    (
        SELECT COUNT(*)
        FROM [dbo].[HMR_ACTIVITY_CODE]
        WHERE [ACTIVITY_NUMBER] IN ('101300', '101301', '101304', '933300', '102300', '102301')
    ) <> 6
        THROW 50002, 'One or more hard-surfacing activity codes are missing or duplicated.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[HMR_SUBMISSION_CONFIGURATION]
        WHERE [CONFIGURATION_KEY] = 'CAPITALIZED_HARD_SURFACING'
    )
    BEGIN
        INSERT INTO [dbo].[HMR_SUBMISSION_CONFIGURATION]
        (
            [CONFIGURATION_KEY],
            [CONFIGURATION_NAME],
            [SUBMISSION_STREAM_ID],
            [IS_ACTIVE],
            [EFFECTIVE_FROM_DATE],
            [EFFECTIVE_TO_DATE],
            [TIME_ZONE_ID],
            [SCOPE_TYPE],
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
            'CAPITALIZED_HARD_SURFACING',
            'Capitalized Hard Surfacing Works',
            @WorkReportStreamId,
            1,
            '20270101',
            NULL,
            'America/Vancouver',
            'GLOBAL',
            @MigrationUserId,
            @MigrationTimestamp,
            @MigrationUserGuid,
            @MigrationUserDirectory,
            @MigrationUserId,
            @MigrationTimestamp,
            @MigrationUserGuid,
            @MigrationUserDirectory
        );
    END;

    DECLARE @SubmissionConfigurationId NUMERIC(9, 0);
    SELECT @SubmissionConfigurationId = [SUBMISSION_CONFIGURATION_ID]
    FROM [dbo].[HMR_SUBMISSION_CONFIGURATION]
    WHERE [CONFIGURATION_KEY] = 'CAPITALIZED_HARD_SURFACING';

    INSERT INTO [dbo].[HMR_SUBMISSION_CONFIG_WINDOW]
    (
        [SUBMISSION_CONFIGURATION_ID],
        [WINDOW_TYPE],
        [START_MONTH],
        [START_DAY],
        [END_MONTH],
        [END_DAY],
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
        @SubmissionConfigurationId,
        source.[WINDOW_TYPE],
        source.[START_MONTH],
        source.[START_DAY],
        source.[END_MONTH],
        source.[END_DAY],
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory,
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory
    FROM
    (
        VALUES
            ('REMINDER', CONVERT(TINYINT, 1), CONVERT(TINYINT, 1), CONVERT(TINYINT, 1), CONVERT(TINYINT, 31)),
            ('BLACKOUT', CONVERT(TINYINT, 2), CONVERT(TINYINT, 1), CONVERT(TINYINT, 2), CONVERT(TINYINT, 29))
    ) source ([WINDOW_TYPE], [START_MONTH], [START_DAY], [END_MONTH], [END_DAY])
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[HMR_SUBMISSION_CONFIG_WINDOW] existing
        WHERE existing.[SUBMISSION_CONFIGURATION_ID] = @SubmissionConfigurationId
          AND existing.[WINDOW_TYPE] = source.[WINDOW_TYPE]
    );

    INSERT INTO [dbo].[HMR_SUBMISSION_CONFIG_RULE]
    (
        [SUBMISSION_CONFIGURATION_ID],
        [RULE_TYPE],
        [DISPLAY_LABEL],
        [COMPARISON_OPERATOR],
        [THRESHOLD_VALUE],
        [UNIT_OF_MEASURE],
        [DISPLAY_ORDER],
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
        @SubmissionConfigurationId,
        'ACTIVITY_ACCOMPLISHMENT',
        source.[DISPLAY_LABEL],
        source.[COMPARISON_OPERATOR],
        source.[THRESHOLD_VALUE],
        source.[UNIT_OF_MEASURE],
        source.[DISPLAY_ORDER],
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory,
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory
    FROM
    (
        VALUES
            ('Asphalt paving activities', 'GT', CONVERT(DECIMAL(18, 4), 450), 'tonne', CONVERT(SMALLINT, 1)),
            ('Graded aggregate seal activities', 'GTE', CONVERT(DECIMAL(18, 4), 7000), 'm2', CONVERT(SMALLINT, 2))
    ) source ([DISPLAY_LABEL], [COMPARISON_OPERATOR], [THRESHOLD_VALUE], [UNIT_OF_MEASURE], [DISPLAY_ORDER])
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[HMR_SUBMISSION_CONFIG_RULE] existing
        WHERE existing.[SUBMISSION_CONFIGURATION_ID] = @SubmissionConfigurationId
          AND existing.[DISPLAY_ORDER] = source.[DISPLAY_ORDER]
    );

    INSERT INTO [dbo].[HMR_SUBMISSION_CONFIG_RULE_ACTIVITY]
    (
        [SUBMISSION_CONFIG_RULE_ID],
        [ACTIVITY_CODE_ID],
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
        configuredRule.[SUBMISSION_CONFIG_RULE_ID],
        activity.[ACTIVITY_CODE_ID],
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory,
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory
    FROM
    (
        VALUES
            (CONVERT(SMALLINT, 1), '101300'),
            (CONVERT(SMALLINT, 1), '101301'),
            (CONVERT(SMALLINT, 1), '101304'),
            (CONVERT(SMALLINT, 1), '933300'),
            (CONVERT(SMALLINT, 2), '102300'),
            (CONVERT(SMALLINT, 2), '102301')
    ) source ([DISPLAY_ORDER], [ACTIVITY_NUMBER])
    INNER JOIN [dbo].[HMR_SUBMISSION_CONFIG_RULE] configuredRule
        ON configuredRule.[SUBMISSION_CONFIGURATION_ID] = @SubmissionConfigurationId
       AND configuredRule.[DISPLAY_ORDER] = source.[DISPLAY_ORDER]
    INNER JOIN [dbo].[HMR_ACTIVITY_CODE] activity
        ON activity.[ACTIVITY_NUMBER] = source.[ACTIVITY_NUMBER]
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[HMR_SUBMISSION_CONFIG_RULE_ACTIVITY] existing
        WHERE existing.[SUBMISSION_CONFIG_RULE_ID] = configuredRule.[SUBMISSION_CONFIG_RULE_ID]
          AND existing.[ACTIVITY_CODE_ID] = activity.[ACTIVITY_CODE_ID]
    );

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[HMR_SUBMISSION_CONFIG_AUDIENCE]
        WHERE [SUBMISSION_CONFIGURATION_ID] = @SubmissionConfigurationId
          AND [AUDIENCE_TYPE] = 'USER_TYPE'
          AND [AUDIENCE_VALUE] = 'BUSINESS'
    )
    BEGIN
        INSERT INTO [dbo].[HMR_SUBMISSION_CONFIG_AUDIENCE]
        (
            [SUBMISSION_CONFIGURATION_ID],
            [AUDIENCE_TYPE],
            [AUDIENCE_VALUE],
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
            @SubmissionConfigurationId,
            'USER_TYPE',
            'BUSINESS',
            @MigrationUserId,
            @MigrationTimestamp,
            @MigrationUserGuid,
            @MigrationUserDirectory,
            @MigrationUserId,
            @MigrationTimestamp,
            @MigrationUserGuid,
            @MigrationUserDirectory
        );
    END;

    /* ------------------------------------------------------------------ */
    /* Application permissions                                             */
    /* ------------------------------------------------------------------ */

    INSERT INTO [dbo].[HMR_PERMISSION]
    (
        [NAME],
        [DESCRIPTION],
        [END_DATE],
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
        source.[NAME],
        source.[DESCRIPTION],
        NULL,
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory,
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory
    FROM
    (
        VALUES
            ('SUB_CONFIG_W', 'Submission Configuration Write'),
            ('WORK_REPORT_W', 'Work Report Upload Write')
    ) source ([NAME], [DESCRIPTION])
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[HMR_PERMISSION] existing
        WHERE existing.[NAME] = source.[NAME]
    );

    DECLARE @RoleVariants TABLE
    (
        [LogicalRole] VARCHAR(30) NOT NULL,
        [RoleName] VARCHAR(30) NOT NULL
    );

    INSERT INTO @RoleVariants ([LogicalRole], [RoleName])
    VALUES
        ('SYSTEM_ADMIN', 'SYSTEM_ADMIN'),
        ('SYSTEM_ADMIN', 'System Administrator'),
        ('SYSTEM_ADMIN', '1 System Administrator'),
        ('DISTRICT_ADMIN', 'DISTRICT_ADMIN'),
        ('DISTRICT_ADMIN', 'District Administrator'),
        ('DISTRICT_ADMIN', '2 District Administrator'),
        ('MOTI_STAFF', 'STAFF'),
        ('MOTI_STAFF', 'MoTI Staff'),
        ('MOTI_STAFF', '4 MoTI Staff'),
        ('MANAGER_OPERATION', 'MANAGER_OPERATION'),
        ('MANAGER_OPERATION', 'Manager/Operations'),
        ('MANAGER_OPERATION', '3 Manager/Operations'),
        ('MAINT_CONTRACTOR', 'MAINT_CONTRACTOR'),
        ('MAINT_CONTRACTOR', 'Maintenance Contractor'),
        ('MAINT_CONTRACTOR', '5 Maintenance Contractor');

    DECLARE @RolePermissionTargets TABLE
    (
        [LogicalRole] VARCHAR(30) NOT NULL,
        [PermissionName] VARCHAR(30) NOT NULL
    );

    INSERT INTO @RolePermissionTargets ([LogicalRole], [PermissionName])
    VALUES
        ('SYSTEM_ADMIN', 'SUB_CONFIG_W'),
        ('DISTRICT_ADMIN', 'SUB_CONFIG_W'),
        ('SYSTEM_ADMIN', 'WORK_REPORT_W'),
        ('DISTRICT_ADMIN', 'WORK_REPORT_W'),
        ('MOTI_STAFF', 'WORK_REPORT_W'),
        ('MANAGER_OPERATION', 'WORK_REPORT_W'),
        ('MAINT_CONTRACTOR', 'WORK_REPORT_W');

    IF EXISTS
    (
        SELECT 1
        FROM @RolePermissionTargets target
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM @RoleVariants variant
            INNER JOIN [dbo].[HMR_ROLE] role
                ON role.[NAME] = variant.[RoleName]
            WHERE variant.[LogicalRole] = target.[LogicalRole]
              AND (role.[END_DATE] IS NULL OR role.[END_DATE] >= CONVERT(DATE, GETUTCDATE()))
        )
    )
        THROW 50003, 'One or more target HMCR roles could not be resolved.', 1;

    ;WITH permission_ids AS
    (
        SELECT [NAME], MIN([PERMISSION_ID]) AS [PERMISSION_ID]
        FROM [dbo].[HMR_PERMISSION]
        WHERE [NAME] IN ('SUB_CONFIG_W', 'WORK_REPORT_W')
        GROUP BY [NAME]
    ),
    resolved_targets AS
    (
        SELECT DISTINCT
            role.[ROLE_ID],
            permission_ids.[PERMISSION_ID]
        FROM @RolePermissionTargets target
        INNER JOIN @RoleVariants variant
            ON variant.[LogicalRole] = target.[LogicalRole]
        INNER JOIN [dbo].[HMR_ROLE] role
            ON role.[NAME] = variant.[RoleName]
        INNER JOIN permission_ids
            ON permission_ids.[NAME] = target.[PermissionName]
        WHERE role.[END_DATE] IS NULL
           OR role.[END_DATE] >= CONVERT(DATE, GETUTCDATE())
    )
    INSERT INTO [dbo].[HMR_ROLE_PERMISSION]
    (
        [ROLE_ID],
        [PERMISSION_ID],
        [END_DATE],
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
        target.[ROLE_ID],
        target.[PERMISSION_ID],
        NULL,
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory,
        @MigrationUserId,
        @MigrationTimestamp,
        @MigrationUserGuid,
        @MigrationUserDirectory
    FROM resolved_targets target
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[HMR_ROLE_PERMISSION] existing
        WHERE existing.[ROLE_ID] = target.[ROLE_ID]
          AND existing.[PERMISSION_ID] = target.[PERMISSION_ID]
          AND
          (
              existing.[END_DATE] IS NULL
              OR existing.[END_DATE] >= CONVERT(DATE, GETUTCDATE())
          )
    );

    /* ------------------------------------------------------------------ */
    /* Descriptions                                                        */
    /* ------------------------------------------------------------------ */

    DECLARE @DescriptionTables TABLE ([TableName] SYSNAME NOT NULL);
    INSERT INTO @DescriptionTables ([TableName])
    VALUES
        (N'HMR_SUBMISSION_CONFIGURATION'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW'),
        (N'HMR_SUBMISSION_CONFIG_RULE'),
        (N'HMR_SUBMISSION_CONFIG_RULE_ACTIVITY'),
        (N'HMR_SUBMISSION_CONFIG_AUDIENCE'),
        (N'HMR_SUBMISSION_CONFIG_SERVICE_AREA');

    DECLARE @Descriptions TABLE
    (
        [TableName] SYSNAME NOT NULL,
        [ColumnName] SYSNAME NULL,
        [Description] NVARCHAR(4000) NOT NULL
    );

    INSERT INTO @Descriptions ([TableName], [ColumnName], [Description])
    VALUES
        (N'HMR_SUBMISSION_CONFIGURATION', NULL, N'Defines configurable submission restrictions and their scope, effective dates, and activation state.'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', NULL, N'Defines annually recurring reminder and submission blackout date windows.'),
        (N'HMR_SUBMISSION_CONFIG_RULE', NULL, N'Defines typed threshold rules evaluated against submitted report rows.'),
        (N'HMR_SUBMISSION_CONFIG_RULE_ACTIVITY', NULL, N'Associates submission configuration rules with activity codes.'),
        (N'HMR_SUBMISSION_CONFIG_AUDIENCE', NULL, N'Defines user types or roles to which a submission configuration applies.'),
        (N'HMR_SUBMISSION_CONFIG_SERVICE_AREA', NULL, N'Associates a service-area-scoped submission configuration with service areas.'),

        (N'HMR_SUBMISSION_CONFIGURATION', N'SUBMISSION_CONFIGURATION_ID', N'Unique identifier for a submission configuration.'),
        (N'HMR_SUBMISSION_CONFIGURATION', N'CONFIGURATION_KEY', N'Stable application key for a submission configuration.'),
        (N'HMR_SUBMISSION_CONFIGURATION', N'CONFIGURATION_NAME', N'Business display name for a submission configuration.'),
        (N'HMR_SUBMISSION_CONFIGURATION', N'SUBMISSION_STREAM_ID', N'Submission stream governed by the configuration.'),
        (N'HMR_SUBMISSION_CONFIGURATION', N'IS_ACTIVE', N'Indicates whether validation and generated notices are enabled.'),
        (N'HMR_SUBMISSION_CONFIGURATION', N'EFFECTIVE_FROM_DATE', N'First Pacific calendar date on which the recurring configuration may apply.'),
        (N'HMR_SUBMISSION_CONFIGURATION', N'EFFECTIVE_TO_DATE', N'Optional last Pacific calendar date on which the recurring configuration may apply.'),
        (N'HMR_SUBMISSION_CONFIGURATION', N'TIME_ZONE_ID', N'IANA time zone used to evaluate dates and recurring windows.'),
        (N'HMR_SUBMISSION_CONFIGURATION', N'SCOPE_TYPE', N'Configuration scope: GLOBAL or SERVICE_AREA.'),

        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'SUBMISSION_CONFIG_WINDOW_ID', N'Unique identifier for a recurring configuration window.'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'SUBMISSION_CONFIGURATION_ID', N'Submission configuration governed by this window.'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'WINDOW_TYPE', N'Window behavior: REMINDER or BLACKOUT.'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'START_MONTH', N'Calendar month in which the recurring window starts.'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'START_DAY', N'Day of month on which the recurring window starts.'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'END_MONTH', N'Calendar month in which the recurring window ends.'),
        (N'HMR_SUBMISSION_CONFIG_WINDOW', N'END_DAY', N'Day of month on which the recurring window ends.'),

        (N'HMR_SUBMISSION_CONFIG_RULE', N'SUBMISSION_CONFIG_RULE_ID', N'Unique identifier for a submission restriction rule.'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'SUBMISSION_CONFIGURATION_ID', N'Submission configuration containing this rule.'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'RULE_TYPE', N'Typed application evaluator used for this rule.'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'DISPLAY_LABEL', N'Business label used when displaying the grouped rule.'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'COMPARISON_OPERATOR', N'Threshold comparison operator: GT or GTE.'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'THRESHOLD_VALUE', N'Accomplishment threshold evaluated for each individual report row.'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'UNIT_OF_MEASURE', N'Canonical unit of measure for the configured threshold.'),
        (N'HMR_SUBMISSION_CONFIG_RULE', N'DISPLAY_ORDER', N'Deterministic order in which rules are evaluated and displayed.'),

        (N'HMR_SUBMISSION_CONFIG_RULE_ACTIVITY', N'SUBMISSION_CONFIG_RULE_ACTIVITY_ID', N'Unique identifier for a rule and activity association.'),
        (N'HMR_SUBMISSION_CONFIG_RULE_ACTIVITY', N'SUBMISSION_CONFIG_RULE_ID', N'Submission restriction rule associated with the activity.'),
        (N'HMR_SUBMISSION_CONFIG_RULE_ACTIVITY', N'ACTIVITY_CODE_ID', N'Activity code to which the rule applies.'),

        (N'HMR_SUBMISSION_CONFIG_AUDIENCE', N'SUBMISSION_CONFIG_AUDIENCE_ID', N'Unique identifier for a configuration audience.'),
        (N'HMR_SUBMISSION_CONFIG_AUDIENCE', N'SUBMISSION_CONFIGURATION_ID', N'Submission configuration associated with the audience.'),
        (N'HMR_SUBMISSION_CONFIG_AUDIENCE', N'AUDIENCE_TYPE', N'Audience discriminator: USER_TYPE or ROLE.'),
        (N'HMR_SUBMISSION_CONFIG_AUDIENCE', N'AUDIENCE_VALUE', N'User type or role value to which the configuration applies.'),

        (N'HMR_SUBMISSION_CONFIG_SERVICE_AREA', N'SUBMISSION_CONFIG_SERVICE_AREA_ID', N'Unique identifier for a configuration and service area association.'),
        (N'HMR_SUBMISSION_CONFIG_SERVICE_AREA', N'SUBMISSION_CONFIGURATION_ID', N'Service-area-scoped submission configuration.'),
        (N'HMR_SUBMISSION_CONFIG_SERVICE_AREA', N'SERVICE_AREA_NUMBER', N'Service area to which the configuration applies.');

    DECLARE @CommonDescriptions TABLE
    (
        [ColumnName] SYSNAME NOT NULL,
        [Description] NVARCHAR(4000) NOT NULL
    );

    INSERT INTO @CommonDescriptions ([ColumnName], [Description])
    VALUES
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

    INSERT INTO @Descriptions ([TableName], [ColumnName], [Description])
    SELECT tables.[TableName], common.[ColumnName], common.[Description]
    FROM @DescriptionTables tables
    CROSS JOIN @CommonDescriptions common;

    DECLARE @Description NVARCHAR(4000);
    DECLARE description_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT [TableName], [ColumnName], [Description]
        FROM @Descriptions;

    OPEN description_cursor;
    FETCH NEXT FROM description_cursor INTO @TableName, @ColumnName, @Description;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @ColumnName IS NULL
        BEGIN
            IF NOT EXISTS
            (
                SELECT 1 FROM sys.extended_properties
                WHERE [major_id] = OBJECT_ID(N'dbo.' + @TableName)
                  AND [minor_id] = 0
                  AND [name] = N'MS_Description'
            )
            BEGIN
                EXEC sys.sp_addextendedproperty
                    @name = N'MS_Description', @value = @Description,
                    @level0type = N'SCHEMA', @level0name = N'dbo',
                    @level1type = N'TABLE', @level1name = @TableName;
            END;
        END
        ELSE IF NOT EXISTS
        (
            SELECT 1
            FROM sys.extended_properties ep
            INNER JOIN sys.columns c
                ON c.[object_id] = ep.[major_id]
               AND c.[column_id] = ep.[minor_id]
            WHERE ep.[major_id] = OBJECT_ID(N'dbo.' + @TableName)
              AND c.[name] = @ColumnName
              AND ep.[name] = N'MS_Description'
        )
        BEGIN
            EXEC sys.sp_addextendedproperty
                @name = N'MS_Description', @value = @Description,
                @level0type = N'SCHEMA', @level0name = N'dbo',
                @level1type = N'TABLE', @level1name = @TableName,
                @level2type = N'COLUMN', @level2name = @ColumnName;
        END;

        FETCH NEXT FROM description_cursor INTO @TableName, @ColumnName, @Description;
    END;
    CLOSE description_cursor;
    DEALLOCATE description_cursor;

    /* ------------------------------------------------------------------ */
    /* Database role grants                                                */
    /* ------------------------------------------------------------------ */

    IF DATABASE_PRINCIPAL_ID(N'HMR_APPLICATION_PROXY') IS NOT NULL
    BEGIN
        DECLARE application_grant_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT [TableName] FROM @DescriptionTables;

        OPEN application_grant_cursor;
        FETCH NEXT FROM application_grant_cursor INTO @TableName;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @Sql = N'GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[dbo].'
                + QUOTENAME(@TableName) + N' TO [HMR_APPLICATION_PROXY];';
            EXEC sys.sp_executesql @Sql;
            FETCH NEXT FROM application_grant_cursor INTO @TableName;
        END;
        CLOSE application_grant_cursor;
        DEALLOCATE application_grant_cursor;

        DECLARE @SequenceName SYSNAME;
        DECLARE @Sequences TABLE ([SequenceName] SYSNAME NOT NULL);
        INSERT INTO @Sequences ([SequenceName])
        VALUES
            (N'HMR_SUBM_CFG_ID_SEQ'),
            (N'HMR_SUBM_CFG_WIN_ID_SEQ'),
            (N'HMR_SUBM_CFG_RULE_ID_SEQ'),
            (N'HMR_SUBM_CFG_RACT_ID_SEQ'),
            (N'HMR_SUBM_CFG_AUD_ID_SEQ'),
            (N'HMR_SUBM_CFG_SA_ID_SEQ');

        DECLARE sequence_grant_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT [SequenceName] FROM @Sequences;

        OPEN sequence_grant_cursor;
        FETCH NEXT FROM sequence_grant_cursor INTO @SequenceName;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @Sql = N'GRANT UPDATE ON OBJECT::[dbo].' + QUOTENAME(@SequenceName)
                + N' TO [HMR_APPLICATION_PROXY];';
            EXEC sys.sp_executesql @Sql;
            FETCH NEXT FROM sequence_grant_cursor INTO @SequenceName;
        END;
        CLOSE sequence_grant_cursor;
        DEALLOCATE sequence_grant_cursor;
    END;

    IF DATABASE_PRINCIPAL_ID(N'HMR_READ_ONLY') IS NOT NULL
    BEGIN
        DECLARE readonly_grant_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT [TableName] FROM @DescriptionTables;

        OPEN readonly_grant_cursor;
        FETCH NEXT FROM readonly_grant_cursor INTO @TableName;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @Sql = N'GRANT SELECT ON OBJECT::[dbo].' + QUOTENAME(@TableName)
                + N' TO [HMR_READ_ONLY];';
            EXEC sys.sp_executesql @Sql;
            FETCH NEXT FROM readonly_grant_cursor INTO @TableName;
        END;
        CLOSE readonly_grant_cursor;
        DEALLOCATE readonly_grant_cursor;
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
