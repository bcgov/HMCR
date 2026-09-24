using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hmcr.Data.Database.Entities
{
    public partial class AppDbContext
    {
        public virtual DbSet<HmrSubmissionConfiguration> HmrSubmissionConfigurations { get; set; }
        public virtual DbSet<HmrSubmissionConfigWindow> HmrSubmissionConfigWindows { get; set; }
        public virtual DbSet<HmrSubmissionConfigRule> HmrSubmissionConfigRules { get; set; }
        public virtual DbSet<HmrSubmissionConfigRuleActivity> HmrSubmissionConfigRuleActivities { get; set; }
        public virtual DbSet<HmrSubmissionConfigAudience> HmrSubmissionConfigAudiences { get; set; }
        public virtual DbSet<HmrSubmissionConfigServiceArea> HmrSubmissionConfigServiceAreas { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            ConfigureSubmissionConfiguration(modelBuilder);
            ConfigureSubmissionConfigWindow(modelBuilder);
            ConfigureSubmissionConfigRule(modelBuilder);
            ConfigureSubmissionConfigRuleActivity(modelBuilder);
            ConfigureSubmissionConfigAudience(modelBuilder);
            ConfigureSubmissionConfigServiceArea(modelBuilder);
        }

        private static void ConfigureSubmissionConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HmrSubmissionConfiguration>(entity =>
            {
                entity.HasKey(e => e.SubmissionConfigurationId)
                    .HasName("HMR_SUBM_CFG_PK");
                entity.ToTable("HMR_SUBMISSION_CONFIGURATION");
                entity.HasIndex(e => e.ConfigurationKey)
                    .HasDatabaseName("HMR_SUBM_CFG_KEY_UK")
                    .IsUnique();

                entity.Property(e => e.SubmissionConfigurationId)
                    .HasColumnName("SUBMISSION_CONFIGURATION_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBM_CFG_ID_SEQ])");
                entity.Property(e => e.ConfigurationKey)
                    .IsRequired()
                    .HasColumnName("CONFIGURATION_KEY")
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.ConfigurationName)
                    .IsRequired()
                    .HasColumnName("CONFIGURATION_NAME")
                    .HasMaxLength(150)
                    .IsUnicode(false);
                entity.Property(e => e.SubmissionStreamId)
                    .HasColumnName("SUBMISSION_STREAM_ID")
                    .HasColumnType("numeric(9, 0)");
                entity.Property(e => e.IsActive).HasColumnName("IS_ACTIVE");
                entity.Property(e => e.EffectiveFromDate)
                    .HasColumnName("EFFECTIVE_FROM_DATE")
                    .HasColumnType("date");
                entity.Property(e => e.EffectiveToDate)
                    .HasColumnName("EFFECTIVE_TO_DATE")
                    .HasColumnType("date");
                entity.Property(e => e.TimeZoneId)
                    .IsRequired()
                    .HasColumnName("TIME_ZONE_ID")
                    .HasMaxLength(64)
                    .IsUnicode(false);
                entity.Property(e => e.ScopeType)
                    .IsRequired()
                    .HasColumnName("SCOPE_TYPE")
                    .HasMaxLength(20)
                    .IsUnicode(false);
                ConfigureAudit(entity);

                entity.HasOne(e => e.SubmissionStream)
                    .WithMany(e => e.HmrSubmissionConfigurations)
                    .HasForeignKey(e => e.SubmissionStreamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SUBM_CFG_STREAM_FK");
            });
        }

        private static void ConfigureSubmissionConfigWindow(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HmrSubmissionConfigWindow>(entity =>
            {
                entity.HasKey(e => e.SubmissionConfigWindowId)
                    .HasName("HMR_SUBM_CFG_WIN_PK");
                entity.ToTable("HMR_SUBMISSION_CONFIG_WINDOW");
                entity.HasIndex(e => new { e.SubmissionConfigurationId, e.WindowType })
                    .HasDatabaseName("HMR_SUBM_CFG_WIN_UK")
                    .IsUnique();

                entity.Property(e => e.SubmissionConfigWindowId)
                    .HasColumnName("SUBMISSION_CONFIG_WINDOW_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBM_CFG_WIN_ID_SEQ])");
                entity.Property(e => e.SubmissionConfigurationId)
                    .HasColumnName("SUBMISSION_CONFIGURATION_ID")
                    .HasColumnType("numeric(9, 0)");
                entity.Property(e => e.WindowType)
                    .IsRequired()
                    .HasColumnName("WINDOW_TYPE")
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.StartMonth).HasColumnName("START_MONTH");
                entity.Property(e => e.StartDay).HasColumnName("START_DAY");
                entity.Property(e => e.EndMonth).HasColumnName("END_MONTH");
                entity.Property(e => e.EndDay).HasColumnName("END_DAY");
                ConfigureAudit(entity);

                entity.HasOne(e => e.SubmissionConfiguration)
                    .WithMany(e => e.Windows)
                    .HasForeignKey(e => e.SubmissionConfigurationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SUBM_CFG_WIN_CFG_FK");
            });
        }

        private static void ConfigureSubmissionConfigRule(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HmrSubmissionConfigRule>(entity =>
            {
                entity.HasKey(e => e.SubmissionConfigRuleId)
                    .HasName("HMR_SUBM_CFG_RULE_PK");
                entity.ToTable("HMR_SUBMISSION_CONFIG_RULE");
                entity.HasIndex(e => new { e.SubmissionConfigurationId, e.DisplayOrder })
                    .HasDatabaseName("HMR_SUBM_CFG_RULE_ORDER_UK")
                    .IsUnique();

                entity.Property(e => e.SubmissionConfigRuleId)
                    .HasColumnName("SUBMISSION_CONFIG_RULE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBM_CFG_RULE_ID_SEQ])");
                entity.Property(e => e.SubmissionConfigurationId)
                    .HasColumnName("SUBMISSION_CONFIGURATION_ID")
                    .HasColumnType("numeric(9, 0)");
                entity.Property(e => e.RuleType)
                    .IsRequired()
                    .HasColumnName("RULE_TYPE")
                    .HasMaxLength(30)
                    .IsUnicode(false);
                entity.Property(e => e.DisplayLabel)
                    .IsRequired()
                    .HasColumnName("DISPLAY_LABEL")
                    .HasMaxLength(150)
                    .IsUnicode(false);
                entity.Property(e => e.ComparisonOperator)
                    .IsRequired()
                    .HasColumnName("COMPARISON_OPERATOR")
                    .HasMaxLength(3)
                    .IsUnicode(false);
                entity.Property(e => e.ThresholdValue)
                    .HasColumnName("THRESHOLD_VALUE")
                    .HasColumnType("decimal(18, 4)");
                entity.Property(e => e.UnitOfMeasure)
                    .IsRequired()
                    .HasColumnName("UNIT_OF_MEASURE")
                    .HasMaxLength(30)
                    .IsUnicode(false);
                entity.Property(e => e.DisplayOrder).HasColumnName("DISPLAY_ORDER");
                ConfigureAudit(entity);

                entity.HasOne(e => e.SubmissionConfiguration)
                    .WithMany(e => e.Rules)
                    .HasForeignKey(e => e.SubmissionConfigurationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SUBM_CFG_RULE_CFG_FK");
            });
        }

        private static void ConfigureSubmissionConfigRuleActivity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HmrSubmissionConfigRuleActivity>(entity =>
            {
                entity.HasKey(e => e.SubmissionConfigRuleActivityId)
                    .HasName("HMR_SUBM_CFG_RACT_PK");
                entity.ToTable("HMR_SUBMISSION_CONFIG_RULE_ACTIVITY");
                entity.HasIndex(e => new { e.SubmissionConfigRuleId, e.ActivityCodeId })
                    .HasDatabaseName("HMR_SUBM_CFG_RACT_UK")
                    .IsUnique();

                entity.Property(e => e.SubmissionConfigRuleActivityId)
                    .HasColumnName("SUBMISSION_CONFIG_RULE_ACTIVITY_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBM_CFG_RACT_ID_SEQ])");
                entity.Property(e => e.SubmissionConfigRuleId)
                    .HasColumnName("SUBMISSION_CONFIG_RULE_ID")
                    .HasColumnType("numeric(9, 0)");
                entity.Property(e => e.ActivityCodeId)
                    .HasColumnName("ACTIVITY_CODE_ID")
                    .HasColumnType("numeric(9, 0)");
                ConfigureAudit(entity);

                entity.HasOne(e => e.SubmissionConfigRule)
                    .WithMany(e => e.Activities)
                    .HasForeignKey(e => e.SubmissionConfigRuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SUBM_CFG_RACT_RULE_FK");
                entity.HasOne(e => e.ActivityCode)
                    .WithMany(e => e.HmrSubmissionConfigRuleActivities)
                    .HasForeignKey(e => e.ActivityCodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SUBM_CFG_RACT_ACT_FK");
            });
        }

        private static void ConfigureSubmissionConfigAudience(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HmrSubmissionConfigAudience>(entity =>
            {
                entity.HasKey(e => e.SubmissionConfigAudienceId)
                    .HasName("HMR_SUBM_CFG_AUD_PK");
                entity.ToTable("HMR_SUBMISSION_CONFIG_AUDIENCE");
                entity.HasIndex(e => new { e.SubmissionConfigurationId, e.AudienceType, e.AudienceValue })
                    .HasDatabaseName("HMR_SUBM_CFG_AUD_UK")
                    .IsUnique();

                entity.Property(e => e.SubmissionConfigAudienceId)
                    .HasColumnName("SUBMISSION_CONFIG_AUDIENCE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBM_CFG_AUD_ID_SEQ])");
                entity.Property(e => e.SubmissionConfigurationId)
                    .HasColumnName("SUBMISSION_CONFIGURATION_ID")
                    .HasColumnType("numeric(9, 0)");
                entity.Property(e => e.AudienceType)
                    .IsRequired()
                    .HasColumnName("AUDIENCE_TYPE")
                    .HasMaxLength(30)
                    .IsUnicode(false);
                entity.Property(e => e.AudienceValue)
                    .IsRequired()
                    .HasColumnName("AUDIENCE_VALUE")
                    .HasMaxLength(50)
                    .IsUnicode(false);
                ConfigureAudit(entity);

                entity.HasOne(e => e.SubmissionConfiguration)
                    .WithMany(e => e.Audiences)
                    .HasForeignKey(e => e.SubmissionConfigurationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SUBM_CFG_AUD_CFG_FK");
            });
        }

        private static void ConfigureSubmissionConfigServiceArea(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HmrSubmissionConfigServiceArea>(entity =>
            {
                entity.HasKey(e => e.SubmissionConfigServiceAreaId)
                    .HasName("HMR_SUBM_CFG_SA_PK");
                entity.ToTable("HMR_SUBMISSION_CONFIG_SERVICE_AREA");
                entity.HasIndex(e => new { e.SubmissionConfigurationId, e.ServiceAreaNumber })
                    .HasDatabaseName("HMR_SUBM_CFG_SA_UK")
                    .IsUnique();

                entity.Property(e => e.SubmissionConfigServiceAreaId)
                    .HasColumnName("SUBMISSION_CONFIG_SERVICE_AREA_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBM_CFG_SA_ID_SEQ])");
                entity.Property(e => e.SubmissionConfigurationId)
                    .HasColumnName("SUBMISSION_CONFIGURATION_ID")
                    .HasColumnType("numeric(9, 0)");
                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(9, 0)");
                ConfigureAudit(entity);

                entity.HasOne(e => e.SubmissionConfiguration)
                    .WithMany(e => e.ServiceAreas)
                    .HasForeignKey(e => e.SubmissionConfigurationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SUBM_CFG_SA_CFG_FK");
                entity.HasOne(e => e.ServiceArea)
                    .WithMany(e => e.HmrSubmissionConfigServiceAreas)
                    .HasForeignKey(e => e.ServiceAreaNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SUBM_CFG_SA_AREA_FK");
            });
        }

        private static void ConfigureAudit<TEntity>(EntityTypeBuilder<TEntity> entity)
            where TEntity : class
        {
            entity.Property<long>("ConcurrencyControlNumber")
                .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                .HasDefaultValueSql("((1))")
                .IsConcurrencyToken();
            entity.Property<string>("AppCreateUserid")
                .IsRequired().HasColumnName("APP_CREATE_USERID").HasMaxLength(30).IsUnicode(false);
            entity.Property<System.DateTime>("AppCreateTimestamp")
                .HasColumnName("APP_CREATE_TIMESTAMP").HasColumnType("datetime");
            entity.Property<System.Guid>("AppCreateUserGuid").HasColumnName("APP_CREATE_USER_GUID");
            entity.Property<string>("AppCreateUserDirectory")
                .IsRequired().HasColumnName("APP_CREATE_USER_DIRECTORY").HasMaxLength(12).IsUnicode(false);
            entity.Property<string>("AppLastUpdateUserid")
                .IsRequired().HasColumnName("APP_LAST_UPDATE_USERID").HasMaxLength(30).IsUnicode(false);
            entity.Property<System.DateTime>("AppLastUpdateTimestamp")
                .HasColumnName("APP_LAST_UPDATE_TIMESTAMP").HasColumnType("datetime");
            entity.Property<System.Guid>("AppLastUpdateUserGuid").HasColumnName("APP_LAST_UPDATE_USER_GUID");
            entity.Property<string>("AppLastUpdateUserDirectory")
                .IsRequired().HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY").HasMaxLength(12).IsUnicode(false);
            entity.Property<string>("DbAuditCreateUserid")
                .IsRequired().HasColumnName("DB_AUDIT_CREATE_USERID").HasMaxLength(30).IsUnicode(false)
                .HasDefaultValueSql("(user_name())");
            entity.Property<System.DateTime>("DbAuditCreateTimestamp")
                .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP").HasColumnType("datetime")
                .HasDefaultValueSql("(getutcdate())");
            entity.Property<string>("DbAuditLastUpdateUserid")
                .IsRequired().HasColumnName("DB_AUDIT_LAST_UPDATE_USERID").HasMaxLength(30).IsUnicode(false)
                .HasDefaultValueSql("(user_name())");
            entity.Property<System.DateTime>("DbAuditLastUpdateTimestamp")
                .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP").HasColumnType("datetime")
                .HasDefaultValueSql("(getutcdate())");
        }
    }
}
