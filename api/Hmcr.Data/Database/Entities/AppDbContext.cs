using System;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Hmcr.Data.Database.Entities
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<HmrActivityCode> HmrActivityCodes { get; set; }
        public virtual DbSet<HmrActivityCodeHist> HmrActivityCodeHists { get; set; }
        public virtual DbSet<HmrActivityCodeRule> HmrActivityCodeRules { get; set; }
        public virtual DbSet<HmrCodeLookup> HmrCodeLookups { get; set; }
        public virtual DbSet<HmrCodeLookupHist> HmrCodeLookupHists { get; set; }
        public virtual DbSet<HmrContractTerm> HmrContractTerms { get; set; }
        public virtual DbSet<HmrContractTermHist> HmrContractTermHists { get; set; }
        public virtual DbSet<HmrDistrict> HmrDistricts { get; set; }
        public virtual DbSet<HmrFeedbackMessage> HmrFeedbackMessages { get; set; }
        public virtual DbSet<HmrLocationCode> HmrLocationCodes { get; set; }
        public virtual DbSet<HmrLocationCodeHist> HmrLocationCodeHists { get; set; }
        public virtual DbSet<HmrMimeType> HmrMimeTypes { get; set; }
        public virtual DbSet<HmrParty> HmrParties { get; set; }
        public virtual DbSet<HmrPartyHist> HmrPartyHists { get; set; }
        public virtual DbSet<HmrPermission> HmrPermissions { get; set; }
        public virtual DbSet<HmrPermissionHist> HmrPermissionHists { get; set; }
        public virtual DbSet<HmrRegion> HmrRegions { get; set; }
        public virtual DbSet<HmrRockfallReport> HmrRockfallReports { get; set; }
        public virtual DbSet<HmrRockfallReportHist> HmrRockfallReportHists { get; set; }
        public virtual DbSet<HmrRockfallReportVw> HmrRockfallReportVws { get; set; }
        public virtual DbSet<HmrRole> HmrRoles { get; set; }
        public virtual DbSet<HmrRoleHist> HmrRoleHists { get; set; }
        public virtual DbSet<HmrRolePermission> HmrRolePermissions { get; set; }
        public virtual DbSet<HmrRolePermissionHist> HmrRolePermissionHists { get; set; }
        public virtual DbSet<HmrServiceArea> HmrServiceAreas { get; set; }
        public virtual DbSet<HmrServiceAreaActivity> HmrServiceAreaActivities { get; set; }
        public virtual DbSet<HmrServiceAreaActivityHist> HmrServiceAreaActivityHists { get; set; }
        public virtual DbSet<HmrServiceAreaHist> HmrServiceAreaHists { get; set; }
        public virtual DbSet<HmrServiceAreaUser> HmrServiceAreaUsers { get; set; }
        public virtual DbSet<HmrServiceAreaUserHist> HmrServiceAreaUserHists { get; set; }
        public virtual DbSet<HmrStreamElement> HmrStreamElements { get; set; }
        public virtual DbSet<HmrStreamElementHist> HmrStreamElementHists { get; set; }
        public virtual DbSet<HmrSubmissionObject> HmrSubmissionObjects { get; set; }
        public virtual DbSet<HmrSubmissionRow> HmrSubmissionRows { get; set; }
        public virtual DbSet<HmrSubmissionRowHist> HmrSubmissionRowHists { get; set; }
        public virtual DbSet<HmrSubmissionStatu> HmrSubmissionStatus { get; set; }
        public virtual DbSet<HmrSubmissionStream> HmrSubmissionStreams { get; set; }
        public virtual DbSet<HmrSubmissionStreamHist> HmrSubmissionStreamHists { get; set; }
        public virtual DbSet<HmrSystemUser> HmrSystemUsers { get; set; }
        public virtual DbSet<HmrSystemUserHist> HmrSystemUserHists { get; set; }
        public virtual DbSet<HmrSystemValidation> HmrSystemValidations { get; set; }
        public virtual DbSet<HmrUserRole> HmrUserRoles { get; set; }
        public virtual DbSet<HmrUserRoleHist> HmrUserRoleHists { get; set; }
        public virtual DbSet<HmrWildlifeReport> HmrWildlifeReports { get; set; }
        public virtual DbSet<HmrWildlifeReportHist> HmrWildlifeReportHists { get; set; }
        public virtual DbSet<HmrWildlifeReportVw> HmrWildlifeReportVws { get; set; }
        public virtual DbSet<HmrWorkReport> HmrWorkReports { get; set; }
        public virtual DbSet<HmrWorkReportHist> HmrWorkReportHists { get; set; }
        public virtual DbSet<HmrWorkReportVw> HmrWorkReportVws { get; set; }
        public virtual DbSet<HmrSaltReport> HmrSaltReports { get; set; }
        public virtual DbSet<HmrSaltStockpile> HmrSaltStockpiles { get; set; }
        public virtual DbSet<HmrSaltReportAppendix> HmrSaltReportAppendixes { get; set; }
        public virtual DbSet<HmrSaltVulnArea> HmrSaltVulnAreas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HmrActivityCode>(entity =>
            {
                entity.HasKey(e => e.ActivityCodeId)
                    .HasName("HMR_ACT_CODE_PK");

                entity.ToTable("HMR_ACTIVITY_CODE");

                entity.HasComment("A tracking number for maintenance activities undertaken by the Contractor. This number is required for the specific reporting of each activity. The numbers are provided by the Province.  Reporting criteria varies based on location requirements, record frequency and reporting frequency.  Local Area Specification activities vary by Service Area, and therefore many of these activities do not apply to each Service Area.");

                entity.HasIndex(e => e.ActivityNumber)
                    .HasName("HMR_ACTIVITY_CODE_UC")
                    .IsUnique();

                entity.HasIndex(e => e.LocationCodeId)
                    .HasName("HMR_ACT_CODE_FK_I");

                entity.Property(e => e.ActivityCodeId)
                    .HasColumnName("ACTIVITY_CODE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_ACT_CODE_ID_SEQ])")
                    .HasComment("Unique identifier for a record.");

                entity.Property(e => e.ActivityApplication)
                    .HasColumnName("ACTIVITY_APPLICATION")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Indicates if activity is conducted in all service areas or is specified for some service areas. ");

                entity.Property(e => e.ActivityName)
                    .IsRequired()
                    .HasColumnName("ACTIVITY_NAME")
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasComment("N");

                entity.Property(e => e.ActivityNumber)
                    .IsRequired()
                    .HasColumnName("ACTIVITY_NUMBER")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasComment("Code which uniquely identifies the activity performed.  The number reflects a a classificaiton hierarchy comprised of three levels: ABBCCC  A - the first digit represents Specification Category (eg:2 - Drainage ) BB - the second two digits represent Activity Category (eg: 02 - Drainage Appliance Maintenance) CCC - the last three digits represent Activity Type and Detail (eg: 310 - Boring, Augering.  300 series reflects Quantified value, which would be linear meters in this case.)");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime")
                    .HasComment("The latest date submissions will be accepted.");

                entity.Property(e => e.FeatureType)
                    .HasColumnName("FEATURE_TYPE")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Indicator of spatial nature of the activity.  (ie:  point, line or either)   Point - a point location will be reported  Line - activity occurs in relation to a section of road  Either - may be spatially represented in either manner");

                entity.Property(e => e.IsSiteNumRequired)
                    .HasColumnName("IS_SITE_NUM_REQUIRED")
                    .HasComment("Indicates if a site number must be submitted for the activity");

                entity.Property(e => e.LocationCodeId)
                    .HasColumnName("LOCATION_CODE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for a record");

                entity.Property(e => e.MaintenanceType)
                    .IsRequired()
                    .HasColumnName("MAINTENANCE_TYPE")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment(" Classification of maintenance activities which specifies detail of submission or reporting requirements (ie: Routine, Quantified, Additional).   Routine - reoccuring maintenace activities that require less detailed reporting  Quantified - maintenance activities that require more detailed reporting  Additional - activities that exceed agreement threasholds");

                entity.Property(e => e.MaxValue)
                    .HasColumnName("MAX_VALUE")
                    .HasColumnType("numeric(11, 2)");

                entity.Property(e => e.MinValue)
                    .HasColumnName("MIN_VALUE")
                    .HasColumnType("numeric(11, 2)");

                entity.Property(e => e.ReportingFrequency).HasColumnName("REPORTING_FREQUENCY");

                entity.Property(e => e.RoadClassRule)
                    .HasColumnName("ROAD_CLASS_RULE")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.RoadLengthRule)
                    .HasColumnName("ROAD_LENGTH_RULE")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.SpThresholdLevel)
                    .HasColumnName("SP_THRESHOLD_LEVEL")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Determines the tolerated spatial variance allowed when comparing submitted activity coordinates vs the related Highway Unique road segment. Each level is defined within the CODE_LOOKUP table under the THRSHLD_SP_VAR code");

                entity.Property(e => e.SurfaceTypeRule)
                    .HasColumnName("SURFACE_TYPE_RULE")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.UnitOfMeasure)
                    .IsRequired()
                    .HasColumnName("UNIT_OF_MEASURE")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("The code which represents the unit of measure for the specified activity. ");

                entity.HasOne(d => d.LocationCode)
                    .WithMany(p => p.HmrActivityCodes)
                    .HasForeignKey(d => d.LocationCodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_ACT_CODE_LOC_CODE_FK");

                entity.HasOne(d => d.RoadClassRuleNavigation)
                    .WithMany(p => p.HmrActivityCodeRoadClassRuleNavigations)
                    .HasForeignKey(d => d.RoadClassRule)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HMR_ACTIV__ROAD___1C3D2329");

                entity.HasOne(d => d.RoadLengthRuleNavigation)
                    .WithMany(p => p.HmrActivityCodeRoadLengthRuleNavigations)
                    .HasForeignKey(d => d.RoadLengthRule)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HMR_ACTIV__ROAD___1D314762");

                entity.HasOne(d => d.SurfaceTypeRuleNavigation)
                    .WithMany(p => p.HmrActivityCodeSurfaceTypeRuleNavigations)
                    .HasForeignKey(d => d.SurfaceTypeRule)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HMR_ACTIV__SURFA__1E256B9B");
            });

            modelBuilder.Entity<HmrActivityCodeHist>(entity =>
            {
                entity.HasKey(e => e.ActivityCodeHistId)
                    .HasName("HMR_ACT_C_H_PK");

                entity.ToTable("HMR_ACTIVITY_CODE_HIST");

                entity.HasIndex(e => new { e.ActivityCodeHistId, e.EndDateHist })
                    .HasName("HMR_ACT_C_H_UK")
                    .IsUnique();

                entity.Property(e => e.ActivityCodeHistId)
                    .HasColumnName("ACTIVITY_CODE_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_ACTIVITY_CODE_H_ID_SEQ])");

                entity.Property(e => e.ActivityApplication)
                    .HasColumnName("ACTIVITY_APPLICATION")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ActivityCodeId)
                    .HasColumnName("ACTIVITY_CODE_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ActivityName)
                    .IsRequired()
                    .HasColumnName("ACTIVITY_NAME")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ActivityNumber)
                    .IsRequired()
                    .HasColumnName("ACTIVITY_NUMBER")
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.FeatureType)
                    .HasColumnName("FEATURE_TYPE")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.IsSiteNumRequired).HasColumnName("IS_SITE_NUM_REQUIRED");

                entity.Property(e => e.LocationCodeId)
                    .HasColumnName("LOCATION_CODE_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.MaintenanceType)
                    .IsRequired()
                    .HasColumnName("MAINTENANCE_TYPE")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.MaxValue)
                    .HasColumnName("MAX_VALUE")
                    .HasColumnType("numeric(11, 2)");

                entity.Property(e => e.MinValue)
                    .HasColumnName("MIN_VALUE")
                    .HasColumnType("numeric(11, 2)");

                entity.Property(e => e.ReportingFrequency).HasColumnName("REPORTING_FREQUENCY");

                entity.Property(e => e.RoadClassRule)
                    .HasColumnName("ROAD_CLASS_RULE")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.RoadLengthRule)
                    .HasColumnName("ROAD_LENGTH_RULE")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.SpThresholdLevel)
                    .HasColumnName("SP_THRESHOLD_LEVEL")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SurfaceTypeRule)
                    .HasColumnName("SURFACE_TYPE_RULE")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.UnitOfMeasure)
                    .IsRequired()
                    .HasColumnName("UNIT_OF_MEASURE")
                    .HasMaxLength(12)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HmrActivityCodeRule>(entity =>
            {
                entity.HasKey(e => e.ActivityCodeRuleId)
                    .HasName("PK__HMR_ACTI__E4140F7D2F39902D");

                entity.ToTable("HMR_ACTIVITY_CODE_RULE");

                entity.Property(e => e.ActivityCodeRuleId)
                    .HasColumnName("ACTIVITY_CODE_RULE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_ACTIVITY_CODE_RULE_ID_SEQ])");

                entity.Property(e => e.ActivityRuleExecName)
                    .IsRequired()
                    .HasColumnName("ACTIVITY_RULE_EXEC_NAME")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ActivityRuleName)
                    .IsRequired()
                    .HasColumnName("ACTIVITY_RULE_NAME")
                    .HasMaxLength(150);

                entity.Property(e => e.ActivityRuleSet)
                    .IsRequired()
                    .HasColumnName("ACTIVITY_RULE_SET")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())");

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("DISPLAY_ORDER")
                    .HasColumnType("numeric(3, 0)");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<HmrCodeLookup>(entity =>
            {
                entity.HasKey(e => e.CodeLookupId)
                    .HasName("HMR_CODE_LKUP_PK");

                entity.ToTable("HMR_CODE_LOOKUP");

                entity.HasComment("A range of lookup values used to decipher codes used in submissions to business legible values for reporting purposes.  As many code lookups share this table, views are available to join for reporting purposes.");

                entity.HasIndex(e => new { e.CodeSet, e.CodeValueNum, e.CodeName })
                    .HasName("HMR_CODE_LKUP_VAL_NUM_UC")
                    .IsUnique();

                entity.HasIndex(e => new { e.CodeSet, e.CodeValueText, e.CodeName })
                    .HasName("HMR_CODE_LKUP_VAL_TXT_UC")
                    .IsUnique();

                entity.Property(e => e.CodeLookupId)
                    .HasColumnName("CODE_LOOKUP_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_CODE_LKUP_ID_SEQ])")
                    .HasComment("Unique identifier for a record.");

                entity.Property(e => e.CodeName)
                    .HasColumnName("CODE_NAME")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("Display name or business name for a submission value.  These values are for display in analytical reporting.");

                entity.Property(e => e.CodeSet)
                    .HasColumnName("CODE_SET")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("Unique identifier for a group of lookup codes.  A database view is available for each group for use in analytics.");

                entity.Property(e => e.CodeValueFormat)
                    .HasColumnName("CODE_VALUE_FORMAT")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Specifies if the code value is text or numeric.");

                entity.Property(e => e.CodeValueNum)
                    .HasColumnName("CODE_VALUE_NUM")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment(" Numeric enumeration values provided in submissions.   These values are used for validating submissions and for display of CODE NAMES in analytical reporting.  Values must be unique per CODE SET.");

                entity.Property(e => e.CodeValueText)
                    .HasColumnName("CODE_VALUE_TEXT")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("Look up code values provided in submissions.   These values are used for validating submissions and for display of CODE NAMES in analytical reporting.  Values must be unique per CODE SET.");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("DISPLAY_ORDER")
                    .HasColumnType("numeric(3, 0)")
                    .HasComment("When displaying list of values, value can be used to present list in desired order.");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime")
                    .HasComment("The latest date submissions will be accepted.");

                entity.Property(e => e.IsIntegerOnly).HasColumnName("IS_INTEGER_ONLY");
            });

            modelBuilder.Entity<HmrCodeLookupHist>(entity =>
            {
                entity.HasKey(e => e.CodeLookupHistId)
                    .HasName("HMR_CODE__H_PK");

                entity.ToTable("HMR_CODE_LOOKUP_HIST");

                entity.HasIndex(e => new { e.CodeLookupHistId, e.EndDateHist })
                    .HasName("HMR_CODE__H_UK")
                    .IsUnique();

                entity.Property(e => e.CodeLookupHistId)
                    .HasColumnName("CODE_LOOKUP_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_CODE_LOOKUP_H_ID_SEQ])");

                entity.Property(e => e.CodeLookupId)
                    .HasColumnName("CODE_LOOKUP_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.CodeName)
                    .HasColumnName("CODE_NAME")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CodeSet)
                    .HasColumnName("CODE_SET")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CodeValueFormat)
                    .HasColumnName("CODE_VALUE_FORMAT")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.CodeValueNum)
                    .HasColumnName("CODE_VALUE_NUM")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.CodeValueText)
                    .HasColumnName("CODE_VALUE_TEXT")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("DISPLAY_ORDER")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsIntegerOnly).HasColumnName("IS_INTEGER_ONLY");
            });

            modelBuilder.Entity<HmrContractTerm>(entity =>
            {
                entity.HasKey(e => e.ContractTermId)
                    .HasName("HMR_CNRT_TRM_PK")
                    .IsClustered(false);

                entity.ToTable("HMR_CONTRACT_TERM");

                entity.HasComment("Identifies a unique contract term for each party and the service areas those organizations are obligated to provide services for. This table enables the confirmation of unique task IDs assigned by vendors for the term of their contract.");

                entity.HasIndex(e => e.PartyId)
                    .HasName("HMR_CNT_TRM_PRTY_FK_I");

                entity.HasIndex(e => e.ServiceAreaNumber)
                    .HasName("HMR_CNT_TRM_SRV_A_FK_I");

                entity.HasIndex(e => new { e.PartyId, e.ServiceAreaNumber, e.StartDate })
                    .HasName("HMR_CNT_TRM_UQ_CH")
                    .IsUnique();

                entity.Property(e => e.ContractTermId)
                    .HasColumnName("CONTRACT_TERM_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_CNT_TRM_ID_SEQ])")
                    .HasComment("Unique identifier for the record");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.ContractName)
                    .IsRequired()
                    .HasColumnName("CONTRACT_NAME")
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasComment("Contract name describing the contract term.  ");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime")
                    .HasComment("Latest date a contract term was in effect.");

                entity.Property(e => e.PartyId)
                    .HasColumnName("PARTY_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier of related PARTY record");

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Assigned number of the Service Area");

                entity.Property(e => e.StartDate)
                    .HasColumnName("START_DATE")
                    .HasColumnType("datetime")
                    .HasComment("Earliest date a contract term was in effect.");

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.HmrContractTerms)
                    .HasForeignKey(d => d.PartyId)
                    .HasConstraintName("HMR_CNRT_TRM_PRTY_FK");

                entity.HasOne(d => d.ServiceAreaNumberNavigation)
                    .WithMany(p => p.HmrContractTerms)
                    .HasForeignKey(d => d.ServiceAreaNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_CNRT_TRM_SRV_ARA_FK");
            });

            modelBuilder.Entity<HmrContractTermHist>(entity =>
            {
                entity.HasKey(e => e.ContractTermHistId)
                    .HasName("HMR_CNRT__H_PK");

                entity.ToTable("HMR_CONTRACT_TERM_HIST");

                entity.HasIndex(e => new { e.ContractTermHistId, e.EndDateHist })
                    .HasName("HMR_CNRT__H_UK")
                    .IsUnique();

                entity.Property(e => e.ContractTermHistId)
                    .HasColumnName("CONTRACT_TERM_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_CONTRACT_TERM_H_ID_SEQ])");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.ContractName)
                    .IsRequired()
                    .HasColumnName("CONTRACT_NAME")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ContractTermId)
                    .HasColumnName("CONTRACT_TERM_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.PartyId)
                    .HasColumnName("PARTY_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.StartDate)
                    .HasColumnName("START_DATE")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<HmrDistrict>(entity =>
            {
                entity.HasKey(e => e.DistrictNumber)
                    .HasName("HMR_DISTRICT_PK");

                entity.ToTable("HMR_DISTRICT");

                entity.HasComment("Ministry Districts lookup values.");

                entity.HasIndex(e => new { e.DistrictNumber, e.DistrictName })
                    .HasName("HMR_DIST_NO_NAME_UK")
                    .IsUnique();

                entity.Property(e => e.DistrictNumber)
                    .HasColumnName("DISTRICT_NUMBER")
                    .HasColumnType("numeric(2, 0)")
                    .HasComment("Number assigned to represent the District");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.DistrictId)
                    .HasColumnName("DISTRICT_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_DIST_ID_SEQ])")
                    .HasComment("Unique identifier for district records");

                entity.Property(e => e.DistrictName)
                    .IsRequired()
                    .HasColumnName("DISTRICT_NAME")
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasComment("The name of the District");

                entity.Property(e => e.RegionNumber)
                    .HasColumnName("REGION_NUMBER")
                    .HasColumnType("numeric(2, 0)")
                    .HasComment("Parent REGION containing the DISTRICT");

                entity.HasOne(d => d.RegionNumberNavigation)
                    .WithMany(p => p.HmrDistricts)
                    .HasForeignKey(d => d.RegionNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_DISTRICT_REGION_FK");
            });

            modelBuilder.Entity<HmrFeedbackMessage>(entity =>
            {
                entity.HasKey(e => e.FeedbackMessageId)
                    .HasName("HMR_FDBK_MSG_PK");

                entity.ToTable("HMR_FEEDBACK_MESSAGE");

                entity.HasComment("A record of each validation feedback message that is issued during the submission process.");

                entity.Property(e => e.FeedbackMessageId)
                    .HasColumnName("FEEDBACK_MESSAGE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [FDBK_MSG_ID_SEQ])")
                    .HasComment("Unique identifier for a record.");

                entity.Property(e => e.CommunicationDate)
                    .HasColumnName("COMMUNICATION_DATE")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of the issued email.");

                entity.Property(e => e.CommunicationSubject)
                    .HasColumnName("COMMUNICATION_SUBJECT")
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasComment("Subject line of the issued email.");

                entity.Property(e => e.CommunicationText)
                    .HasColumnName("COMMUNICATION_TEXT")
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasComment("Body of the issued email.");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.IsError)
                    .HasColumnName("IS_ERROR")
                    .HasComment("Indicates if the message encountered an error during generation.  This indicator reflects a processing error when generating the email from the applicaiton server, but does not factor in any subsequent email bounces or rejections from a receiving mail server.");

                entity.Property(e => e.IsSent)
                    .HasColumnName("IS_SENT")
                    .HasComment("Indicates if the message was sent from the application.  This flag is used to queue messages to be sent.  If an error is encountered during email processing it will be indicated by the IS_ERROR flag.");

                entity.Property(e => e.SendErrorText)
                    .HasColumnName("SEND_ERROR_TEXT")
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasComment("Error message received from application email invocation, if encountered.  Used to troubleshoot emailing issues.  Does not factor in any subsequent email bounces or rejections from a receiving mail server.");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for a SUBMISSION OBJECT");

                entity.HasOne(d => d.SubmissionObject)
                    .WithMany(p => p.HmrFeedbackMessages)
                    .HasForeignKey(d => d.SubmissionObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_FDBK_MSG_SUBM_OBJ_FK");
            });

            modelBuilder.Entity<HmrLocationCode>(entity =>
            {
                entity.HasKey(e => e.LocationCodeId)
                    .HasName("HMR_LOC_CODE_PK");

                entity.ToTable("HMR_LOCATION_CODE");

                entity.HasComment("Provides a code for each location type, applicable to each activity. Routine activities require general location and quantified activities require more detailed location information.");

                entity.Property(e => e.LocationCodeId)
                    .HasColumnName("LOCATION_CODE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_LOC_CODE_ID_SEQ])")
                    .HasComment("Unique identifier for a record");

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnName("ADDITIONAL_INFO")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Additional cross reference values that must be provided in a SUBMISSION OBJECT, dependant on the ACTIVITY NUMBER classification.  These are often Asset and Site identifiers from other systems (ie: Structure Number, Major Event Site Number).");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.LocationCode)
                    .IsRequired()
                    .HasColumnName("LOCATION_CODE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("Unique identifier for required submission details regarding positional references and submission detail level.");

                entity.Property(e => e.ReportingFrequency)
                    .HasColumnName("REPORTING_FREQUENCY")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Describes level of summary or detail that must be provided in a SUBMISSION OBJECT, dependant on the ACTIVITY NUMBER classification.  (ie: a summary record within a monthly submission, or daily records within a monthly submission)");

                entity.Property(e => e.RequiredLocationDetails)
                    .HasColumnName("REQUIRED_LOCATION_DETAILS")
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasComment("Listing of positional supporting references that must be provided in a SUBMISSION OBJECT dependant on the ACTIVITY NUMBER classification.  Routine activities require general location and quantified activities require more detailed location information.");
            });

            modelBuilder.Entity<HmrLocationCodeHist>(entity =>
            {
                entity.HasKey(e => e.LocationCodeHistId)
                    .HasName("HMR_LOC_C_H_PK");

                entity.ToTable("HMR_LOCATION_CODE_HIST");

                entity.HasIndex(e => new { e.LocationCodeHistId, e.EndDateHist })
                    .HasName("HMR_LOC_C_H_UK")
                    .IsUnique();

                entity.Property(e => e.LocationCodeHistId)
                    .HasColumnName("LOCATION_CODE_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_LOCATION_CODE_H_ID_SEQ])");

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnName("ADDITIONAL_INFO")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.LocationCode)
                    .IsRequired()
                    .HasColumnName("LOCATION_CODE")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LocationCodeId)
                    .HasColumnName("LOCATION_CODE_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ReportingFrequency)
                    .HasColumnName("REPORTING_FREQUENCY")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.RequiredLocationDetails)
                    .HasColumnName("REQUIRED_LOCATION_DETAILS")
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HmrMimeType>(entity =>
            {
                entity.HasKey(e => e.MimeTypeId)
                    .HasName("HMR_MIME_TYPE_PK");

                entity.ToTable("HMR_MIME_TYPE");

                entity.HasComment("MIME Type (Multipurpose Internet Mail Extensions) is a method used by web browsers to associate files of a certain type with helper applications that display files of that type. ");

                entity.Property(e => e.MimeTypeId)
                    .HasColumnName("MIME_TYPE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_MIME_TYPE_ID_SEQ])")
                    .HasComment("Unique identifier for a record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('0')")
                    .HasComment("A translation of the convention used by web browsers to associate files of a certain type with helper applications that display files of that type. ");

                entity.Property(e => e.MimeTypeCode)
                    .IsRequired()
                    .HasColumnName("MIME_TYPE_CODE")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("A convention used by web browsers to associate files of a certain type with helper applications that display files of that type. ");
            });

            modelBuilder.Entity<HmrParty>(entity =>
            {
                entity.HasKey(e => e.PartyId)
                    .HasName("HMR_PRTY_PK");

                entity.ToTable("HMR_PARTY");

                entity.HasComment("External organizations or Ministry branches/offices.");

                entity.Property(e => e.PartyId)
                    .HasColumnName("PARTY_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_PRTY_ID_SEQ])")
                    .HasComment("Unique record identifier");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.BusinessGuid)
                    .HasColumnName("BUSINESS_GUID")
                    .HasComment("A system generated unique identifier.  Reflects the active directory unique idenifier for the business associated with the user.");

                entity.Property(e => e.BusinessLegalName)
                    .HasColumnName("BUSINESS_LEGAL_NAME")
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasComment(@"Lega lName assigned to the business and derived from BC Registry via BCeID (SMGOV_BUSINESSLEGALNAME)
");

                entity.Property(e => e.BusinessNumber)
                    .HasColumnName("BUSINESS_NUMBER")
                    .HasColumnType("numeric(20, 0)")
                    .HasComment("Business Number assigned to the organization (SMGOV_BUSINESSNUMBER)");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.DisplayName)
                    .HasColumnName("DISPLAY_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Organization name displayed to users if different from the legal name.");

                entity.Property(e => e.Email)
                    .HasColumnName("EMAIL")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("Business BCeID Email");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("date")
                    .HasComment("Date reflecting when submissions are no longer expected from the organization.");

                entity.Property(e => e.PartyType)
                    .HasColumnName("PARTY_TYPE")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("Classifies Party as either External or Internal to enable relationships between staff users and their branch as well as contractor submission users and their ogranizations.");

                entity.Property(e => e.Telephone)
                    .HasColumnName("TELEPHONE")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("Business BCeID telephone number");
            });

            modelBuilder.Entity<HmrPartyHist>(entity =>
            {
                entity.HasKey(e => e.PartyHistId)
                    .HasName("HMR_PRTY_H_PK");

                entity.ToTable("HMR_PARTY_HIST");

                entity.HasIndex(e => new { e.PartyHistId, e.EndDateHist })
                    .HasName("HMR_PRTY_H_UK")
                    .IsUnique();

                entity.Property(e => e.PartyHistId)
                    .HasColumnName("PARTY_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_PARTY_H_ID_SEQ])");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessGuid).HasColumnName("BUSINESS_GUID");

                entity.Property(e => e.BusinessLegalName)
                    .HasColumnName("BUSINESS_LEGAL_NAME")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessNumber)
                    .HasColumnName("BUSINESS_NUMBER")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayName)
                    .HasColumnName("DISPLAY_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Email)
                    .HasColumnName("EMAIL")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.PartyId)
                    .HasColumnName("PARTY_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.PartyType)
                    .HasColumnName("PARTY_TYPE")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Telephone)
                    .HasColumnName("TELEPHONE")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HmrPermission>(entity =>
            {
                entity.HasKey(e => e.PermissionId)
                    .HasName("HMR_PERMISSION_PK");

                entity.ToTable("HMR_PERMISSION");

                entity.HasComment("Permission definition table for assignment to individual system users.");

                entity.Property(e => e.PermissionId)
                    .HasColumnName("PERMISSION_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_PERM_ID_SEQ])")
                    .HasComment("Unique identifier for a record");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasComment("Description of a permission.");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("date")
                    .HasComment("Date permission was deactivated");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Business name for a permission");
            });

            modelBuilder.Entity<HmrPermissionHist>(entity =>
            {
                entity.HasKey(e => e.PermissionHistId)
                    .HasName("HMR_PERM_H_PK");

                entity.ToTable("HMR_PERMISSION_HIST");

                entity.HasIndex(e => new { e.PermissionHistId, e.EndDateHist })
                    .HasName("HMR_PERM_H_UK")
                    .IsUnique();

                entity.Property(e => e.PermissionHistId)
                    .HasColumnName("PERMISSION_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_PERMISSION_H_ID_SEQ])");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PermissionId)
                    .HasColumnName("PERMISSION_ID")
                    .HasColumnType("numeric(18, 0)");
            });

            modelBuilder.Entity<HmrRegion>(entity =>
            {
                entity.HasKey(e => e.RegionNumber)
                    .HasName("HMR_REGION_PK");

                entity.ToTable("HMR_REGION");

                entity.HasComment("Ministry Region lookup values");

                entity.HasIndex(e => new { e.RegionNumber, e.RegionName })
                    .HasName("HMR_REG_NO_NAME_UK")
                    .IsUnique();

                entity.Property(e => e.RegionNumber)
                    .HasColumnName("REGION_NUMBER")
                    .HasColumnType("numeric(2, 0)")
                    .HasComment("Number assigned to the Ministry region");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.RegionId)
                    .HasColumnName("REGION_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_REG_ID_SEQ])")
                    .HasComment("A ministry organizational unit responsible for an exclusive geographic area within the province.  ");

                entity.Property(e => e.RegionName)
                    .IsRequired()
                    .HasColumnName("REGION_NAME")
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasComment("Name of the Ministry region");
            });

            modelBuilder.Entity<HmrRockfallReport>(entity =>
            {
                entity.HasKey(e => e.RockfallReportId)
                    .HasName("HMR_RCKFL_RPT_PK");

                entity.ToTable("HMR_ROCKFALL_REPORT");

                entity.HasComment("Submission data regarding rockfall incidents is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.");

                entity.HasIndex(e => new { e.SubmissionObjectId, e.RowId })
                    .HasName("HMR_RCKFL_RPT_FK_I");

                entity.Property(e => e.RockfallReportId)
                    .HasColumnName("ROCKFALL_REPORT_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_RCKF_ID_SEQ])")
                    .HasComment("A system generated unique identifier.");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.Comments)
                    .HasColumnName("COMMENTS")
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasComment("Comments of occurrence");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.DirectionFromLandmark)
                    .HasColumnName("DIRECTION_FROM_LANDMARK")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("Direction of travel from Landmark to START_OFFSET");

                entity.Property(e => e.DitchSnowIce)
                    .HasColumnName("DITCH_SNOW_ICE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("Ditch snow or ice conditions present at rockfall site. Enter “Y” or leave blank.");

                entity.Property(e => e.DitchVolume)
                    .HasColumnName("DITCH_VOLUME")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Range of estimated volume of material in ditch (m cubed). if volume exceeds 5.0 m3 and report value in the other volume field.");

                entity.Property(e => e.EndLatitude)
                    .HasColumnName("END_LATITUDE")
                    .HasColumnType("numeric(16, 8)")
                    .HasComment("The M (northing) portion of the activity end coordinate. Specified as a latitude in decimal degrees with six decimal places of precision. Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum. For point activity if this field is not provided it can be defaulted to same as START LATITUDE");

                entity.Property(e => e.EndLongitude)
                    .HasColumnName("END_LONGITUDE")
                    .HasColumnType("numeric(16, 8)")
                    .HasComment("The X (easting) portion of the activity end coordinate. Specified as a longitude in decimal degrees with six decimal places of precision. Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum. For point activity if this field is not provided it can be defaulted to same as START LONGITUDE.");

                entity.Property(e => e.EndOffset)
                    .HasColumnName("END_OFFSET")
                    .HasColumnType("numeric(7, 3)")
                    .HasComment("This field is needed for linear referencing for location specific reports. If the work is less than 30 m, this field is not mandatory Offset from beginning of segment");

                entity.Property(e => e.EstimatedRockfallDate)
                    .HasColumnName("ESTIMATED_ROCKFALL_DATE")
                    .HasColumnType("date")
                    .HasComment("Estimated date of occurrence.");

                entity.Property(e => e.EstimatedRockfallTime)
                    .HasColumnName("ESTIMATED_ROCKFALL_TIME")
                    .HasComment("Estimated time of occurrence using the 24-hour clock");

                entity.Property(e => e.FreezeThaw)
                    .HasColumnName("FREEZE_THAW")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("Freezing/thawing conditions present at rockfall site. Enter “Y” or leave blank.");

                entity.Property(e => e.Geometry)
                    .HasColumnName("GEOMETRY")
                    .HasColumnType("geometry")
                    .HasComment("Spatial geometry where the event occured, as conformed to the road network.   ");

                entity.Property(e => e.HeavyPrecip)
                    .HasColumnName("HEAVY_PRECIP")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("Heavy precipitation conditions present at rockfall site. Enter “Y” or leave blank.");

                entity.Property(e => e.HighwayUnique)
                    .HasColumnName("HIGHWAY_UNIQUE")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasComment("This identifies the section of road on which the activity occurred.  Road or Highway number sourced from a road network data product (RFI as of  2019) This is a value in the in the format: [Service Area]-[area manager area]-[subarea]-[highway number]");

                entity.Property(e => e.HighwayUniqueLength)
                    .HasColumnName("HIGHWAY_UNIQUE_LENGTH")
                    .HasColumnType("numeric(25, 20)")
                    .HasComment("Driven length in KM of the HIGHWAY_UNIQUE segment at the time of data submission.  ");

                entity.Property(e => e.HighwayUniqueName)
                    .HasColumnName("HIGHWAY_UNIQUE_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasComment("Road or Highway description sourced from a road network data product (RFI as of Dec 2019)");

                entity.Property(e => e.Landmark)
                    .HasColumnName("LANDMARK")
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasComment("This field needed for location reference: Landmarks provided should be those listed in the CHRIS HRP report for each Highway or road within the Service Area");

                entity.Property(e => e.LandmarkName)
                    .HasColumnName("LANDMARK_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasComment("Highway reference point (HRP) landmark.  This reference name reflects a valid landmark in the infrastructure asset management system (currenlty CHRIS as of 2019)");

                entity.Property(e => e.LocationDescription)
                    .HasColumnName("LOCATION_DESCRIPTION")
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasComment("Text field for comments and/or notes pertinent to the specified activity.");

                entity.Property(e => e.McPhoneNumber)
                    .HasColumnName("MC_PHONE_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Phone number of person reporting");

                entity.Property(e => e.McrrIncidentNumber)
                    .HasColumnName("MCRR_INCIDENT_NUMBER")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Rockfall reporting incident number. Unique work report record number from the Contractor maintenance management system.");

                entity.Property(e => e.OtherDitchVolume)
                    .HasColumnName("OTHER_DITCH_VOLUME")
                    .HasColumnType("numeric(6, 2)")
                    .HasComment("Ditch volume total when the estimated volume in the ditch exceeds 5.0 m3.");

                entity.Property(e => e.OtherTravelledLanesVolume)
                    .HasColumnName("OTHER_TRAVELLED_LANES_VOLUME")
                    .HasColumnType("numeric(6, 2)")
                    .HasComment("Travelled lanes volume total when the estimated volume in traveled lanes exceeds 5.0 m3.");

                entity.Property(e => e.RecordType)
                    .IsRequired()
                    .HasColumnName("RECORD_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("Alpha identifier for a Rockfall report submission.");

                entity.Property(e => e.RecordVersionNumber)
                    .HasColumnName("RECORD_VERSION_NUMBER")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ReportDate)
                    .HasColumnName("REPORT_DATE")
                    .HasColumnType("date")
                    .HasComment("Date reported");

                entity.Property(e => e.ReporterName)
                    .HasColumnName("REPORTER_NAME")
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasComment("Name of person reporting occurrence");

                entity.Property(e => e.RowId)
                    .HasColumnName("ROW_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for originating SUBMISSION ROW.");

                entity.Property(e => e.RowNum)
                    .HasColumnName("ROW_NUM")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Relative row number the record was located within a submission.");

                entity.Property(e => e.ServiceArea)
                    .HasColumnName("SERVICE_AREA")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("The Ministry Contract Service Area number in which the incident occured.");

                entity.Property(e => e.StartLatitude)
                    .HasColumnName("START_LATITUDE")
                    .HasColumnType("numeric(16, 8)")
                    .HasComment("The M (northing) portion of the activity start coordinate. Specified as a latitude in decimal degrees with six decimal places of precision. Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum.");

                entity.Property(e => e.StartLongitude)
                    .HasColumnName("START_LONGITUDE")
                    .HasColumnType("numeric(16, 8)")
                    .HasComment("The X (easting) portion of the activity start coordinate. Specified as a longitude in decimal degrees with six decimal places of precision. Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum.");

                entity.Property(e => e.StartOffset)
                    .HasColumnName("START_OFFSET")
                    .HasColumnType("numeric(7, 3)")
                    .HasComment("This field is needed for linear referencing for location specific reports.  Offset from beginning of segment.");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for SUBMISSION OBJECT.");

                entity.Property(e => e.TravelledLanesVolume)
                    .HasColumnName("TRAVELLED_LANES_VOLUME")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Range of estimated volume of material on the road (m cubed).");

                entity.Property(e => e.ValidationStatusId)
                    .HasColumnName("VALIDATION_STATUS_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for validation STATUS.  Indicates the overall status of the submitted row of data.");

                entity.Property(e => e.VehicleDamage)
                    .HasColumnName("VEHICLE_DAMAGE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("Vehicle damage present at rockfall site. Enter “Y” or leave blank.");

                entity.HasOne(d => d.Row)
                    .WithMany(p => p.HmrRockfallReports)
                    .HasForeignKey(d => d.RowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_RCKF_RRT_SUBM_RW_FK");

                entity.HasOne(d => d.ServiceAreaNavigation)
                    .WithMany(p => p.HmrRockfallReports)
                    .HasForeignKey(d => d.ServiceArea)
                    .HasConstraintName("HMR_RCKFL_RPT_HMR_SRV_ARA_FK");

                entity.HasOne(d => d.SubmissionObject)
                    .WithMany(p => p.HmrRockfallReports)
                    .HasForeignKey(d => d.SubmissionObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_RCKFL_RPT_SUBM_OBJ_FK");

                entity.HasOne(d => d.ValidationStatus)
                    .WithMany(p => p.HmrRockfallReports)
                    .HasForeignKey(d => d.ValidationStatusId)
                    .HasConstraintName("HMR_RKFLL_RRT_SUBM_STAT_FK");
            });

            modelBuilder.Entity<HmrRockfallReportHist>(entity =>
            {
                entity.HasKey(e => e.RockfallReportHistId)
                    .HasName("HMR_RCKFL_H_PK");

                entity.ToTable("HMR_ROCKFALL_REPORT_HIST");

                entity.HasIndex(e => new { e.RockfallReportHistId, e.EndDateHist })
                    .HasName("HMR_RCKFL_H_UK")
                    .IsUnique();

                entity.Property(e => e.RockfallReportHistId)
                    .HasColumnName("ROCKFALL_REPORT_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_ROCKFALL_REPORT_H_ID_SEQ])");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Comments)
                    .HasColumnName("COMMENTS")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DirectionFromLandmark)
                    .HasColumnName("DIRECTION_FROM_LANDMARK")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.DitchSnowIce)
                    .HasColumnName("DITCH_SNOW_ICE")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.DitchVolume)
                    .HasColumnName("DITCH_VOLUME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndLatitude)
                    .HasColumnName("END_LATITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.EndLongitude)
                    .HasColumnName("END_LONGITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.EndOffset)
                    .HasColumnName("END_OFFSET")
                    .HasColumnType("numeric(7, 3)");

                entity.Property(e => e.EstimatedRockfallDate)
                    .HasColumnName("ESTIMATED_ROCKFALL_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.EstimatedRockfallTime).HasColumnName("ESTIMATED_ROCKFALL_TIME");

                entity.Property(e => e.FreezeThaw)
                    .HasColumnName("FREEZE_THAW")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Geometry)
                    .HasColumnName("GEOMETRY")
                    .HasColumnType("geometry");

                entity.Property(e => e.HeavyPrecip)
                    .HasColumnName("HEAVY_PRECIP")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.HighwayUnique)
                    .HasColumnName("HIGHWAY_UNIQUE")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.HighwayUniqueLength)
                    .HasColumnName("HIGHWAY_UNIQUE_LENGTH")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.HighwayUniqueName)
                    .HasColumnName("HIGHWAY_UNIQUE_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Landmark)
                    .HasColumnName("LANDMARK")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.LandmarkName)
                    .HasColumnName("LANDMARK_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LocationDescription)
                    .HasColumnName("LOCATION_DESCRIPTION")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.McPhoneNumber)
                    .HasColumnName("MC_PHONE_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.McrrIncidentNumber)
                    .HasColumnName("MCRR_INCIDENT_NUMBER")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.OtherDitchVolume)
                    .HasColumnName("OTHER_DITCH_VOLUME")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.OtherTravelledLanesVolume)
                    .HasColumnName("OTHER_TRAVELLED_LANES_VOLUME")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.RecordType)
                    .HasColumnName("RECORD_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RecordVersionNumber).HasColumnName("RECORD_VERSION_NUMBER");

                entity.Property(e => e.ReportDate)
                    .HasColumnName("REPORT_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.ReporterName)
                    .HasColumnName("REPORTER_NAME")
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.RockfallReportId)
                    .HasColumnName("ROCKFALL_REPORT_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.RowId)
                    .HasColumnName("ROW_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.RowNum)
                    .HasColumnName("ROW_NUM")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ServiceArea)
                    .HasColumnName("SERVICE_AREA")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.StartLatitude)
                    .HasColumnName("START_LATITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.StartLongitude)
                    .HasColumnName("START_LONGITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.StartOffset)
                    .HasColumnName("START_OFFSET")
                    .HasColumnType("numeric(7, 3)");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.TravelledLanesVolume)
                    .HasColumnName("TRAVELLED_LANES_VOLUME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ValidationStatusId)
                    .HasColumnName("VALIDATION_STATUS_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.VehicleDamage)
                    .HasColumnName("VEHICLE_DAMAGE")
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HmrRockfallReportVw>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("HMR_ROCKFALL_REPORT_VW");

                entity.Property(e => e.AppCreateTimestampUtc)
                    .HasColumnName("APP_CREATE_TIMESTAMP_UTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateTimestampUtc)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP_UTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.Comments)
                    .HasColumnName("COMMENTS")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.DirectionFromLandmark)
                    .HasColumnName("DIRECTION_FROM_LANDMARK")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.DitchSnowIce)
                    .HasColumnName("DITCH_SNOW_ICE")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.DitchVolume)
                    .HasColumnName("DITCH_VOLUME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EndLatitude)
                    .HasColumnName("END_LATITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.EndLongitude)
                    .HasColumnName("END_LONGITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.EndOffset)
                    .HasColumnName("END_OFFSET")
                    .HasColumnType("numeric(7, 3)");

                entity.Property(e => e.EndVariance)
                    .HasColumnName("END_VARIANCE")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.EstimatedRockfallDate)
                    .HasColumnName("ESTIMATED_ROCKFALL_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.EstimatedRockfallTime).HasColumnName("ESTIMATED_ROCKFALL_TIME");

                entity.Property(e => e.FileName)
                    .HasColumnName("FILE_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.FreezeThaw)
                    .HasColumnName("FREEZE_THAW")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Geometry)
                    .HasColumnName("GEOMETRY")
                    .HasColumnType("geometry");

                entity.Property(e => e.HeavyPrecip)
                    .HasColumnName("HEAVY_PRECIP")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.HighwayUnique)
                    .HasColumnName("HIGHWAY_UNIQUE")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.HighwayUniqueLength)
                    .HasColumnName("HIGHWAY_UNIQUE_LENGTH")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.HighwayUniqueName)
                    .HasColumnName("HIGHWAY_UNIQUE_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.IsOverSpThreshold)
                    .IsRequired()
                    .HasColumnName("IS_OVER_SP_THRESHOLD")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Landmark)
                    .HasColumnName("LANDMARK")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.LandmarkName)
                    .HasColumnName("LANDMARK_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LocationDescription)
                    .HasColumnName("LOCATION_DESCRIPTION")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.McName)
                    .HasColumnName("MC_NAME")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.McPhoneNumber)
                    .HasColumnName("MC_PHONE_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.McrrIncidentNumber)
                    .HasColumnName("MCRR_INCIDENT_NUMBER")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.OtherDitchVolume)
                    .HasColumnName("OTHER_DITCH_VOLUME")
                    .HasColumnType("numeric(6, 2)");

                entity.Property(e => e.OtherTravelledLanesVolume)
                    .HasColumnName("OTHER_TRAVELLED_LANES_VOLUME")
                    .HasColumnType("numeric(6, 2)");

                entity.Property(e => e.RecordType)
                    .HasColumnName("RECORD_TYPE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.RecordVersionNumber).HasColumnName("RECORD_VERSION_NUMBER");

                entity.Property(e => e.ReportDate)
                    .HasColumnName("REPORT_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.ReportType)
                    .IsRequired()
                    .HasColumnName("REPORT_TYPE")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ReporterName)
                    .HasColumnName("REPORTER_NAME")
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.RockfallReportId)
                    .HasColumnName("ROCKFALL_REPORT_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.RowNum)
                    .HasColumnName("ROW_NUM")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.ServiceArea)
                    .HasColumnName("SERVICE_AREA")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.StartLatitude)
                    .HasColumnName("START_LATITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.StartLongitude)
                    .HasColumnName("START_LONGITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.StartOffset)
                    .HasColumnName("START_OFFSET")
                    .HasColumnType("numeric(7, 3)");

                entity.Property(e => e.StartVariance)
                    .HasColumnName("START_VARIANCE")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.SubmissionDate)
                    .HasColumnName("SUBMISSION_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.TravelledLanesVolume)
                    .HasColumnName("TRAVELLED_LANES_VOLUME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ValidationStatus)
                    .HasColumnName("VALIDATION_STATUS")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.VehicleDamage)
                    .HasColumnName("VEHICLE_DAMAGE")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.WarningSpThreshold)
                    .HasColumnName("WARNING_SP_THRESHOLD")
                    .HasColumnType("numeric(12, 6)");
            });

            modelBuilder.Entity<HmrRole>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("HMR_ROLE_PK");

                entity.ToTable("HMR_ROLE");

                entity.HasComment("Role description table for groups of permissions.");

                entity.Property(e => e.RoleId)
                    .HasColumnName("ROLE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_RL_ID_SEQ])")
                    .HasComment("Unique identifier for a record");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasComment("Description of a permission.");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("date")
                    .HasComment("Date permission was deactivated");

                entity.Property(e => e.IsInternal)
                    .HasColumnName("IS_INTERNAL")
                    .HasComment("Indicates if the role is only appropriate for users in a contract management position.");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Business name for a permission");
            });

            modelBuilder.Entity<HmrRoleHist>(entity =>
            {
                entity.HasKey(e => e.RoleHistId)
                    .HasName("HMR_RL_H_PK");

                entity.ToTable("HMR_ROLE_HIST");

                entity.HasIndex(e => new { e.RoleHistId, e.EndDateHist })
                    .HasName("HMR_RL_H_UK")
                    .IsUnique();

                entity.Property(e => e.RoleHistId)
                    .HasColumnName("ROLE_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_ROLE_H_ID_SEQ])");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsInternal).HasColumnName("IS_INTERNAL");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId)
                    .HasColumnName("ROLE_ID")
                    .HasColumnType("numeric(18, 0)");
            });

            modelBuilder.Entity<HmrRolePermission>(entity =>
            {
                entity.HasKey(e => e.RolePermissionId)
                    .HasName("HMR_RL_PERM_PK");

                entity.ToTable("HMR_ROLE_PERMISSION");

                entity.HasComment("Role to Permission associative table for assignment of permissions to parent roles.");

                entity.HasIndex(e => e.PermissionId)
                    .HasName("HMR_RL_PERM_PERM_FK_I");

                entity.HasIndex(e => e.RoleId)
                    .HasName("HMR_RL_PERM_RL_FK_I");

                entity.HasIndex(e => new { e.RoleId, e.PermissionId, e.EndDate })
                    .HasName("HMR_RL_PERM_UN_CH")
                    .IsUnique();

                entity.Property(e => e.RolePermissionId)
                    .HasColumnName("ROLE_PERMISSION_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_RL_PERM_ID_SEQ])")
                    .HasComment("Unique identifier for a record");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("date")
                    .HasComment("Date record was deactivated");

                entity.Property(e => e.PermissionId)
                    .HasColumnName("PERMISSION_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique idenifier for related permission");

                entity.Property(e => e.RoleId)
                    .HasColumnName("ROLE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique idenifier for related role");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.HmrRolePermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_RL_PERM_PERM_FK");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.HmrRolePermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_RL_PERM_RL_FK");
            });

            modelBuilder.Entity<HmrRolePermissionHist>(entity =>
            {
                entity.HasKey(e => e.RolePermissionHistId)
                    .HasName("HMR_RL_PE_H_PK");

                entity.ToTable("HMR_ROLE_PERMISSION_HIST");

                entity.HasIndex(e => new { e.RolePermissionHistId, e.EndDateHist })
                    .HasName("HMR_RL_PE_H_UK")
                    .IsUnique();

                entity.Property(e => e.RolePermissionHistId)
                    .HasColumnName("ROLE_PERMISSION_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_ROLE_PERMISSION_H_ID_SEQ])");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.PermissionId)
                    .HasColumnName("PERMISSION_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.RoleId)
                    .HasColumnName("ROLE_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.RolePermissionId)
                    .HasColumnName("ROLE_PERMISSION_ID")
                    .HasColumnType("numeric(18, 0)");
            });

            modelBuilder.Entity<HmrServiceArea>(entity =>
            {
                entity.HasKey(e => e.ServiceAreaNumber)
                    .HasName("HMR_SERVICE_AREA_PK");

                entity.ToTable("HMR_SERVICE_AREA");

                entity.HasComment("Service Area lookup values");

                entity.HasIndex(e => new { e.ServiceAreaNumber, e.ServiceAreaName })
                    .HasName("HMR_SRV_ARA_UK")
                    .IsUnique();

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Assigned number of the Service Area");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.DistrictNumber)
                    .HasColumnName("DISTRICT_NUMBER")
                    .HasColumnType("numeric(2, 0)")
                    .HasComment("Unique identifier for DISTRICT.");

                entity.Property(e => e.HighwayUniquePrefix)
                    .HasColumnName("HIGHWAY_UNIQUE_PREFIX")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Determines the tolerated spatial variance allowed between submitted activity coordinates and the related Highway Unique road segment");

                entity.Property(e => e.ServiceAreaId)
                    .HasColumnName("SERVICE_AREA_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SRV_ARA_ID_SEQ])")
                    .HasComment("Unique idenifier for table records");

                entity.Property(e => e.ServiceAreaName)
                    .IsRequired()
                    .HasColumnName("SERVICE_AREA_NAME")
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasComment("Name of the service area");

                entity.HasOne(d => d.DistrictNumberNavigation)
                    .WithMany(p => p.HmrServiceAreas)
                    .HasForeignKey(d => d.DistrictNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SRV_AREA_DISTRICT_FK");
            });

            modelBuilder.Entity<HmrServiceAreaActivity>(entity =>
            {
                entity.HasKey(e => e.ServiceAreaActivityId)
                    .HasName("PK__HMR_SERV__56CBEAEDE93A5D16");

                entity.ToTable("HMR_SERVICE_AREA_ACTIVITY");

                entity.HasIndex(e => e.ActivityCodeId)
                    .HasName("IDX_HMR_SVC_AR_ACT_ACT_CD");

                entity.HasIndex(e => e.ServiceAreaNumber)
                    .HasName("IDX_HMR_SVC_AR_ACT_SVC_AREA");

                entity.HasIndex(e => new { e.ServiceAreaNumber, e.ActivityCodeId })
                    .HasName("UQ__HMR_SERV__307B5DE2288F04C9")
                    .IsUnique();

                entity.Property(e => e.ServiceAreaActivityId)
                    .HasColumnName("SERVICE_AREA_ACTIVITY_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SERVICE_AREA_ACTIVITY_ID_SEQ])");

                entity.Property(e => e.ActivityCodeId)
                    .HasColumnName("ACTIVITY_CODE_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(9, 0)");

                entity.HasOne(d => d.ActivityCode)
                    .WithMany(p => p.HmrServiceAreaActivities)
                    .HasForeignKey(d => d.ActivityCodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HMR_SERVI__ACTIV__4356F04A");

                entity.HasOne(d => d.ServiceAreaNumberNavigation)
                    .WithMany(p => p.HmrServiceAreaActivities)
                    .HasForeignKey(d => d.ServiceAreaNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HMR_SERVI__SERVI__4262CC11");
            });

            modelBuilder.Entity<HmrServiceAreaActivityHist>(entity =>
            {
                entity.HasKey(e => e.ServiceAreaActivityHistId)
                    .HasName("PK__HMR_SERV__08C5944CC1FB708E");

                entity.ToTable("HMR_SERVICE_AREA_ACTIVITY_HIST");

                entity.Property(e => e.ServiceAreaActivityHistId)
                    .HasColumnName("SERVICE_AREA_ACTIVITY_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SERVICE_AREA_ACTIVITY_H_ID_SEQ])");

                entity.Property(e => e.ActivityCodeId)
                    .HasColumnName("ACTIVITY_CODE_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())");

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.ServiceAreaActivityId)
                    .HasColumnName("SERVICE_AREA_ACTIVITY_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(9, 0)");
            });

            modelBuilder.Entity<HmrServiceAreaHist>(entity =>
            {
                entity.HasKey(e => e.ServiceAreaHistId)
                    .HasName("HMR_SRV_A_H_PK");

                entity.ToTable("HMR_SERVICE_AREA_HIST");

                entity.HasIndex(e => new { e.ServiceAreaHistId, e.EndDateHist })
                    .HasName("HMR_SRV_A_H_UK")
                    .IsUnique();

                entity.Property(e => e.ServiceAreaHistId)
                    .HasColumnName("SERVICE_AREA_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SERVICE_AREA_H_ID_SEQ])");

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DistrictNumber)
                    .HasColumnName("DISTRICT_NUMBER")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.HighwayUniquePrefix)
                    .HasColumnName("HIGHWAY_UNIQUE_PREFIX")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceAreaId)
                    .HasColumnName("SERVICE_AREA_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ServiceAreaName)
                    .IsRequired()
                    .HasColumnName("SERVICE_AREA_NAME")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(18, 0)");
            });

            modelBuilder.Entity<HmrServiceAreaUser>(entity =>
            {
                entity.HasKey(e => e.ServiceAreaUserId)
                    .HasName("HMR_SRV_ARA_USR_PK");

                entity.ToTable("HMR_SERVICE_AREA_USER");

                entity.HasComment("Association between USER and SERVICE_AREA defining which users can submit or access data.");

                entity.HasIndex(e => new { e.ServiceAreaNumber, e.SystemUserId })
                    .HasName("HMR_SERVICE_AREA_USER_FK_I");

                entity.Property(e => e.ServiceAreaUserId)
                    .HasColumnName("SERVICE_AREA_USER_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SRV_AREA_USR_ID_SEQ])")
                    .HasComment("Unique identifier for SERVICE AREA");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime")
                    .HasComment("Date reflecting when a user can no longer transmit submissions.");

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for SERVICE AREA");

                entity.Property(e => e.SystemUserId)
                    .HasColumnName("SYSTEM_USER_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier of related user");

                entity.HasOne(d => d.ServiceAreaNumberNavigation)
                    .WithMany(p => p.HmrServiceAreaUsers)
                    .HasForeignKey(d => d.ServiceAreaNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SRV_AREA_USR_SRV_AREA_FK");

                entity.HasOne(d => d.SystemUser)
                    .WithMany(p => p.HmrServiceAreaUsers)
                    .HasForeignKey(d => d.SystemUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SRV_AREA_USR_SYS_USR_FK");
            });

            modelBuilder.Entity<HmrServiceAreaUserHist>(entity =>
            {
                entity.HasKey(e => e.ServiceAreaUserHistId)
                    .HasName("HMR_SRV_A_U_H_PK");

                entity.ToTable("HMR_SERVICE_AREA_USER_HIST");

                entity.HasIndex(e => new { e.ServiceAreaUserHistId, e.EndDateHist })
                    .HasName("HMR_SRV_A_U_H_UK")
                    .IsUnique();

                entity.Property(e => e.ServiceAreaUserHistId)
                    .HasColumnName("SERVICE_AREA_USER_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SERVICE_AREA_USER_H_ID_SEQ])");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ServiceAreaUserId)
                    .HasColumnName("SERVICE_AREA_USER_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.SystemUserId)
                    .HasColumnName("SYSTEM_USER_ID")
                    .HasColumnType("numeric(18, 0)");
            });

            modelBuilder.Entity<HmrStreamElement>(entity =>
            {
                entity.HasKey(e => e.StreamElementId)
                    .HasName("HMR_STR_ELMT_PK");

                entity.ToTable("HMR_STREAM_ELEMENT");

                entity.HasComment("Stream Element values reflect elements within a submission. In a CSV submission, consider them relfective of the header record.  In a multidimentional submission, such as XML or JSON they can reflect any leaf node with the Document Object Model (DOM).  Any value within a submission that requires validation or transformation can be listed as an element and multiple tasks can be defined.");

                entity.Property(e => e.StreamElementId)
                    .HasColumnName("STREAM_ELEMENT_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_STR_ELMT_ID_SEQ])")
                    .HasComment("Unique identifier for a record");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.CodeSet)
                    .HasColumnName("CODE_SET")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("Unique identifier for a group of lookup codes.  Used to validate that submissions are within acceptable values.");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.ElementName)
                    .HasColumnName("ELEMENT_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Element names reflect header record or node identifiers within a submission.  Any value within a submission that requires validation or transformation can be listed as an element and multiple valid conditions  can be defined.");

                entity.Property(e => e.ElementType)
                    .HasColumnName("ELEMENT_TYPE")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Indicates type of element to be validated.  Can be STRING, NUMBERIC or DATE.  The type is used to determine which populated valudation rules apply.");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime")
                    .HasComment("The latest date submissions will be accepted.");

                entity.Property(e => e.IsRequired)
                    .HasColumnName("IS_REQUIRED")
                    .HasComment("Indicates the value must be populated in all submissions.");

                entity.Property(e => e.MaxDate)
                    .HasColumnName("MAX_DATE")
                    .HasColumnType("datetime")
                    .HasComment("For validating submission values are within acceptable range ");

                entity.Property(e => e.MaxLength)
                    .HasColumnName("MAX_LENGTH")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("For validating submission values are within acceptable length range.");

                entity.Property(e => e.MaxValue)
                    .HasColumnName("MAX_VALUE")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("For validating submission values are within acceptable length range.");

                entity.Property(e => e.MinDate)
                    .HasColumnName("MIN_DATE")
                    .HasColumnType("datetime")
                    .HasComment("For validating submission values are within acceptable range ");

                entity.Property(e => e.MinLength)
                    .HasColumnName("MIN_LENGTH")
                    .HasColumnType("numeric(9, 2)")
                    .HasComment("For validating submission values are within acceptable range ");

                entity.Property(e => e.MinValue)
                    .HasColumnName("MIN_VALUE")
                    .HasColumnType("numeric(9, 2)")
                    .HasComment("For validating submission values are within acceptable range ");

                entity.Property(e => e.RegExp)
                    .HasColumnName("REG_EXP")
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasComment("Regular expression used for validation of submitted value patterns.");

                entity.Property(e => e.StagingColumnName)
                    .HasColumnName("STAGING_COLUMN_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("The target field  within the target staging tables for the submitted element.  Used to enable business logic for loading submission data into target staging tables.");

                entity.Property(e => e.SubmissionStreamId)
                    .HasColumnName("SUBMISSION_STREAM_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("A system generated unique identifier.");

                entity.HasOne(d => d.SubmissionStream)
                    .WithMany(p => p.HmrStreamElements)
                    .HasForeignKey(d => d.SubmissionStreamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_STR_ELMTS_SUBM_STREAM_FK");
            });

            modelBuilder.Entity<HmrStreamElementHist>(entity =>
            {
                entity.HasKey(e => e.StreamElementHistId)
                    .HasName("HMR_STR_E_H_PK");

                entity.ToTable("HMR_STREAM_ELEMENT_HIST");

                entity.HasIndex(e => new { e.StreamElementHistId, e.EndDateHist })
                    .HasName("HMR_STR_E_H_UK")
                    .IsUnique();

                entity.Property(e => e.StreamElementHistId)
                    .HasColumnName("STREAM_ELEMENT_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_STREAM_ELEMENT_H_ID_SEQ])");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CodeSet)
                    .HasColumnName("CODE_SET")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.ElementName)
                    .HasColumnName("ELEMENT_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ElementType)
                    .HasColumnName("ELEMENT_TYPE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsRequired).HasColumnName("IS_REQUIRED");

                entity.Property(e => e.MaxDate)
                    .HasColumnName("MAX_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.MaxLength)
                    .HasColumnName("MAX_LENGTH")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.MaxValue)
                    .HasColumnName("MAX_VALUE")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.MinDate)
                    .HasColumnName("MIN_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.MinLength)
                    .HasColumnName("MIN_LENGTH")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.MinValue)
                    .HasColumnName("MIN_VALUE")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.RegExp)
                    .HasColumnName("REG_EXP")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.StagingColumnName)
                    .HasColumnName("STAGING_COLUMN_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.StreamElementId)
                    .HasColumnName("STREAM_ELEMENT_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.SubmissionStreamId)
                    .HasColumnName("SUBMISSION_STREAM_ID")
                    .HasColumnType("numeric(18, 0)");
            });

            modelBuilder.Entity<HmrSubmissionObject>(entity =>
            {
                entity.HasKey(e => e.SubmissionObjectId)
                    .HasName("HMR_SUBM_OBJ_PK");

                entity.ToTable("HMR_SUBMISSION_OBJECT");

                entity.HasComment("Digital file containing a batch of records being submitted for validation,  ingestion and reporting.");

                entity.HasIndex(e => new { e.SubmissionStatusId, e.ServiceAreaNumber, e.SubmissionStreamId })
                    .HasName("HMR_SUBM_OBJ_FK_I");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBM_OBJ_ID_SEQ])")
                    .HasComment("Unique identifier for a record");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.ContractTermId)
                    .HasColumnName("CONTRACT_TERM_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for CONTRACT TERM");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.DigitalRepresentation)
                    .IsRequired()
                    .HasColumnName("DIGITAL_REPRESENTATION")
                    .HasComment("Raw file storage within the database.");

                entity.Property(e => e.ErrorDetail)
                    .HasColumnName("ERROR_DETAIL")
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasComment("Error descriptions applicable to an early stage file validation error (eg: missing mandatory column).");

                entity.Property(e => e.FileHash)
                    .HasColumnName("FILE_HASH")
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasComment("Cryptographic hash for each submission object received. The hash total is used to compare with subsequently submitted data to check for duplicate submissions. If a match exists, newly matched data is not processed further.");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasColumnName("FILE_NAME")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("The name of the document file name as was supplied by the user.");

                entity.Property(e => e.MimeTypeId)
                    .HasColumnName("MIME_TYPE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Multipurpose Internet Mail Extensions (MIME) type of the submitted file");

                entity.Property(e => e.PartyId)
                    .HasColumnName("PARTY_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier of related PARTY record");

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for SERVICE AREA");

                entity.Property(e => e.SubmissionStatusId)
                    .HasColumnName("SUBMISSION_STATUS_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier relecting the current status of the submission.");

                entity.Property(e => e.SubmissionStreamId)
                    .HasColumnName("SUBMISSION_STREAM_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for SUBMISSION STREAM");

                entity.HasOne(d => d.ContractTerm)
                    .WithMany(p => p.HmrSubmissionObjects)
                    .HasForeignKey(d => d.ContractTermId)
                    .HasConstraintName("HMR_SUBM_OBJ_CNT_TRM_FK");

                entity.HasOne(d => d.MimeType)
                    .WithMany(p => p.HmrSubmissionObjects)
                    .HasForeignKey(d => d.MimeTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HRM_SUBM_OBJ_MIME_TYPE_FK");

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.HmrSubmissionObjects)
                    .HasForeignKey(d => d.PartyId)
                    .HasConstraintName("HMR_SUBM_OBJ_PRTY_FK");

                entity.HasOne(d => d.ServiceAreaNumberNavigation)
                    .WithMany(p => p.HmrSubmissionObjects)
                    .HasForeignKey(d => d.ServiceAreaNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HRM_SUBM_OBJ_SRV_AREA_FK");

                entity.HasOne(d => d.SubmissionStatus)
                    .WithMany(p => p.HmrSubmissionObjects)
                    .HasForeignKey(d => d.SubmissionStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HRM_SUBM_OBJ_SUBM_STAT_CODE_FK");

                entity.HasOne(d => d.SubmissionStream)
                    .WithMany(p => p.HmrSubmissionObjects)
                    .HasForeignKey(d => d.SubmissionStreamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SUBM_OBJ_SUBM_STR_FK");
            });

            modelBuilder.Entity<HmrSubmissionRow>(entity =>
            {
                entity.HasKey(e => e.RowId)
                    .HasName("HMR_SUBM_RW_PK")
                    .IsClustered(false);

                entity.ToTable("HMR_SUBMISSION_ROW");

                entity.HasComment("Each row of data within a  SUBMISSION OBJECT for each file submission that  passes basic file validation.");

                entity.Property(e => e.RowId)
                    .HasColumnName("ROW_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBM_RW_ID_SEQ])")
                    .HasComment("A system generated unique identifier.");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.EndVariance)
                    .HasColumnName("END_VARIANCE")
                    .HasColumnType("numeric(25, 20)")
                    .HasComment("Measured spatial distance from submitted coordinates to nearest road segment, as determined at time of validation.  Only applicable to submissions with end coordinates.");

                entity.Property(e => e.ErrorDetail)
                    .HasColumnName("ERROR_DETAIL")
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasComment("Full listing of validation errors in JSON format for the submitted row.");

                entity.Property(e => e.ErrorSpThreshold)
                    .HasColumnName("ERROR_SP_THRESHOLD")
                    .HasColumnType("numeric(12, 6)")
                    .HasComment("Spatial error threshold beyond which an error is raised, when comparing input and actual values");

                entity.Property(e => e.IsResubmitted)
                    .HasColumnName("IS_RESUBMITTED")
                    .HasComment("Indicates if the RECORD_NUMBER for the same CONTRACT_TERM and PARTY has been previously processed successfully and is being overwritten by a subsequent submission row.");

                entity.Property(e => e.RecordNumber)
                    .HasColumnName("RECORD_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique work report record number from the maintainence contractor. This is uniquely identifies each record submission for a contractor. <Service Area><Record Number> will uniquely identify each record in the application for a particular contractor.");

                entity.Property(e => e.RowHash)
                    .HasColumnName("ROW_HASH")
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasComment("Cryptographic hash for each row of data received. The hash total is is used to compared with subsequently submitted data to check for duplicate submissions. If a match exists, newly matched data is not processed further.");

                entity.Property(e => e.RowNum)
                    .HasColumnName("ROW_NUM")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Relative row number within the SUBMISSION_OBJECT.");

                entity.Property(e => e.RowStatusId)
                    .HasColumnName("ROW_STATUS_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier relecting the current status of the submission row.");

                entity.Property(e => e.RowValue)
                    .HasColumnName("ROW_VALUE")
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasComment("Contains a complete row of submitted data, including delimiters (ie: comma) and text qualifiers (ie: quote).  The row value is used to queue data for validation and loading.  This is staged data used to queue and compare data before loading it within the appropriate tables for reporting.");

                entity.Property(e => e.StartVariance)
                    .HasColumnName("START_VARIANCE")
                    .HasColumnType("numeric(25, 20)")
                    .HasComment("Measured spatial distance from submitted coordinates to nearest road segment, as determined at time of validation.");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for a SUBMISSION OBJECT record");

                entity.Property(e => e.WarningDetail)
                    .HasColumnName("WARNING_DETAIL")
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasComment("Full listing of validation warnings for the submitted row.  Thresholds can be  established whereby data will not be rejected, but a warning will be noted.");

                entity.Property(e => e.WarningSpThreshold)
                    .HasColumnName("WARNING_SP_THRESHOLD")
                    .HasColumnType("numeric(12, 6)")
                    .HasComment("Spatial warning threshold beyond which a warning is raised, when comparing input and actual values");

                entity.HasOne(d => d.RowStatus)
                    .WithMany(p => p.HmrSubmissionRows)
                    .HasForeignKey(d => d.RowStatusId)
                    .HasConstraintName("HMR_SUBM_RW_SUBM_STAT_FK");

                entity.HasOne(d => d.SubmissionObject)
                    .WithMany(p => p.HmrSubmissionRows)
                    .HasForeignKey(d => d.SubmissionObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SUBM_RW_HMR_SUBM_OBJ_FK");
            });

            modelBuilder.Entity<HmrSubmissionRowHist>(entity =>
            {
                entity.HasKey(e => e.SubmissionRowHistId)
                    .HasName("HMR_SUBM__H_PK");

                entity.ToTable("HMR_SUBMISSION_ROW_HIST");

                entity.HasIndex(e => new { e.SubmissionRowHistId, e.EndDateHist })
                    .HasName("HMR_SUBM__H_UK")
                    .IsUnique();

                entity.Property(e => e.SubmissionRowHistId)
                    .HasColumnName("SUBMISSION_ROW_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBMISSION_ROW_H_ID_SEQ])");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndVariance)
                    .HasColumnName("END_VARIANCE")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.ErrorDetail)
                    .HasColumnName("ERROR_DETAIL")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorSpThreshold)
                    .HasColumnName("ERROR_SP_THRESHOLD")
                    .HasColumnType("numeric(12, 0)");

                entity.Property(e => e.IsResubmitted).HasColumnName("IS_RESUBMITTED");

                entity.Property(e => e.RecordNumber)
                    .HasColumnName("RECORD_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.RowHash)
                    .HasColumnName("ROW_HASH")
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.RowId)
                    .HasColumnName("ROW_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.RowNum)
                    .HasColumnName("ROW_NUM")
                    .HasColumnType("numeric(30, 0)");

                entity.Property(e => e.RowStatusId)
                    .HasColumnName("ROW_STATUS_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.RowValue)
                    .HasColumnName("ROW_VALUE")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.StartVariance)
                    .HasColumnName("START_VARIANCE")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.WarningDetail)
                    .HasColumnName("WARNING_DETAIL")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.WarningSpThreshold)
                    .HasColumnName("WARNING_SP_THRESHOLD")
                    .HasColumnType("numeric(12, 0)");
            });

            modelBuilder.Entity<HmrSubmissionStatu>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("HMR_SUBMISSION_STATUS_CODE_PK");

                entity.ToTable("HMR_SUBMISSION_STATUS");

                entity.HasComment("Indicates the statues a SUBMISSION_OBJECT can be assigned during ingestion (ie:  Received, Invalid, Valid)");

                entity.Property(e => e.StatusId)
                    .HasColumnName("STATUS_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBM_STAT_ID_SEQ])")
                    .HasComment("Unique identifier for a record.");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("DESCRIPTION")
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('0')")
                    .HasComment("Provides business description of the submission processing status");

                entity.Property(e => e.LongDescription)
                    .HasColumnName("LONG_DESCRIPTION")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasComment("Full description of the status code.");

                entity.Property(e => e.Stage)
                    .HasColumnName("STAGE")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Stages in the processing/parsing of a submitted file");

                entity.Property(e => e.StatusCode)
                    .IsRequired()
                    .HasColumnName("STATUS_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("Describes the file processing status.");

                entity.Property(e => e.StatusType)
                    .HasColumnName("STATUS_TYPE")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Indicates if status code is for SUBMISSION OBJECT, SUBMISSION  ROW or final staging table..");
            });

            modelBuilder.Entity<HmrSubmissionStream>(entity =>
            {
                entity.HasKey(e => e.SubmissionStreamId)
                    .HasName("HMR_SUBM_STR_PK");

                entity.ToTable("HMR_SUBMISSION_STREAM");

                entity.HasComment("Highway maintenance reporting submissions are defined by common attributes and often part of contractual obligations with 3rd party vendors.  The SUBMISSION STREAM reflects a type of reporting submission, which is used to drive validation and loading of the submitted data.  This table also defines the target staging table for the submtted data.");

                entity.Property(e => e.SubmissionStreamId)
                    .HasColumnName("SUBMISSION_STREAM_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBM_STR_ID_SEQ])")
                    .HasComment("Unique identifier for a record");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime")
                    .HasComment("The latest date submissions will be accepted.");

                entity.Property(e => e.FileSizeLimit)
                    .HasColumnName("FILE_SIZE_LIMIT")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("For validating submitted files are within a reasonable size");

                entity.Property(e => e.StagingTableName)
                    .HasColumnName("STAGING_TABLE_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Name of staging table for submission data");

                entity.Property(e => e.StreamName)
                    .HasColumnName("STREAM_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Business name for a submission stream that reflects a common set of submission subject area, format, validation rules, and transformations (ie:  Wildlife Accident Reporting (WAR)).");
            });

            modelBuilder.Entity<HmrSubmissionStreamHist>(entity =>
            {
                entity.HasKey(e => e.SubmissionStreamHistId)
                    .HasName("HMR_SUBM_U_H_PK");

                entity.ToTable("HMR_SUBMISSION_STREAM_HIST");

                entity.HasIndex(e => new { e.SubmissionStreamHistId, e.EndDateHist })
                    .HasName("HMR_SUBM_U_H_UK")
                    .IsUnique();

                entity.Property(e => e.SubmissionStreamHistId)
                    .HasColumnName("SUBMISSION_STREAM_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SUBMISSION_STREAM_H_ID_SEQ])");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.FileSizeLimit)
                    .HasColumnName("FILE_SIZE_LIMIT")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.StagingTableName)
                    .HasColumnName("STAGING_TABLE_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.StreamName)
                    .HasColumnName("STREAM_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SubmissionStreamId)
                    .HasColumnName("SUBMISSION_STREAM_ID")
                    .HasColumnType("numeric(18, 0)");
            });

            modelBuilder.Entity<HmrSystemUser>(entity =>
            {
                entity.HasKey(e => e.SystemUserId)
                    .HasName("HMR_SYSTEM_USER_PK");

                entity.ToTable("HMR_SYSTEM_USER");

                entity.HasComment("Defines users and their attributes as found in IDIR or BCeID.");

                entity.HasIndex(e => e.PartyId)
                    .HasName("HMR_SYSTEM_USER_FK_I");

                entity.Property(e => e.SystemUserId)
                    .HasColumnName("SYSTEM_USER_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [SYS_USR_ID_SEQ])")
                    .HasComment("A system generated unique identifier.");

                entity.Property(e => e.ApiClientId)
                    .HasColumnName("API_CLIENT_ID")
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasComment("This ID is used to track Keycloak client ID created for the users");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.BusinessGuid)
                    .HasColumnName("BUSINESS_GUID")
                    .HasComment("A system generated unique identifier.  Reflects the active directory unique idenifier for the business associated with the user.");

                entity.Property(e => e.BusinessLegalName)
                    .HasColumnName("BUSINESS_LEGAL_NAME")
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasComment("Lega lName assigned to the business and derived from BC Registry via BCeID (SMGOV_BUSINESSLEGALNAME)");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.Email)
                    .HasColumnName("EMAIL")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("Contact email address within Active Directory (Email = SMGOV_EMAIL)");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime")
                    .HasComment("Date a user can no longer access the system or invoke data submissions.");

                entity.Property(e => e.FirstName)
                    .HasColumnName("FIRST_NAME")
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasComment("First Name of the user");

                entity.Property(e => e.LastName)
                    .HasColumnName("LAST_NAME")
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasComment("Last Name of the user");

                entity.Property(e => e.PartyId)
                    .HasColumnName("PARTY_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("A system generated unique identifier.  Reflects the party record for the individual who has been assigned any roles.");

                entity.Property(e => e.UserDirectory)
                    .HasColumnName("USER_DIRECTORY")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Directory (IDIR / BCeID/Oracle) in which the userid is defined.");

                entity.Property(e => e.UserGuid)
                    .HasColumnName("USER_GUID")
                    .HasComment("A system generated unique identifier.  Reflects the active directory unique idenifier for the user.");

                entity.Property(e => e.UserType)
                    .HasColumnName("USER_TYPE")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Defined attribute within IDIR Active directory (UserType = SMGOV_USERTYPE)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("USERNAME")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("IDIR or BCeID Active Directory defined universal identifier (SM_UNIVERSALID or userID) attributed to a user.  This value can change over time, while USER_GUID will remain consistant.");

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.HmrSystemUsers)
                    .HasForeignKey(d => d.PartyId)
                    .HasConstraintName("HMR_SYS_USR_PRTY_FK");
            });

            modelBuilder.Entity<HmrSystemUserHist>(entity =>
            {
                entity.HasKey(e => e.SystemUserHistId)
                    .HasName("HMR_SYS_U_H_PK");

                entity.ToTable("HMR_SYSTEM_USER_HIST");

                entity.HasIndex(e => new { e.SystemUserHistId, e.EndDateHist })
                    .HasName("HMR_SYS_U_H_UK")
                    .IsUnique();

                entity.Property(e => e.SystemUserHistId)
                    .HasColumnName("SYSTEM_USER_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SYSTEM_USER_H_ID_SEQ])");

                entity.Property(e => e.ApiClientId)
                    .HasColumnName("API_CLIENT_ID")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessGuid).HasColumnName("BUSINESS_GUID");

                entity.Property(e => e.BusinessLegalName)
                    .HasColumnName("BUSINESS_LEGAL_NAME")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Email)
                    .HasColumnName("EMAIL")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.FirstName)
                    .HasColumnName("FIRST_NAME")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasColumnName("LAST_NAME")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.PartyId)
                    .HasColumnName("PARTY_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.SystemUserId)
                    .HasColumnName("SYSTEM_USER_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.UserDirectory)
                    .HasColumnName("USER_DIRECTORY")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UserGuid).HasColumnName("USER_GUID");

                entity.Property(e => e.UserType)
                    .HasColumnName("USER_TYPE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("USERNAME")
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HmrSystemValidation>(entity =>
            {
                entity.HasKey(e => e.SystemValidationId)
                    .HasName("HMR_SYS_VLD_PK");

                entity.ToTable("HMR_SYSTEM_VALIDATION");

                entity.HasComment("Stream Element values reflect elements within a submission. In a CSV submission, consider them relfective of the header record.  In a multidimentional submission, such as XML or JSON they can reflect any leaf node with the Document Object Model (DOM).  Any value within a submission that requires validation or transformation can be listed as an element and multiple tasks can be defined.");

                entity.Property(e => e.SystemValidationId)
                    .HasColumnName("SYSTEM_VALIDATION_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [SYS_VLD_ID_SEQ])")
                    .HasComment("A system generated unique identifier.");

                entity.Property(e => e.AttributeName)
                    .HasColumnName("ATTRIBUTE_NAME")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("Attribute names reflect user configured system attributes that require validation (eg:  Activity Codes, Users, etc.)");

                entity.Property(e => e.AttributeType)
                    .HasColumnName("ATTRIBUTE_TYPE")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Indicates type of attribute to be validated.  Can be STRING, NUMBERIC or DATE.  The type is used to determine which populated valudation rules apply.");

                entity.Property(e => e.CodeSet)
                    .HasColumnName("CODE_SET")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("Unique identifier for a group of lookup codes.  Used to validate that submissions are within acceptable values.");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime")
                    .HasComment("The latest date submissions will be accepted.");

                entity.Property(e => e.EntityName)
                    .HasColumnName("ENTITY_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Entity names reflect the parent table that contains configurable attributes that require validation (eg:  Activity Codes, Users, etc.)");

                entity.Property(e => e.IsRequired)
                    .HasColumnName("IS_REQUIRED")
                    .HasComment("Indicates the system attribute must be populated.");

                entity.Property(e => e.MaxDate)
                    .HasColumnName("MAX_DATE")
                    .HasColumnType("datetime")
                    .HasComment("For validating system attributes are within acceptable range ");

                entity.Property(e => e.MaxLength)
                    .HasColumnName("MAX_LENGTH")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("For validating system attributes are within acceptable length range.");

                entity.Property(e => e.MaxValue)
                    .HasColumnName("MAX_VALUE")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("For validating system attributes are within acceptable length range.");

                entity.Property(e => e.MinDate)
                    .HasColumnName("MIN_DATE")
                    .HasColumnType("datetime")
                    .HasComment("For validating system attributes are within acceptable range ");

                entity.Property(e => e.MinLength)
                    .HasColumnName("MIN_LENGTH")
                    .HasColumnType("numeric(9, 2)")
                    .HasComment("For validating system attributes are within acceptable range ");

                entity.Property(e => e.MinValue)
                    .HasColumnName("MIN_VALUE")
                    .HasColumnType("numeric(9, 2)")
                    .HasComment("For validating system attributes are within acceptable range ");

                entity.Property(e => e.RegExp)
                    .HasColumnName("REG_EXP")
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasComment("Regular expression used for validation of attribute value patterns.");
            });

            modelBuilder.Entity<HmrUserRole>(entity =>
            {
                entity.HasKey(e => e.UserRoleId)
                    .HasName("HMR_USR_RL_PK");

                entity.ToTable("HMR_USER_ROLE");
                entity.HasComment("Associative table for assignment of roles to individual system users.");

                entity.HasIndex(e => e.RoleId)
                    .HasName("HMR_USR_RL_RL_FK_I");

                entity.HasIndex(e => e.SystemUserId)
                    .HasName("HMR_USR_RL_USR_FK_I");

                entity.HasIndex(e => new { e.EndDate, e.SystemUserId, e.RoleId })
                    .HasName("HMR_USR_RL_UQ_CH")
                    .IsUnique();

                entity.Property(e => e.UserRoleId)
                    .HasColumnName("USER_ROLE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_USR_RL_ID_SEQ])")
                    .HasComment("Unique identifier for a record");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime")
                    .HasComment("Date a user is no longer assigned the role.  The APP_CREATED_TIMESTAMP and the END_DATE can be used to determine which roles were assigned to a user at a given point in time.");

                entity.Property(e => e.RoleId)
                    .HasColumnName("ROLE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for related ROLE");

                entity.Property(e => e.SystemUserId)
                    .HasColumnName("SYSTEM_USER_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for related SYSTEM USER");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.HmrUserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_USR_RL_RL_FK");

                entity.HasOne(d => d.SystemUser)
                    .WithMany(p => p.HmrUserRoles)
                    .HasForeignKey(d => d.SystemUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_USR_RL_SYS_USR_FK");
            });

            modelBuilder.Entity<HmrUserRoleHist>(entity =>
            {
                entity.HasKey(e => e.UserRoleHistId)
                    .HasName("HMR_USR_R_H_PK");

                entity.ToTable("HMR_USER_ROLE_HIST");

                entity.HasIndex(e => new { e.UserRoleHistId, e.EndDateHist })
                    .HasName("HMR_USR_R_H_UK")
                    .IsUnique();

                entity.Property(e => e.UserRoleHistId)
                    .HasColumnName("USER_ROLE_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_USER_ROLE_H_ID_SEQ])");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.RoleId)
                    .HasColumnName("ROLE_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.SystemUserId)
                    .HasColumnName("SYSTEM_USER_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.UserRoleId)
                    .HasColumnName("USER_ROLE_ID")
                    .HasColumnType("numeric(18, 0)");
            });

            modelBuilder.Entity<HmrWildlifeReport>(entity =>
            {
                entity.HasKey(e => e.WildlifeRecordId)
                    .HasName("HMR_WLDLF_RRT_PK");

                entity.ToTable("HMR_WILDLIFE_REPORT");

                entity.HasComment("Submission data regarding wildlife incidents is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.");

                entity.HasIndex(e => e.ServiceArea)
                    .HasName("WLDLF_RPT_CNT_ARA_FK_I");

                entity.HasIndex(e => new { e.SubmissionObjectId, e.RowId })
                    .HasName("HMR_WLDLF_RPT_SUBM_FK_I");

                entity.Property(e => e.WildlifeRecordId)
                    .HasColumnName("WILDLIFE_RECORD_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_WLDLF_ID_SEQ])")
                    .HasComment("Unique identifier for a record");

                entity.Property(e => e.AccidentDate)
                    .HasColumnName("ACCIDENT_DATE")
                    .HasColumnType("datetime")
                    .HasComment("Date of accident. ");

                entity.Property(e => e.Age)
                    .HasColumnName("AGE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("Unique identifer for age range of involved animal.  (eg: A=Adult, Y=Young,U=unknown) ");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.Comment)
                    .HasColumnName("COMMENT")
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasComment("Text field for comments and/or notes pertinent to the specified occurance.");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.Geometry)
                    .HasColumnName("GEOMETRY")
                    .HasColumnType("geometry")
                    .HasComment("Spatial geometry where the event occured, as conformed to the road network.  ");

                entity.Property(e => e.HighwayUnique)
                    .HasColumnName("HIGHWAY_UNIQUE")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasComment("This identifies the section of road on which the incident occurred. This is a value in the in the format: [Service Area]-[area manager area]-[subarea]-[highway number] This reference number reflects a valid reference in the road network (currenltyRFI within  CHRIS as of 2019)");

                entity.Property(e => e.HighwayUniqueLength)
                    .HasColumnName("HIGHWAY_UNIQUE_LENGTH")
                    .HasColumnType("numeric(25, 20)")
                    .HasComment("Driven length in KM of the HIGHWAY_UNIQUE segment at the time of data submission.  ");

                entity.Property(e => e.HighwayUniqueName)
                    .HasColumnName("HIGHWAY_UNIQUE_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasComment("Road or Highway description sourced from a road network data product (RFI as of Dec 2019)");

                entity.Property(e => e.Landmark)
                    .HasColumnName("LANDMARK")
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasComment("Highway reference point (HRP) landmark.  This reference number reflects a valid landmark in the asset management system (currenlty CHRIS as of 2019)");

                entity.Property(e => e.Latitude)
                    .HasColumnName("LATITUDE")
                    .HasColumnType("numeric(16, 8)")
                    .HasComment("The Y (northing) portion of the accident coordinate. Coordinate is to be reported using the WGS84 datum.");

                entity.Property(e => e.Longitude)
                    .HasColumnName("LONGITUDE")
                    .HasColumnType("numeric(16, 8)")
                    .HasComment("The X (easting)  portion of the accident coordinate. Coordinate is to be reported using the WGS84 datum.");

                entity.Property(e => e.NearestTown)
                    .HasColumnName("NEAREST_TOWN")
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasComment("Name of nearest town to wildlife accident");

                entity.Property(e => e.Offset)
                    .HasColumnName("OFFSET")
                    .HasColumnType("numeric(7, 3)")
                    .HasComment("This field is needed for linear referencing for location specific reports.  Offset from beginning of segment.");

                entity.Property(e => e.Quantity)
                    .HasColumnName("QUANTITY")
                    .HasColumnType("numeric(4, 0)")
                    .HasComment("Number of animals injured or killed");

                entity.Property(e => e.RecordType)
                    .HasColumnName("RECORD_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("Identifies the type of record.  WARS = W / Allowed Values: W");

                entity.Property(e => e.RowId)
                    .HasColumnName("ROW_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for originating SUBMISSION ROW");

                entity.Property(e => e.RowNum)
                    .HasColumnName("ROW_NUM")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Relative row number the record was located within a submission.");

                entity.Property(e => e.ServiceArea)
                    .HasColumnName("SERVICE_AREA")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("The Ministry Contract Service Area number in which the incident occured.");

                entity.Property(e => e.Sex)
                    .HasColumnName("SEX")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("Unique identifer for sex of involved animal.  Allowed values: M, F, U");

                entity.Property(e => e.Species)
                    .HasColumnName("SPECIES")
                    .HasColumnType("numeric(2, 0)")
                    .HasComment("Unique identifier for animal species. (eg: 2 = Moose)");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for SUBMISSION OBJECT.");

                entity.Property(e => e.TimeOfKill)
                    .HasColumnName("TIME_OF_KILL")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("General  light conditions at time the incident occured. (eg: 1=Dawn, 2=Dusk)");

                entity.Property(e => e.ValidationStatusId)
                    .HasColumnName("VALIDATION_STATUS_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for VALIDATION STATUS.  Indicates the overall status of the submitted row of data.");

                entity.Property(e => e.WildlifeSign)
                    .HasColumnName("WILDLIFE_SIGN")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("Is Wildlife sign within 100m (Y, N or Unknown)");

                entity.HasOne(d => d.Row)
                    .WithMany(p => p.HmrWildlifeReports)
                    .HasForeignKey(d => d.RowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_WLDLF_RRT_SUBM_RW_FK");

                entity.HasOne(d => d.ServiceAreaNavigation)
                    .WithMany(p => p.HmrWildlifeReports)
                    .HasForeignKey(d => d.ServiceArea)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_WLDLF_RPT_HMR_SRV_ARA_FK");

                entity.HasOne(d => d.SubmissionObject)
                    .WithMany(p => p.HmrWildlifeReports)
                    .HasForeignKey(d => d.SubmissionObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_WLDLF_RPT_SUBM_OBJ_FK");

                entity.HasOne(d => d.ValidationStatus)
                    .WithMany(p => p.HmrWildlifeReports)
                    .HasForeignKey(d => d.ValidationStatusId)
                    .HasConstraintName("HMR_WLDLF_RRT_SUBM_STAT_FK");
            });

            modelBuilder.Entity<HmrWildlifeReportHist>(entity =>
            {
                entity.HasKey(e => e.WildlifeReportHistId)
                    .HasName("HMR_WLDLF_H_PK");

                entity.ToTable("HMR_WILDLIFE_REPORT_HIST");

                entity.HasIndex(e => new { e.WildlifeReportHistId, e.EndDateHist })
                    .HasName("HMR_WLDLF_H_UK")
                    .IsUnique();

                entity.Property(e => e.WildlifeReportHistId)
                    .HasColumnName("WILDLIFE_REPORT_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_WILDLIFE_REPORT_H_ID_SEQ])");

                entity.Property(e => e.AccidentDate)
                    .HasColumnName("ACCIDENT_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Age)
                    .HasColumnName("AGE")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Comment)
                    .HasColumnName("COMMENT")
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.Geometry)
                    .HasColumnName("GEOMETRY")
                    .HasColumnType("geometry");

                entity.Property(e => e.HighwayUnique)
                    .HasColumnName("HIGHWAY_UNIQUE")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.HighwayUniqueLength)
                    .HasColumnName("HIGHWAY_UNIQUE_LENGTH")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.HighwayUniqueName)
                    .HasColumnName("HIGHWAY_UNIQUE_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Landmark)
                    .HasColumnName("LANDMARK")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude)
                    .HasColumnName("LATITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("LONGITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.NearestTown)
                    .HasColumnName("NEAREST_TOWN")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Offset)
                    .HasColumnName("OFFSET")
                    .HasColumnType("numeric(7, 3)");

                entity.Property(e => e.Quantity)
                    .HasColumnName("QUANTITY")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.RecordType)
                    .HasColumnName("RECORD_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RowId)
                    .HasColumnName("ROW_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.RowNum)
                    .HasColumnName("ROW_NUM")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ServiceArea)
                    .HasColumnName("SERVICE_AREA")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Sex)
                    .HasColumnName("SEX")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Species)
                    .HasColumnName("SPECIES")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.TimeOfKill)
                    .HasColumnName("TIME_OF_KILL")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ValidationStatusId)
                    .HasColumnName("VALIDATION_STATUS_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.WildlifeRecordId)
                    .HasColumnName("WILDLIFE_RECORD_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.WildlifeSign)
                    .HasColumnName("WILDLIFE_SIGN")
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HmrWildlifeReportVw>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("HMR_WILDLIFE_REPORT_VW");

                entity.Property(e => e.AccidentDate)
                    .HasColumnName("ACCIDENT_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Age)
                    .HasColumnName("AGE")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateTimestampUtc)
                    .HasColumnName("APP_CREATE_TIMESTAMP_UTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateTimestampUtc)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP_UTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.Comment)
                    .HasColumnName("COMMENT")
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.FileName)
                    .HasColumnName("FILE_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Geometry)
                    .HasColumnName("GEOMETRY")
                    .HasColumnType("geometry");

                entity.Property(e => e.HighwayUnique)
                    .HasColumnName("HIGHWAY_UNIQUE")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.HighwayUniqueLength)
                    .HasColumnName("HIGHWAY_UNIQUE_LENGTH")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.HighwayUniqueName)
                    .HasColumnName("HIGHWAY_UNIQUE_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.IsOverSpThreshold)
                    .IsRequired()
                    .HasColumnName("IS_OVER_SP_THRESHOLD")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Landmark)
                    .HasColumnName("LANDMARK")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude)
                    .HasColumnName("LATITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("LONGITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.NearestTown)
                    .HasColumnName("NEAREST_TOWN")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Offset)
                    .HasColumnName("OFFSET")
                    .HasColumnType("numeric(7, 3)");

                entity.Property(e => e.Quantity)
                    .HasColumnName("QUANTITY")
                    .HasColumnType("numeric(4, 0)");

                entity.Property(e => e.RecordType)
                    .HasColumnName("RECORD_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ReportType)
                    .IsRequired()
                    .HasColumnName("REPORT_TYPE")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.RowNum)
                    .HasColumnName("ROW_NUM")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.ServiceArea)
                    .HasColumnName("SERVICE_AREA")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Sex)
                    .HasColumnName("SEX")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SpatialVariance)
                    .HasColumnName("SPATIAL_VARIANCE")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.Species)
                    .HasColumnName("SPECIES")
                    .HasColumnType("numeric(2, 0)");

                entity.Property(e => e.SubmissionDate)
                    .HasColumnName("SUBMISSION_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.TimeOfKill)
                    .HasColumnName("TIME_OF_KILL")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ValidationStatus)
                    .HasColumnName("VALIDATION_STATUS")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.WarningSpThreshold)
                    .HasColumnName("WARNING_SP_THRESHOLD")
                    .HasColumnType("numeric(12, 6)");

                entity.Property(e => e.WildlifeRecordId)
                    .HasColumnName("WILDLIFE_RECORD_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.WildlifeSign)
                    .HasColumnName("WILDLIFE_SIGN")
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HmrWorkReport>(entity =>
            {
                entity.HasKey(e => e.WorkReportId)
                    .HasName("HMR_WRK_RPT_PK");

                entity.ToTable("HMR_WORK_REPORT");

                entity.HasComment("Submission data regarding maintenance activities is ultimately staged in this table after being loaded and validated.  Validation status of the data is also provided here, as it may be desirable for some invalid data to be available and marked accordingly.");

                entity.HasIndex(e => new { e.SubmissionObjectId, e.RowId })
                    .HasName("HMR_WRK_RRT_FK_I");

                entity.Property(e => e.WorkReportId)
                    .HasColumnName("WORK_REPORT_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_WRK_RPT_ID_SEQ])")
                    .HasComment("A system generated unique identifier.");

                entity.Property(e => e.Accomplishment)
                    .HasColumnName("ACCOMPLISHMENT")
                    .HasColumnType("numeric(9, 2)")
                    .HasComment("The number of units of work completed for the activity corresponding to the activity number.");

                entity.Property(e => e.ActivityNumber)
                    .HasColumnName("ACTIVITY_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Code which uniquely identifies the activity performed.  The number reflects a a classificaiton hierarchy comprised of three levels: ABBCCC  A - the first digit represents Specification Category (eg:2 - Drainage ) BB - the second two digits represent Activity Category (eg: 02 - Drainage Appliance Maintenance) CCC - the last three digits represent Activity Type and Detail (eg: 310 - Boring, Augering.  300 series reflects Quantified value, which would be linear meters in this case.)");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.Comments)
                    .HasColumnName("COMMENTS")
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasComment("Text field for comments and/or notes pertinent to the specified activity.");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who last updated record");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("date")
                    .HasComment("Date when work was completed");

                entity.Property(e => e.EndLatitude)
                    .HasColumnName("END_LATITUDE")
                    .HasColumnType("numeric(16, 8)")
                    .HasComment("The M (northing) portion of the activity end coordinate. Specified as a latitude in decimal degrees with six decimal places of precision. Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum. For point activity if this field is not provided it can be defaulted to same as START LATITUDE");

                entity.Property(e => e.EndLongitude)
                    .HasColumnName("END_LONGITUDE")
                    .HasColumnType("numeric(16, 8)")
                    .HasComment("The X (easting) portion of the activity end coordinate. Specified as a longitude in decimal degrees with six decimal places of precision. Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum. For point activity if this field is not provided it can be defaulted to same as START LONGITUDE.");

                entity.Property(e => e.EndOffset)
                    .HasColumnName("END_OFFSET")
                    .HasColumnType("numeric(7, 3)")
                    .HasComment("This field is needed for linear referencing for location specific reports. If the work is less than 30 m, this field is not mandatory Offset from beginning of segment");

                entity.Property(e => e.Geometry)
                    .HasColumnName("GEOMETRY")
                    .HasColumnType("geometry")
                    .HasComment("Spatial geometry where the activity occured, as conformed to the road network.   Provided start and end coordinates are used to derive the best-fit road segment.");

                entity.Property(e => e.HighwayUnique)
                    .HasColumnName("HIGHWAY_UNIQUE")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasComment("This identifies the section of road on which the activity occurred. This is a value in the in the format: [Service Area]-[area manager area]-[subarea]-[highway number] This should be a value found in RFI (CHRIS)");

                entity.Property(e => e.HighwayUniqueLength)
                    .HasColumnName("HIGHWAY_UNIQUE_LENGTH")
                    .HasColumnType("numeric(25, 20)")
                    .HasComment("Driven length in KM of the HIGHWAY_UNIQUE segment at the time of data submission.  ");

                entity.Property(e => e.HighwayUniqueName)
                    .HasColumnName("HIGHWAY_UNIQUE_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasComment("Road or Highway description sourced from a road network data product (Road Feature Inventory [RFI] as of Dec 2019).  The name is derived from the HIGHWAY_UNIQUE value provided wtihin the submission.");

                entity.Property(e => e.Landmark)
                    .HasColumnName("LANDMARK")
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasComment("This field needed for location reference: Landmarks provided should be those listed in the CHRIS HRP report for each Highway or road within the Service Area");

                entity.Property(e => e.PostedDate)
                    .HasColumnName("POSTED_DATE")
                    .HasColumnType("date")
                    .HasComment("Date the data is posted into the contractor management system");

                entity.Property(e => e.RecordNumber)
                    .HasColumnName("RECORD_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique work report record number from the maintainence contractor. This is uniquely identifies each record submission for a contractor. <Service Area><Record Number> will uniquely identify each record in the application for a particular contractor.");

                entity.Property(e => e.RecordType)
                    .HasColumnName("RECORD_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("This field describes the type of work the associated record is reporting on. This is restricted to specific set of values - Q - Quantified, R - Routine, E - Major Event, A - Additional");

                entity.Property(e => e.RecordVersionNumber)
                    .HasColumnName("RECORD_VERSION_NUMBER")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RowId)
                    .HasColumnName("ROW_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for originating SUBMISSION ROW.");

                entity.Property(e => e.RowNum)
                    .HasColumnName("ROW_NUM")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Relative row number the record was located within a submission.");

                entity.Property(e => e.ServiceArea)
                    .HasColumnName("SERVICE_AREA")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("The Ministry Contract Service Area number in which the activity occured.");

                entity.Property(e => e.SiteNumber)
                    .HasColumnName("SITE_NUMBER")
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasComment("Contains a site type code followed by a The Ministry site number. Site types are provided by the Province, are four to six digits preceded by: A – Avalanche B – Arrestor Bed/Dragnet Barrier D – Debris and/or Rockfall L – Landscape R – Rest Area S – Signalized Intersection T – Traffic Patrol W – Weather Station X – Railway Crossing");

                entity.Property(e => e.StartDate)
                    .HasColumnName("START_DATE")
                    .HasColumnType("date")
                    .HasComment("Date when work commenced");

                entity.Property(e => e.StartLatitude)
                    .HasColumnName("START_LATITUDE")
                    .HasColumnType("numeric(16, 8)")
                    .HasComment("The M (northing) portion of the activity start coordinate. Specified as a latitude in decimal degrees with six decimal places of precision. Positive numbers are indicative of the Northern Hemisphere. Coordinate is to be reported using the WGS84 datum.");

                entity.Property(e => e.StartLongitude)
                    .HasColumnName("START_LONGITUDE")
                    .HasColumnType("numeric(16, 8)")
                    .HasComment("The X (easting) portion of the activity start coordinate. Specified as a longitude in decimal degrees with six decimal places of precision. Negative numbers are indicative of the Western Hemisphere. Coordinate is to be reported using the WGS84 datum.");

                entity.Property(e => e.StartOffset)
                    .HasColumnName("START_OFFSET")
                    .HasColumnType("numeric(7, 3)")
                    .HasComment("This field is needed for linear referencing for location specific reports.  Offset from beginning of segment.");

                entity.Property(e => e.StructureNumber)
                    .HasColumnName("STRUCTURE_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("From list of Bridge Structure Road (BSR) structures provided by the Province. Is only applicable at defined BSR structures.  BSR structures include; bridges, culverts over 3m, retaining walls.");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for SUBMISSION OBJECT.");

                entity.Property(e => e.TaskNumber)
                    .HasColumnName("TASK_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Contractor Task Number");

                entity.Property(e => e.UnitOfMeasure)
                    .HasColumnName("UNIT_OF_MEASURE")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("The code which represents the unit of measure for the specified activity. ");

                entity.Property(e => e.ValidationStatusId)
                    .HasColumnName("VALIDATION_STATUS_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for VALIDATION STATUS.  Indicates the overall status of the submitted row of data.");

                entity.Property(e => e.ValueOfWork)
                    .HasColumnName("VALUE_OF_WORK")
                    .HasColumnType("numeric(9, 2)")
                    .HasComment("Total dollar value of the work activity being reported, for each activity.");

                entity.Property(e => e.WorkLength)
                    .HasColumnName("WORK_LENGTH")
                    .HasColumnType("numeric(25, 20)")
                    .HasComment("Driven length in KM of the work distance as calculated from start and end coordinates and related HIGHWAY_UNIQUE segment at the time of data submission.");

                entity.HasOne(d => d.Row)
                    .WithMany(p => p.HmrWorkReports)
                    .HasForeignKey(d => d.RowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_WRK_RRT_SUBM_RW_FK");

                entity.HasOne(d => d.ServiceAreaNavigation)
                    .WithMany(p => p.HmrWorkReports)
                    .HasForeignKey(d => d.ServiceArea)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_WRK_RRT_SRV_ARA_FK");

                entity.HasOne(d => d.SubmissionObject)
                    .WithMany(p => p.HmrWorkReports)
                    .HasForeignKey(d => d.SubmissionObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_WRK_RRT_SUBM_OBJ_FK");

                entity.HasOne(d => d.ValidationStatus)
                    .WithMany(p => p.HmrWorkReports)
                    .HasForeignKey(d => d.ValidationStatusId)
                    .HasConstraintName("HMR_WRK_RRT_SUBM_STAT_FK");
            });

            modelBuilder.Entity<HmrWorkReportHist>(entity =>
            {
                entity.HasKey(e => e.WorkReportHistId)
                    .HasName("HMR_WRK_R_H_PK");

                entity.ToTable("HMR_WORK_REPORT_HIST");

                entity.HasIndex(e => new { e.WorkReportHistId, e.EndDateHist })
                    .HasName("HMR_WRK_R_H_UK")
                    .IsUnique();

                entity.Property(e => e.WorkReportHistId)
                    .HasColumnName("WORK_REPORT_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_WORK_REPORT_H_ID_SEQ])");

                entity.Property(e => e.Accomplishment)
                    .HasColumnName("ACCOMPLISHMENT")
                    .HasColumnType("numeric(9, 2)");

                entity.Property(e => e.ActivityNumber)
                    .HasColumnName("ACTIVITY_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateUserGuid).HasColumnName("APP_CREATE_USER_GUID");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.AppLastUpdateUserGuid).HasColumnName("APP_LAST_UPDATE_USER_GUID");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Comments)
                    .HasColumnName("COMMENTS")
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.ConcurrencyControlNumber).HasColumnName("CONCURRENCY_CONTROL_NUMBER");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateHist)
                    .HasColumnName("EFFECTIVE_DATE_HIST")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.EndDateHist)
                    .HasColumnName("END_DATE_HIST")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndLatitude)
                    .HasColumnName("END_LATITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.EndLongitude)
                    .HasColumnName("END_LONGITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.EndOffset)
                    .HasColumnName("END_OFFSET")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.Geometry)
                    .HasColumnName("GEOMETRY")
                    .HasColumnType("geometry");

                entity.Property(e => e.HighwayUnique)
                    .HasColumnName("HIGHWAY_UNIQUE")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.HighwayUniqueLength)
                    .HasColumnName("HIGHWAY_UNIQUE_LENGTH")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.HighwayUniqueName)
                    .HasColumnName("HIGHWAY_UNIQUE_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Landmark)
                    .HasColumnName("LANDMARK")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.PostedDate)
                    .HasColumnName("POSTED_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.RecordNumber)
                    .HasColumnName("RECORD_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.RecordType)
                    .HasColumnName("RECORD_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RecordVersionNumber).HasColumnName("RECORD_VERSION_NUMBER");

                entity.Property(e => e.RowId)
                    .HasColumnName("ROW_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.RowNum)
                    .HasColumnName("ROW_NUM")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ServiceArea)
                    .HasColumnName("SERVICE_AREA")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.SiteNumber)
                    .HasColumnName("SITE_NUMBER")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate)
                    .HasColumnName("START_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.StartLatitude)
                    .HasColumnName("START_LATITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.StartLongitude)
                    .HasColumnName("START_LONGITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.StartOffset)
                    .HasColumnName("START_OFFSET")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.StructureNumber)
                    .HasColumnName("STRUCTURE_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.TaskNumber)
                    .HasColumnName("TASK_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UnitOfMeasure)
                    .HasColumnName("UNIT_OF_MEASURE")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.ValidationStatusId)
                    .HasColumnName("VALIDATION_STATUS_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ValueOfWork)
                    .HasColumnName("VALUE_OF_WORK")
                    .HasColumnType("numeric(9, 2)");

                entity.Property(e => e.WorkLength)
                    .HasColumnName("WORK_LENGTH")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.WorkReportId)
                    .HasColumnName("WORK_REPORT_ID")
                    .HasColumnType("numeric(18, 0)");
            });

            modelBuilder.Entity<HmrWorkReportVw>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("HMR_WORK_REPORT_VW");

                entity.Property(e => e.Accomplishment)
                    .HasColumnName("ACCOMPLISHMENT")
                    .HasColumnType("numeric(9, 2)");

                entity.Property(e => e.ActivityName)
                    .HasColumnName("ACTIVITY_NAME")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ActivityNumber)
                    .HasColumnName("ACTIVITY_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppCreateTimestampUtc)
                    .HasColumnName("APP_CREATE_TIMESTAMP_UTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppLastUpdateTimestampUtc)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP_UTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.Comments)
                    .HasColumnName("COMMENTS")
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.DataPrecisionValidWarning)
                    .HasColumnName("DATA_PRECISION_VALID_WARNING")
                    .HasMaxLength(4000);

                entity.Property(e => e.EndDate)
                    .HasColumnName("END_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.EndLatitude)
                    .HasColumnName("END_LATITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.EndLongitude)
                    .HasColumnName("END_LONGITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.EndOffset)
                    .HasColumnName("END_OFFSET")
                    .HasColumnType("numeric(7, 3)");

                entity.Property(e => e.EndVariance)
                    .HasColumnName("END_VARIANCE")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.FileName)
                    .HasColumnName("FILE_NAME")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Geometry)
                    .HasColumnName("GEOMETRY")
                    .HasColumnType("geometry");

                entity.Property(e => e.HighwayUnique)
                    .HasColumnName("HIGHWAY_UNIQUE")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.HighwayUniqueLength)
                    .HasColumnName("HIGHWAY_UNIQUE_LENGTH")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.HighwayUniqueName)
                    .HasColumnName("HIGHWAY_UNIQUE_NAME")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.IsOverSpThreshold)
                    .IsRequired()
                    .HasColumnName("IS_OVER_SP_THRESHOLD")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Landmark)
                    .HasColumnName("LANDMARK")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MinMaxValueValidWarning)
                    .HasColumnName("MIN_MAX_VALUE_VALID_WARNING")
                    .HasMaxLength(4000);

                entity.Property(e => e.PostedDate)
                    .HasColumnName("POSTED_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.RecordNumber)
                    .HasColumnName("RECORD_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.RecordType)
                    .HasColumnName("RECORD_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RecordVersionNumber).HasColumnName("RECORD_VERSION_NUMBER");

                entity.Property(e => e.ReportType)
                    .IsRequired()
                    .HasColumnName("REPORT_TYPE")
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.ReportingFreqValidWarning)
                    .HasColumnName("REPORTING_FREQ_VALID_WARNING")
                    .HasMaxLength(4000);

                entity.Property(e => e.RoadClassValidWarning)
                    .HasColumnName("ROAD_CLASS_VALID_WARNING")
                    .HasMaxLength(4000);

                entity.Property(e => e.RoadLengthValidWarning)
                    .HasColumnName("ROAD_LENGTH_VALID_WARNING")
                    .HasMaxLength(4000);

                entity.Property(e => e.RowNum)
                    .HasColumnName("ROW_NUM")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.ServiceArea)
                    .HasColumnName("SERVICE_AREA")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.SiteNumber)
                    .HasColumnName("SITE_NUMBER")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate)
                    .HasColumnName("START_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.StartLatitude)
                    .HasColumnName("START_LATITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.StartLongitude)
                    .HasColumnName("START_LONGITUDE")
                    .HasColumnType("numeric(16, 8)");

                entity.Property(e => e.StartOffset)
                    .HasColumnName("START_OFFSET")
                    .HasColumnType("numeric(7, 3)");

                entity.Property(e => e.StartVariance)
                    .HasColumnName("START_VARIANCE")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.StructureNumber)
                    .HasColumnName("STRUCTURE_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SubmissionDate)
                    .HasColumnName("SUBMISSION_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.SubmissionObjectId)
                    .HasColumnName("SUBMISSION_OBJECT_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.SurfaceTypeValidWarning)
                    .HasColumnName("SURFACE_TYPE_VALID_WARNING")
                    .HasMaxLength(4000);

                entity.Property(e => e.TaskNumber)
                    .HasColumnName("TASK_NUMBER")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UnitOfMeasure)
                    .HasColumnName("UNIT_OF_MEASURE")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.ValidationStatus)
                    .HasColumnName("VALIDATION_STATUS")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ValueOfWork)
                    .HasColumnName("VALUE_OF_WORK")
                    .HasColumnType("numeric(9, 2)");

                entity.Property(e => e.WarningSpThreshold)
                    .HasColumnName("WARNING_SP_THRESHOLD")
                    .HasColumnType("numeric(12, 6)");

                entity.Property(e => e.WorkLength)
                    .HasColumnName("WORK_LENGTH")
                    .HasColumnType("numeric(25, 20)");

                entity.Property(e => e.WorkReportId)
                    .HasColumnName("WORK_REPORT_ID")
                    .HasColumnType("numeric(9, 0)");
            });

            modelBuilder.Entity<HmrSaltReport>(entity =>
            {
                entity.ToTable("HMR_SALT_REPORT");

                entity.HasMany(e => e.Stockpiles)
                    .WithOne(e => e.SaltReport)
                    .HasForeignKey(e => e.SaltReportId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.VulnerableAreas)
                    .WithOne(e => e.SaltReport)
                    .HasForeignKey(e => e.SaltReportId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Appendix)
                    .WithOne(e => e.SaltReport)
                    .HasForeignKey<HmrSaltReportAppendix>(e => e.SaltReportId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasKey(e => e.SaltReportId)
                    .HasName("HMR_SLTRPT_PK");

                entity.Property(e => e.SaltReportId)
                    .HasColumnName("SALT_REPORT_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SLT_RPT_ID_SEQ])")
                    .HasComment("A system generated unique identifier.");

                entity.Property(e => e.ServiceArea)
                    .HasColumnName("SERVICE_AREA")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for SERVICE AREA");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");
            });

            modelBuilder.Entity<HmrSaltStockpile>(entity =>
            {
                entity.ToTable("HMR_SALT_STOCKPILE");

                entity.HasKey(e => e.StockPileId)
                    .HasName("HMR_SLTSTP_PK");

                entity.Property(e => e.StockPileId)
                    .HasColumnName("STOCKPILE_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SLT_STOCKPILE_ID_SEQ])")
                    .HasComment("A system generated unique identifier.");

                entity.Property(e => e.SaltReportId)
                    .HasColumnName("SALT_REPORT_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");
            });

            modelBuilder.Entity<HmrSaltReportAppendix>(entity =>
            {
                entity.ToTable("HMR_SALT_APPENDIX");

                entity.HasKey(e => e.AppendixId)
                    .HasName("HMR_SLTAPP_PK");

                entity.Property(e => e.AppendixId)
                    .HasColumnName("APPENDIX_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SLT_APPENDIX_ID_SEQ])");

                entity.Property(e => e.SaltReportId)
                    .HasColumnName("SALT_REPORT_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");
            });

            modelBuilder.Entity<HmrSaltVulnArea>(entity =>
            {
                entity.ToTable("HMR_SALT_VULNAREA");

                entity.HasKey(e => e.VulnerableAreaId)
                    .HasName("HMR_SLTVUL_PK");

                entity.Property(e => e.VulnerableAreaId)
                    .HasColumnName("VULNAREA_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SLT_VULNAREA_ID_SEQ])")
                    .HasComment("A system generated unique identifier.");

                entity.Property(e => e.SaltReportId)
                    .HasColumnName("SALT_REPORT_ID")
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.AppCreateTimestamp)
                    .HasColumnName("APP_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of record creation");

                entity.Property(e => e.AppCreateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppCreateUserGuid)
                    .HasColumnName("APP_CREATE_USER_GUID")
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppCreateUserid)
                    .IsRequired()
                    .HasColumnName("APP_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who created record");

                entity.Property(e => e.AppLastUpdateTimestamp)
                    .HasColumnName("APP_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time of last record update");

                entity.Property(e => e.AppLastUpdateUserDirectory)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USER_DIRECTORY")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("Active Directory which retains source of truth for user idenifiers.");

                entity.Property(e => e.AppLastUpdateUserGuid)
                    .HasColumnName("APP_LAST_UPDATE_USER_GUID")
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.AppLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("APP_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Unique idenifier of user who last updated record");

                entity.Property(e => e.ConcurrencyControlNumber)
                    .HasColumnName("CONCURRENCY_CONTROL_NUMBER")
                    .HasDefaultValueSql("((1))")
                    .HasComment("Record under edit indicator used for optomisitc record contention management.  If number differs from start of edit, then user will be prompted to that record has been updated by someone else.");

                entity.Property(e => e.DbAuditCreateTimestamp)
                    .HasColumnName("DB_AUDIT_CREATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())")
                    .HasComment("Named database user who created record");
            });

            modelBuilder.HasSequence("FDBK_MSG_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_ACT_CODE_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_ACTIVITY_CODE_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_ACTIVITY_CODE_RULE_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_CNT_TRM_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_CODE_LKUP_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_CODE_LOOKUP_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_CONTRACT_TERM_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_DIST_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_LOC_CODE_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_LOCATION_CODE_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_MIME_TYPE_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_PARTY_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_PERM_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_PERMISSION_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_PRTY_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_RCKF_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_REG_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_RL_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_RL_PERM_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_ROCKFALL_REPORT_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_ROLE_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_ROLE_PERMISSION_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_SERVICE_AREA_ACTIVITY_H_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_SERVICE_AREA_ACTIVITY_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_SERVICE_AREA_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_SERVICE_AREA_USER_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_SRV_ARA_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_SRV_AREA_USR_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_STR_ELMT_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_STREAM_ELEMENT_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_SUBM_OBJ_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_SUBM_RW_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_SUBM_STAT_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_SUBM_STR_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_SUBMISSION_ROW_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_SUBMISSION_STREAM_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_SYSTEM_USER_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_USER_ROLE_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_USR_RL_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_WILDLIFE_REPORT_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_WLDLF_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_WORK_REPORT_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_WRK_RPT_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("SYS_USR_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("SYS_VLD_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            OnModelCreatingPartial(modelBuilder);
        }

        public class BlankTriggerAddingConvention : IModelFinalizingConvention
        {
            public virtual void ProcessModelFinalizing(
                IConventionModelBuilder modelBuilder,
                IConventionContext<IConventionModelBuilder> context)
            {
                foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
                {
                    var table = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);
                    if (table != null
                        && entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(table.Value) == null))
                    {
                        entityType.Builder.HasTrigger(table.Value.Name + "_Trigger");
                    }

                    foreach (var fragment in entityType.GetMappingFragments(StoreObjectType.Table))
                    {
                        if (entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(fragment.StoreObject) == null))
                        {
                            entityType.Builder.HasTrigger(fragment.StoreObject.Name + "_Trigger");
                        }
                    }
                }
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
