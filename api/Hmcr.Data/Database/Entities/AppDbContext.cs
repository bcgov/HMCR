using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public virtual DbSet<HmrContractServiceArea> HmrContractServiceAreas { get; set; }
        public virtual DbSet<HmrContractServiceAreaHist> HmrContractServiceAreaHists { get; set; }
        public virtual DbSet<HmrContractTerm> HmrContractTerms { get; set; }
        public virtual DbSet<HmrContractTermHist> HmrContractTermHists { get; set; }
        public virtual DbSet<HmrDistrict> HmrDistricts { get; set; }
        public virtual DbSet<HmrMimeType> HmrMimeTypes { get; set; }
        public virtual DbSet<HmrParty> HmrParties { get; set; }
        public virtual DbSet<HmrPartyHist> HmrPartyHists { get; set; }
        public virtual DbSet<HmrPermission> HmrPermissions { get; set; }
        public virtual DbSet<HmrPermissionHist> HmrPermissionHists { get; set; }
        public virtual DbSet<HmrRegion> HmrRegions { get; set; }
        public virtual DbSet<HmrRole> HmrRoles { get; set; }
        public virtual DbSet<HmrRoleHist> HmrRoleHists { get; set; }
        public virtual DbSet<HmrRolePermission> HmrRolePermissions { get; set; }
        public virtual DbSet<HmrRolePermissionHist> HmrRolePermissionHists { get; set; }
        public virtual DbSet<HmrServiceArea> HmrServiceAreas { get; set; }
        public virtual DbSet<HmrServiceAreaHist> HmrServiceAreaHists { get; set; }
        public virtual DbSet<HmrServiceAreaUser> HmrServiceAreaUsers { get; set; }
        public virtual DbSet<HmrServiceAreaUserHist> HmrServiceAreaUserHists { get; set; }
        public virtual DbSet<HmrSubmissionObject> HmrSubmissionObjects { get; set; }
        public virtual DbSet<HmrSubmissionStatu> HmrSubmissionStatus { get; set; }
        public virtual DbSet<HmrSystemUser> HmrSystemUsers { get; set; }
        public virtual DbSet<HmrSystemUserHist> HmrSystemUserHists { get; set; }
        public virtual DbSet<HmrUserRole> HmrUserRoles { get; set; }
        public virtual DbSet<HmrUserRoleHist> HmrUserRoleHists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HmrContractServiceArea>(entity =>
            {
                entity.HasKey(e => e.ContractServiceAreaId)
                    .HasName("HMR_CNRT_SRV_ARA_PK");

                entity.ToTable("HMR_CONTRACT_SERVICE_AREA");

                entity.HasComment("SERVICE AREAs that are being administered under a contract term.  This table enables the confirmation of unique task IDs assigned by vendors for the term of their contract.");

                entity.HasIndex(e => new { e.ContractTermId, e.ServiceAreaNumber, e.EndDate })
                    .HasName("HMR_CNRT_SRV_ARA_UN_CH")
                    .IsUnique();

                entity.Property(e => e.ContractServiceAreaId)
                    .HasColumnName("CONTRACT_SERVICE_AREA_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_CNT_SRV_ARA_ID_SEQ])")
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

                entity.Property(e => e.ContractTermId)
                    .HasColumnName("CONTRACT_TERM_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique idenifier for related contract term");

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
                    .HasComment("Latest date a contract term was associated with a SERVICE AREA.");

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Assigned number of the Service Area");

                entity.HasOne(d => d.ContractTerm)
                    .WithMany(p => p.HmrContractServiceAreas)
                    .HasForeignKey(d => d.ContractTermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_CNRT_SRV_ARA_CNRT_TRM_FK");

                entity.HasOne(d => d.ServiceAreaNumberNavigation)
                    .WithMany(p => p.HmrContractServiceAreas)
                    .HasForeignKey(d => d.ServiceAreaNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_CNRT_SRV_ARA_SRV_ARA_FK");
            });

            modelBuilder.Entity<HmrContractServiceAreaHist>(entity =>
            {
                entity.HasKey(e => e.ContractServiceAreaHistId)
                    .HasName("HMR_CSA1_H_PK");

                entity.ToTable("HMR_CONTRACT_SERVICE_AREA_HIST");

                entity.HasIndex(e => new { e.ContractServiceAreaHistId, e.EndDateHist })
                    .HasName("HMR_CSA1_H_UK")
                    .IsUnique();

                entity.Property(e => e.ContractServiceAreaHistId)
                    .HasColumnName("CONTRACT_SERVICE_AREA_HIST_ID")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_CONTRACT_SERVICE_AREA_H_ID_SEQ])");

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

                entity.Property(e => e.ContractServiceAreaId)
                    .HasColumnName("CONTRACT_SERVICE_AREA_ID")
                    .HasColumnType("numeric(18, 0)");

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

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(18, 0)");
            });

            modelBuilder.Entity<HmrContractTerm>(entity =>
            {
                entity.HasKey(e => e.ContractTermId)
                    .HasName("HMR_CNRT_TRM_PK");

                entity.ToTable("HMR_CONTRACT_TERM");

                entity.HasComment("Identifies a unique contract term for each party and the service areas those organizations are obligated to provide services for. This table enables the confirmation of unique task IDs assigned by vendors for the term of their contract.");

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

                entity.Property(e => e.StartDate)
                    .HasColumnName("START_DATE")
                    .HasColumnType("datetime")
                    .HasComment("Earliest date a contract term was in effect.");

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.HmrContractTerms)
                    .HasForeignKey(d => d.PartyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_CNRT_TRM_PRTY_FK");
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
                    .IsRequired()
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
                    .IsRequired()
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
                    .HasColumnName("REGION_NAME")
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasComment("Name of the Ministry region");
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

                entity.HasComment("Service Area lookup values ");

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

                entity.Property(e => e.ServiceAreaId)
                    .HasColumnName("SERVICE_AREA_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [HMR_SRV_ARA_ID_SEQ])")
                    .HasComment("Unique idenifier for table records");

                entity.Property(e => e.ServiceAreaName)
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

                entity.Property(e => e.ServiceAreaId)
                    .HasColumnName("SERVICE_AREA_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ServiceAreaName)
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
                    .HasName("HMR_SRV_A_USR_H_PK");

                entity.ToTable("HMR_SERVICE_AREA_USER_HIST");

                entity.HasIndex(e => new { e.ServiceAreaUserHistId, e.EndDateHist })
                    .HasName("HMR_SRV_A_USR_H_UK")
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

            modelBuilder.Entity<HmrSubmissionObject>(entity =>
            {
                entity.HasKey(e => e.SubmissionObjectId)
                    .HasName("HMR_SUBM_OBJ_PK");

                entity.ToTable("HMR_SUBMISSION_OBJECT");

                entity.HasComment("Digital file containing a batch of records being submitted for validation,  ingestion and reporting.");

                entity.HasIndex(e => new { e.SubmissionStatusId, e.ServiceAreaNumber })
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
                    .HasColumnType("image")
                    .HasComment("Raw file storage within the database.");

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
                    .HasColumnType("numeric(9, 0)");

                entity.Property(e => e.ServiceAreaNumber)
                    .HasColumnName("SERVICE_AREA_NUMBER")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier for SERVICE AREA");

                entity.Property(e => e.SubmissionStatusId)
                    .HasColumnName("SUBMISSION_STATUS_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasComment("Unique identifier relecting the current status of the submission.");

                entity.HasOne(d => d.MimeType)
                    .WithMany(p => p.HmrSubmissionObjects)
                    .HasForeignKey(d => d.MimeTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HRM_SUBM_OBJ_MIME_TYPE_FK");

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.HmrSubmissionObjects)
                    .HasForeignKey(d => d.PartyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HMR_SUBMISSION_OBJECT_HMR_PARTY_FK");

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
            });

            modelBuilder.Entity<HmrSubmissionStatu>(entity =>
            {
                entity.HasKey(e => e.SubmissionStatusId)
                    .HasName("HMR_SUBMISSION_STATUS_CODE_PK");

                entity.ToTable("HMR_SUBMISSION_STATUS");

                entity.HasComment("Indicates the statues a SUBMISSION_OBJECT can be assigned during ingestion (ie:  Received, Invalid, Valid)");

                entity.Property(e => e.SubmissionStatusId)
                    .HasColumnName("SUBMISSION_STATUS_ID")
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

                entity.Property(e => e.SubmissionStatusCode)
                    .IsRequired()
                    .HasColumnName("SUBMISSION_STATUS_CODE")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("Describes the file processing status.");
            });

            modelBuilder.Entity<HmrSystemUser>(entity =>
            {
                entity.HasKey(e => e.SystemUserId)
                    .HasName("HMR_SYSTEM_USER_PK");

                entity.ToTable("HMR_SYSTEM_USER");

                entity.HasComment("Defines users and their attributes as found in IDIR or BCeID.");

                entity.HasIndex(e => new { e.PartyId, e.UserType })
                    .HasName("HMR_SYSTEM_USER_FK_I");

                entity.Property(e => e.SystemUserId)
                    .HasColumnName("SYSTEM_USER_ID")
                    .HasColumnType("numeric(9, 0)")
                    .HasDefaultValueSql("(NEXT VALUE FOR [SYS_USR_ID_SEQ])")
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

                entity.Property(e => e.BusinessGuid)
                    .HasColumnName("BUSINESS_GUID")
                    .HasComment("A system generated unique identifier.  Reflects the active directory unique idenifier for the business associated with the user.");

                entity.Property(e => e.BusinessLegalName)
                    .HasColumnName("BUSINESS_LEGAL_NAME")
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasComment(@"Lega lName assigned to the business and derived from BC Registry via BCeID (SMGOV_BUSINESSLEGALNAME)
");

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
                    .IsRequired()
                    .HasColumnName("USER_DIRECTORY")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Directory (IDIR / BCeID/Oracle) in which the userid is defined.");

                entity.Property(e => e.UserGuid)
                    .HasColumnName("USER_GUID")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("A system generated unique identifier.  Reflects the active directory unique idenifier for the user.");

                entity.Property(e => e.UserType)
                    .IsRequired()
                    .HasColumnName("USER_TYPE")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment(@"Defined attribute within IDIR Active directory (UserType = SMGOV_USERTYPE)
");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("USERNAME")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasComment("IDIR or BCeID Active Directory defined universal identifier (SM_UNIVERSALID or userID) attributed to a user.  This value can change over time, while USER_GUID will remain consistant.");

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.HmrSystemUsers)
                    .HasForeignKey(d => d.PartyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
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
                    .IsRequired()
                    .HasColumnName("USER_DIRECTORY")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UserGuid)
                    .HasColumnName("USER_GUID")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.UserType)
                    .IsRequired()
                    .HasColumnName("USER_TYPE")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("USERNAME")
                    .HasMaxLength(32)
                    .IsUnicode(false);
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
                    .HasComment("Date and time record created in the database");

                entity.Property(e => e.DbAuditCreateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_CREATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComment("Named database user who created record");

                entity.Property(e => e.DbAuditLastUpdateTimestamp)
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_TIMESTAMP")
                    .HasColumnType("datetime")
                    .HasComment("Date and time record was last updated in the database.");

                entity.Property(e => e.DbAuditLastUpdateUserid)
                    .IsRequired()
                    .HasColumnName("DB_AUDIT_LAST_UPDATE_USERID")
                    .HasMaxLength(30)
                    .IsUnicode(false)
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

            modelBuilder.HasSequence("HMR_CNT_SRV_ARA_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_CNT_TRM_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_CONTRACT_SERVICE_AREA_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_CONTRACT_TERM_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_DIST_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

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

            modelBuilder.HasSequence("HMR_REG_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_RL_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_RL_PERM_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_ROLE_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_ROLE_PERMISSION_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

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

            modelBuilder.HasSequence("HMR_SUBM_OBJ_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_SUBM_STAT_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("HMR_SYSTEM_USER_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_USER_ROLE_H_ID_SEQ")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("HMR_USR_RL_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            modelBuilder.HasSequence("SYS_USR_ID_SEQ")
                .HasMin(1)
                .HasMax(999999999);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
