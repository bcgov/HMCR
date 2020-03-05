// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerConfigurationPostgreSQLExtensions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license
// </copyright>
// <summary>
//   This class contains the PostgreSQL logger configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Serilog.Configuration;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Sinks.PostgreSQL;

    using Microsoft.Extensions.Configuration;
    using NpgsqlTypes;

    /// <summary>
    ///     This class contains the PostgreSQL logger configuration.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    // ReSharper disable once UnusedMember.Global
    public static class LoggerConfigurationPostgreSqlExtensions
    {
        /// <summary>
        ///     Default time to wait between checking for event batches.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(5);

        /// <summary>
        ///     Adds a sink which writes to PostgreSQL table.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="connectionString">The connection string to the database where to store the events.</param>
        /// <param name="tableName">Name of the table to store the events in.</param>
        /// <param name="columnOptions">Table columns writers</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="batchSizeLimit">The maximum number of events to include to single batch.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
        /// <param name="useCopy">If true inserts data via COPY command, otherwise uses INSERT INTO statement </param>
        /// <param name="schemaName">Schema name</param>
        /// <param name="needAutoCreateTable">Set if sink should create table</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        [SuppressMessage(
            "StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        // ReSharper disable once UnusedMember.Global
        public static LoggerConfiguration PostgreSql(
            this LoggerSinkConfiguration sinkConfiguration,
            string connectionString,
            string tableName,
            IDictionary<string, ColumnWriterBase> columnOptions = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null,
            int batchSizeLimit = PostgreSqlSink.DefaultBatchSizeLimit,
            LoggingLevelSwitch levelSwitch = null,
            bool useCopy = true,
            string schemaName = "",
            bool needAutoCreateTable = false,
            IConfiguration appConfiguration = null)
        {
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            period = period ?? DefaultPeriod;

            var connectionStr = ApplyMicrosoftExtensionsConfiguration.GetConnectionString(connectionString, appConfiguration);

            IDictionary<string, ColumnWriterBase> columnOpts = new Dictionary<string, ColumnWriterBase>
            {
                { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
                { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
                { "props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) }
            };

            return sinkConfiguration.Sink(
                new PostgreSqlSink(
                    connectionStr,
                    tableName,
                    period.Value,
                    formatProvider,
                    columnOptions == null ? columnOpts : columnOptions,
                    batchSizeLimit,
                    useCopy,
                    schemaName,
                    needAutoCreateTable),
                restrictedToMinimumLevel,
                levelSwitch);
        }
    }
}