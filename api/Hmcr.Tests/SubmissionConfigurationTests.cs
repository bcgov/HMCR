using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionConfiguration;
using Hmcr.Model.Dtos.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Hmcr.Tests;

public class SubmissionConfigurationWindowServiceTests
{
    private readonly SubmissionConfigurationWindowService _service = new();

    [Theory]
    [InlineData("2027-01-01", SubmissionConfigurationValues.ReminderState)]
    [InlineData("2027-01-31", SubmissionConfigurationValues.ReminderState)]
    [InlineData("2027-02-01", SubmissionConfigurationValues.BlackoutState)]
    [InlineData("2027-03-31", SubmissionConfigurationValues.BlackoutState)]
    [InlineData("2027-04-01", SubmissionConfigurationValues.OutsideWindowState)]
    [InlineData("2026-02-01", SubmissionConfigurationValues.OutsideWindowState)]
    public void GetState_UsesInclusiveAnnualWindowsAndEffectiveDate(string date, string expected)
    {
        var result = _service.GetState(TestConfiguration(), DateOnly.Parse(date));

        Assert.Equal(expected, result.State);
    }

    [Fact]
    public void GetState_InactiveConfigurationOverridesAnActiveWindow()
    {
        var configuration = TestConfiguration();
        configuration.IsActive = false;

        var result = _service.GetState(configuration, new DateOnly(2027, 2, 1));

        Assert.Equal(SubmissionConfigurationValues.InactiveState, result.State);
        Assert.Null(result.Occurrence);
    }

    [Fact]
    public void GetNextBlackout_AfterCurrentWindowReturnsNextYear()
    {
        var result = _service.GetNextBlackout(TestConfiguration(), new DateOnly(2027, 4, 1));

        Assert.Equal(new DateOnly(2028, 2, 1), result.StartDate);
        Assert.Equal(new DateOnly(2028, 3, 31), result.EndDate);
    }

    internal static HmrSubmissionConfiguration TestConfiguration()
    {
        var configuration = new HmrSubmissionConfiguration
        {
            SubmissionConfigurationId = 1,
            ConfigurationKey = "CAPITALIZED_HARD_SURFACING",
            ConfigurationName = "Capitalized Hard Surfacing Works",
            SubmissionStreamId = 1,
            IsActive = true,
            EffectiveFromDate = new DateTime(2027, 1, 1),
            TimeZoneId = Constants.VancouverTimeZone,
            ScopeType = SubmissionConfigurationValues.GlobalScope
        };
        configuration.Windows.Add(new HmrSubmissionConfigWindow
        {
            SubmissionConfigWindowId = 1,
            WindowType = SubmissionConfigurationValues.ReminderWindow,
            StartMonth = 1,
            StartDay = 1,
            EndMonth = 1,
            EndDay = 31
        });
        configuration.Windows.Add(new HmrSubmissionConfigWindow
        {
            SubmissionConfigWindowId = 2,
            WindowType = SubmissionConfigurationValues.BlackoutWindow,
            StartMonth = 2,
            StartDay = 1,
            EndMonth = 3,
            EndDay = 31
        });
        return configuration;
    }
}

public class SubmissionConfigurationMessageServiceTests
{
    private readonly SubmissionConfigurationMessageService _service = new();
    private readonly SubmissionConfigurationWindowService _windows = new();

    [Fact]
    public void BuildNotice_GeneratesApprovedReminderAndBlackoutText()
    {
        var configuration = ConfiguredRules();
        var blackout = _windows.GetNextBlackout(configuration, new DateOnly(2027, 1, 1));

        var reminder = _service.BuildNotice(
            configuration,
            SubmissionConfigurationValues.ReminderState,
            blackout);
        var active = _service.BuildNotice(
            configuration,
            SubmissionConfigurationValues.BlackoutState,
            blackout);

        Assert.Equal(
            "Reminder: Work reports for asphalt paving activities exceeding 450 tonnes and graded aggregate seal activities of 7,000 m² or more must be submitted by January 31. From February 1 through March 31, contractors cannot submit these records. Contact Ministry staff if a late submission is required.",
            reminder);
        Assert.Equal(
            "Submission blackout in effect: From February 1 through March 31, maintenance contractors cannot submit work reports for asphalt paving activities exceeding 450 tonnes or graded aggregate seal activities of 7,000 m² or more. Contact Ministry staff if a late submission is required.",
            active);
    }

    [Fact]
    public void BuildViolationMessage_GeneratesApprovedRowSpecificText()
    {
        var configuration = ConfiguredRules();
        var blackout = _windows.GetNextBlackout(configuration, new DateOnly(2027, 2, 1));

        var message = _service.BuildViolationMessage(
            new SubmissionRestrictionRowDto { RowNum = 9, RecordNumber = "ABC-123" },
            blackout);

        Assert.Equal(
            "Row 9, record ABC-123: This record cannot be submitted by a contractor between February 1 and March 31 because the reported activity meets the capitalization threshold. Contact Ministry staff to submit the record on your behalf.",
            message);
    }

    internal static HmrSubmissionConfiguration ConfiguredRules()
    {
        var configuration = SubmissionConfigurationWindowServiceTests.TestConfiguration();
        configuration.Rules.Add(new HmrSubmissionConfigRule
        {
            DisplayOrder = 1,
            RuleType = SubmissionConfigurationValues.ActivityAccomplishmentRule,
            DisplayLabel = "Asphalt paving activities",
            ComparisonOperator = SubmissionConfigurationValues.GreaterThanOperator,
            ThresholdValue = 450,
            UnitOfMeasure = "tonne"
        });
        configuration.Rules.Add(new HmrSubmissionConfigRule
        {
            DisplayOrder = 2,
            RuleType = SubmissionConfigurationValues.ActivityAccomplishmentRule,
            DisplayLabel = "Graded aggregate seal activities",
            ComparisonOperator = SubmissionConfigurationValues.GreaterThanOrEqualOperator,
            ThresholdValue = 7000,
            UnitOfMeasure = "m2"
        });
        return configuration;
    }
}

public class SubmissionRestrictionEvaluatorTests
{
    [Fact]
    public async Task Evaluate_BlocksEachQualifyingBusinessRowAndHonoursThresholdOperators()
    {
        var configuration = SubmissionConfigurationMessageServiceTests.ConfiguredRules();
        configuration.Audiences.Add(new HmrSubmissionConfigAudience
        {
            AudienceType = SubmissionConfigurationValues.UserTypeAudience,
            AudienceValue = UserTypeDto.BUSINESS
        });
        AddActivity(configuration.Rules.ElementAt(0), 1, "101300");
        AddActivity(configuration.Rules.ElementAt(1), 2, "102300");

        var repository = new Mock<ISubmissionConfigurationRepository>(MockBehavior.Strict);
        repository.Setup(x => x.GetApplicableAsync(1, 7, true))
            .ReturnsAsync(new[] { configuration });
        var clock = new Mock<IPacificTimeService>(MockBehavior.Strict);
        var submittedAt = new DateTimeOffset(2027, 2, 1, 8, 0, 0, TimeSpan.Zero);
        clock.Setup(x => x.GetPacificDate(submittedAt)).Returns(new DateOnly(2027, 2, 1));
        var evaluator = new SubmissionRestrictionEvaluator(
            repository.Object,
            clock.Object,
            new SubmissionConfigurationWindowService(),
            new SubmissionConfigurationMessageService(),
            new[] { new ActivityAccomplishmentRestrictionRuleEvaluator() });
        var rows = new[]
        {
            Row(2, "A", "101300", 450),
            Row(3, "B", "101300", 450.01m),
            Row(4, "C", "102300", 6999.99m),
            Row(5, "D", "102300", 7000),
            Row(6, "E", "999999", 999999)
        };

        var result = await evaluator.EvaluateAsync(1, 7, UserTypeDto.BUSINESS, submittedAt, rows);

        Assert.Equal(new[] { 3, 5 }, result.Select(x => x.RowNum));
        Assert.All(result, violation => Assert.Contains("Contact Ministry staff", violation.Message));
        repository.VerifyAll();
        clock.VerifyAll();
    }

    [Fact]
    public async Task Evaluate_InternalUserIsAlwaysExemptWithoutLoadingConfiguration()
    {
        var repository = new Mock<ISubmissionConfigurationRepository>(MockBehavior.Strict);
        var clock = new Mock<IPacificTimeService>(MockBehavior.Strict);
        var evaluator = new SubmissionRestrictionEvaluator(
            repository.Object,
            clock.Object,
            new SubmissionConfigurationWindowService(),
            new SubmissionConfigurationMessageService(),
            new[] { new ActivityAccomplishmentRestrictionRuleEvaluator() });

        var result = await evaluator.EvaluateAsync(
            1,
            7,
            UserTypeDto.INTERNAL,
            DateTimeOffset.UtcNow,
            new[] { Row(2, "A", "101300", 1000) });

        Assert.Empty(result);
        repository.VerifyNoOtherCalls();
        clock.VerifyNoOtherCalls();
    }

    private static SubmissionRestrictionRowDto Row(
        int rowNum,
        string recordNumber,
        string activityNumber,
        decimal accomplishment)
    {
        return new SubmissionRestrictionRowDto
        {
            RowNum = rowNum,
            RecordNumber = recordNumber,
            ActivityNumber = activityNumber,
            Accomplishment = accomplishment
        };
    }

    private static void AddActivity(HmrSubmissionConfigRule rule, decimal id, string activityNumber)
    {
        rule.Activities.Add(new HmrSubmissionConfigRuleActivity
        {
            ActivityCodeId = id,
            ActivityCode = new HmrActivityCode
            {
                ActivityCodeId = id,
                ActivityNumber = activityNumber
            }
        });
    }
}

public class SubmissionConfigurationServiceTests
{
    [Fact]
    public async Task GetAll_DuringBlackoutReturnsCurrentWindowAndNextYearsBlackout()
    {
        var configuration = SubmissionConfigurationWindowServiceTests.TestConfiguration();
        var repository = new Mock<ISubmissionConfigurationRepository>(MockBehavior.Strict);
        repository.Setup(x => x.GetAllAsync()).ReturnsAsync(new[] { configuration });
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var clock = new Mock<IPacificTimeService>(MockBehavior.Strict);
        clock.Setup(x => x.GetCurrentPacificTime())
            .Returns(new DateTimeOffset(2027, 2, 15, 0, 0, 0, TimeSpan.FromHours(-8)));
        var service = Service(repository.Object, unitOfWork.Object, clock.Object);

        var result = Assert.Single(await service.GetAllAsync());

        Assert.Equal(new DateTime(2027, 2, 1), result.CurrentWindowStartDate);
        Assert.Equal(new DateTime(2027, 3, 31), result.CurrentWindowEndDate);
        Assert.Equal(new DateTime(2028, 2, 1), result.NextBlackoutStartDate);
        Assert.Equal(new DateTime(2028, 3, 31), result.NextBlackoutEndDate);
        repository.VerifyAll();
        unitOfWork.VerifyNoOtherCalls();
        clock.VerifyAll();
    }

    [Fact]
    public async Task UpdateActivation_UpdatesStateAndConcurrencyMetadata()
    {
        var configuration = SubmissionConfigurationWindowServiceTests.TestConfiguration();
        configuration.ConcurrencyControlNumber = 5;
        configuration.AppLastUpdateUserid = "ORIGINAL_USER";
        configuration.AppLastUpdateTimestamp = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);
        var repository = new Mock<ISubmissionConfigurationRepository>(MockBehavior.Strict);
        repository.Setup(x => x.GetByIdForUpdateAsync(1)).ReturnsAsync(configuration);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.Setup(x => x.Commit())
            .Callback(() =>
            {
                configuration.ConcurrencyControlNumber++;
                configuration.AppLastUpdateUserid = "ADMIN";
                configuration.AppLastUpdateTimestamp = new DateTime(2026, 9, 16, 12, 0, 0, DateTimeKind.Utc);
            })
            .Returns(true);
        var clock = new Mock<IPacificTimeService>(MockBehavior.Strict);
        clock.Setup(x => x.GetCurrentPacificTime())
            .Returns(new DateTimeOffset(2027, 2, 1, 0, 0, 0, TimeSpan.FromHours(-8)));
        var service = Service(repository.Object, unitOfWork.Object, clock.Object);

        var result = await service.UpdateActivationAsync(1, false, 5);

        Assert.Equal(SubmissionConfigurationActivationStatus.Updated, result.Status);
        Assert.False(result.Configuration.IsActive);
        Assert.Equal(SubmissionConfigurationValues.InactiveState, result.Configuration.CurrentState);
        Assert.Equal(6, result.Configuration.ConcurrencyControlNumber);
        Assert.Equal("ADMIN", result.Configuration.LastUpdatedBy);
        repository.VerifyAll();
        unitOfWork.VerifyAll();
        clock.VerifyAll();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateActivation_ReturnsNotFoundOrConflictWithoutWriting(bool exists)
    {
        var repository = new Mock<ISubmissionConfigurationRepository>(MockBehavior.Strict);
        repository.Setup(x => x.GetByIdForUpdateAsync(1))
            .ReturnsAsync(exists
                ? new HmrSubmissionConfiguration { ConcurrencyControlNumber = 7 }
                : null);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var clock = new Mock<IPacificTimeService>(MockBehavior.Strict);
        var service = Service(repository.Object, unitOfWork.Object, clock.Object);

        var result = await service.UpdateActivationAsync(1, true, 6);

        Assert.Equal(
            exists
                ? SubmissionConfigurationActivationStatus.Conflict
                : SubmissionConfigurationActivationStatus.NotFound,
            result.Status);
        unitOfWork.VerifyNoOtherCalls();
        clock.VerifyNoOtherCalls();
        repository.VerifyAll();
    }

    private static SubmissionConfigurationService Service(
        ISubmissionConfigurationRepository repository,
        IUnitOfWork unitOfWork,
        IPacificTimeService clock)
    {
        return new SubmissionConfigurationService(
            repository,
            unitOfWork,
            clock,
            new SubmissionConfigurationWindowService(),
            new SubmissionConfigurationMessageService());
    }
}

public class PacificTimeServiceTests
{
    [Theory]
    [InlineData("2025-01-01T07:59:00+00:00", "2024-12-31")]
    [InlineData("2025-01-01T08:00:00+00:00", "2025-01-01")]
    [InlineData("2025-03-09T09:59:00+00:00", "2025-03-09")]
    [InlineData("2025-03-09T10:00:00+00:00", "2025-03-09")]
    public void GetPacificDate_UsesPacificMidnightAndDaylightSaving(
        string timestamp,
        string expectedDate)
    {
        var service = new PacificTimeService(TimeProvider.System);

        var result = service.GetPacificDate(DateTimeOffset.Parse(timestamp));

        Assert.Equal(DateOnly.Parse(expectedDate), result);
    }

    [Theory]
    [InlineData("2025-01-01T08:00:00+00:00", -8)]
    [InlineData("2025-07-01T07:00:00+00:00", -7)]
    public void ConvertToPacific_UsesStandardAndDaylightSavingOffsets(
        string timestamp,
        int expectedOffsetHours)
    {
        var service = new PacificTimeService(TimeProvider.System);

        var result = service.ConvertToPacific(DateTimeOffset.Parse(timestamp));

        Assert.Equal(TimeSpan.FromHours(expectedOffsetHours), result.Offset);
    }
}

public class SubmissionConfigurationRepositoryTests
{
    [Fact]
    public async Task GetApplicable_ReturnsGlobalAndMatchingServiceAreaConfigurations()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"submission-config-{Guid.NewGuid()}")
            .Options;
        var currentUser = new HmcrCurrentUser
        {
            Username = "TEST_USER",
            AuthDirName = "IDIR",
            UserGuid = Guid.NewGuid()
        };

        await using var context = new AppDbContext(
            options,
            NullLogger<AppDbContext>.Instance,
            currentUser);
        var global = Configuration(1, SubmissionConfigurationValues.GlobalScope, isActive: true);
        var serviceAreaSeven = Configuration(2, SubmissionConfigurationValues.ServiceAreaScope, isActive: true);
        serviceAreaSeven.ServiceAreas.Add(ServiceAreaMapping(21, 2, 7));
        var serviceAreaEight = Configuration(3, SubmissionConfigurationValues.ServiceAreaScope, isActive: true);
        serviceAreaEight.ServiceAreas.Add(ServiceAreaMapping(31, 3, 8));
        var inactiveGlobal = Configuration(4, SubmissionConfigurationValues.GlobalScope, isActive: false);
        var stream = SubmissionStream();
        global.SubmissionStream = stream;
        serviceAreaSeven.SubmissionStream = stream;
        serviceAreaEight.SubmissionStream = stream;
        inactiveGlobal.SubmissionStream = stream;
        context.HmrSubmissionConfigurations.AddRange(
            global,
            serviceAreaSeven,
            serviceAreaEight,
            inactiveGlobal);
        context.SaveChanges();
        Assert.Equal(4, context.HmrSubmissionConfigurations.Count());
        Assert.Contains(
            context.HmrSubmissionConfigurations,
            x => x.SubmissionStreamId == 1 &&
                 x.IsActive &&
                 x.ScopeType == SubmissionConfigurationValues.GlobalScope);
        var repository = new SubmissionConfigurationRepository(context);

        var globalOnly = await repository.GetApplicableAsync(1, null);
        var forServiceAreaSeven = await repository.GetApplicableAsync(1, 7);
        var includingInactive = await repository.GetApplicableAsync(1, null, activeOnly: false);

        Assert.Equal(new[] { 1m }, globalOnly.Select(x => x.SubmissionConfigurationId));
        Assert.Equal(
            new[] { 1m, 2m },
            forServiceAreaSeven.Select(x => x.SubmissionConfigurationId).OrderBy(x => x));
        Assert.Equal(
            new[] { 1m, 4m },
            includingInactive.Select(x => x.SubmissionConfigurationId).OrderBy(x => x));
    }

    private static HmrSubmissionConfiguration Configuration(
        decimal id,
        string scopeType,
        bool isActive)
    {
        var timestamp = new DateTime(2026, 9, 16, 12, 0, 0, DateTimeKind.Utc);
        return new HmrSubmissionConfiguration
        {
            SubmissionConfigurationId = id,
            ConfigurationKey = $"CONFIG_{id}",
            ConfigurationName = $"Configuration {id}",
            SubmissionStreamId = 1,
            IsActive = isActive,
            EffectiveFromDate = new DateTime(2027, 1, 1),
            TimeZoneId = Constants.VancouverTimeZone,
            ScopeType = scopeType,
            ConcurrencyControlNumber = 1,
            DbAuditCreateUserid = "TEST_USER",
            DbAuditCreateTimestamp = timestamp,
            DbAuditLastUpdateUserid = "TEST_USER",
            DbAuditLastUpdateTimestamp = timestamp
        };
    }

    private static HmrSubmissionConfigServiceArea ServiceAreaMapping(
        decimal id,
        decimal configurationId,
        decimal serviceAreaNumber)
    {
        var timestamp = new DateTime(2026, 9, 16, 12, 0, 0, DateTimeKind.Utc);
        return new HmrSubmissionConfigServiceArea
        {
            SubmissionConfigServiceAreaId = id,
            SubmissionConfigurationId = configurationId,
            ServiceAreaNumber = serviceAreaNumber,
            ConcurrencyControlNumber = 1,
            DbAuditCreateUserid = "TEST_USER",
            DbAuditCreateTimestamp = timestamp,
            DbAuditLastUpdateUserid = "TEST_USER",
            DbAuditLastUpdateTimestamp = timestamp
        };
    }

    private static HmrSubmissionStream SubmissionStream()
    {
        var timestamp = new DateTime(2026, 9, 16, 12, 0, 0, DateTimeKind.Utc);
        return new HmrSubmissionStream
        {
            SubmissionStreamId = 1,
            StreamName = "MC Work Reporting",
            StagingTableName = TableNames.WorkReport,
            ConcurrencyControlNumber = 1,
            AppCreateUserid = "TEST_USER",
            AppCreateTimestamp = timestamp,
            AppCreateUserGuid = Guid.Empty,
            AppCreateUserDirectory = "IDIR",
            AppLastUpdateUserid = "TEST_USER",
            AppLastUpdateTimestamp = timestamp,
            AppLastUpdateUserGuid = Guid.Empty,
            AppLastUpdateUserDirectory = "IDIR",
            DbAuditCreateUserid = "TEST_USER",
            DbAuditCreateTimestamp = timestamp,
            DbAuditLastUpdateUserid = "TEST_USER",
            DbAuditLastUpdateTimestamp = timestamp
        };
    }
}
