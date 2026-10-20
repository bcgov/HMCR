using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.SubmissionConfiguration;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.User;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Hmcr.Tests;

public class WorkReportSubmissionRestrictionTests
{
    [Fact]
    public async Task ParseRowsAsync_RejectsFileImmediatelyAndReportsCsvRow()
    {
        var evaluator = new Mock<ISubmissionRestrictionEvaluator>(MockBehavior.Strict);
        var submittedAt = new DateTimeOffset(2027, 2, 1, 8, 0, 0, TimeSpan.Zero);
        evaluator
            .Setup(x => x.EvaluateAsync(
                1,
                7,
                UserTypeDto.BUSINESS,
                submittedAt,
                It.Is<IEnumerable<SubmissionRestrictionRowDto>>(rows =>
                    rows.Single().RowNum == 2 &&
                    rows.Single().RecordNumber == "REC001" &&
                    rows.Single().ActivityNumber == "101300" &&
                    rows.Single().Accomplishment == 451)))
            .ReturnsAsync(new[]
            {
                new SubmissionRestrictionViolationDto
                {
                    SubmissionConfigurationId = 1,
                    ConfigurationKey = "CAPITALIZED_HARD_SURFACING",
                    RowNum = 2,
                    RecordNumber = "REC001",
                    ActivityNumber = "101300",
                    Message = "Row 2, record REC001: blocked"
                }
            });

        var serviceAreaService = new Mock<IServiceAreaService>();
        serviceAreaService
            .Setup(x => x.GetServiceAreaByServiceAreaNumberAsyc(7))
            .ReturnsAsync(new ServiceAreaNumberDto
            {
                ServiceAreaNumber = 7,
                HighwayUniquePrefix = "07"
            });
        var statuses = new Mock<ISubmissionStatusService>();
        statuses.SetupGet(x => x.RowReceived).Returns(1);

        var service = new TestableWorkReportService(
            Mock.Of<IUnitOfWork>(),
            Mock.Of<ISubmissionStreamService>(),
            Mock.Of<ISubmissionObjectRepository>(),
            Mock.Of<ISumbissionRowRepository>(),
            Mock.Of<IContractTermRepository>(),
            statuses.Object,
            Mock.Of<IWorkReportRepository>(),
            Mock.Of<IFieldValidatorService>(),
            serviceAreaService.Object,
            evaluator.Object,
            new HmcrCurrentUser { UserType = UserTypeDto.BUSINESS },
            new FixedTimeProvider(submittedAt));
        var submission = new SubmissionObjectCreateDto
        {
            SubmissionStreamId = 1,
            ServiceAreaNumber = 7
        };
        var errors = new Dictionary<string, List<string>>();
        const string csv =
            "Record Type,Service Area,Record Number,Activity Number,End Date,Accomplishment,Unit of Measure,Posted Date\n" +
            "Q,7,REC001,101300,2026-08-01,451,tonne,2026-08-01\n";

        var accepted = await service.ParseAsync(submission, new StringReader(csv), errors);

        Assert.False(accepted);
        Assert.Equal(new[] { "Row 2, record REC001: blocked" }, errors["Submission Blackout"]);
        Assert.Single(submission.SubmissionRows);
        evaluator.VerifyAll();
    }

    private sealed class TestableWorkReportService : WorkReportService
    {
        public TestableWorkReportService(
            IUnitOfWork unitOfWork,
            ISubmissionStreamService streamService,
            ISubmissionObjectRepository submissionRepo,
            ISumbissionRowRepository rowRepo,
            IContractTermRepository contractRepo,
            ISubmissionStatusService statusService,
            IWorkReportRepository workReportRepo,
            IFieldValidatorService validator,
            IServiceAreaService serviceAreaService,
            ISubmissionRestrictionEvaluator evaluator,
            HmcrCurrentUser currentUser,
            TimeProvider timeProvider)
            : base(
                unitOfWork,
                streamService,
                submissionRepo,
                rowRepo,
                contractRepo,
                statusService,
                workReportRepo,
                validator,
                NullLogger<WorkReportService>.Instance,
                serviceAreaService,
                evaluator,
                currentUser,
                timeProvider)
        {
        }

        public Task<bool> ParseAsync(
            SubmissionObjectCreateDto submission,
            TextReader reader,
            Dictionary<string, List<string>> errors)
        {
            return ParseRowsAsync(submission, reader, errors);
        }
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public FixedTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}
