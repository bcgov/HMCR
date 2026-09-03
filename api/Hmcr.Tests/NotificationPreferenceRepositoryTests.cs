using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Mappings;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Hmcr.Tests;

public class NotificationPreferenceRepositoryTests
{
    [Fact]
    public async Task GetForUser_ReturnsActiveAssignmentsAndSupportedStreams_WithMissingRowsEnabled()
    {
        using var context = CreateContext();
        SeedUsersAreasAndStreams(context, UserTypeDto.BUSINESS);
        context.HmrServiceAreaUsers.AddRange(
            Assignment(101, 1, 2),
            Assignment(102, 1, 1),
            Assignment(103, 1, 3, DateTime.Today));
        context.HmrNotificationPreferences.Add(new HmrNotificationPreference
        {
            NotificationPreferenceId = 1001,
            ServiceAreaUserId = 101,
            SubmissionStreamId = 1,
            SuccessEmailEnabled = false,
            ErrorEmailEnabled = true
        });
        context.SaveChanges();
        var repository = new NotificationPreferenceRepository(context);

        var result = await repository.GetForUserAsync(1);

        Assert.Equal(new decimal[] { 1, 2 }, result.ServiceAreas.Select(x => x.ServiceAreaNumber));
        Assert.All(result.ServiceAreas, serviceArea =>
            Assert.Equal(new decimal[] { 1, 2, 3 }, serviceArea.ReportTypes.Select(x => x.SubmissionStreamId)));
        Assert.DoesNotContain(
            result.ServiceAreas.SelectMany(x => x.ReportTypes),
            x => x.StagingTableName == TableNames.SaltReport);

        var areaOneWork = result.ServiceAreas.Single(x => x.ServiceAreaNumber == 1)
            .ReportTypes.Single(x => x.SubmissionStreamId == 1);
        Assert.True(areaOneWork.SuccessfulUploadsEnabled);
        Assert.True(areaOneWork.UploadErrorsEnabled);

        var areaTwoWork = result.ServiceAreas.Single(x => x.ServiceAreaNumber == 2)
            .ReportTypes.Single(x => x.SubmissionStreamId == 1);
        Assert.False(areaTwoWork.SuccessfulUploadsEnabled);
        Assert.True(areaTwoWork.UploadErrorsEnabled);
    }

    [Fact]
    public async Task Upsert_CreatesThenUpdatesSinglePreference()
    {
        using var context = CreateContext();
        SeedUsersAreasAndStreams(context);
        context.HmrServiceAreaUsers.Add(Assignment(101, 1, 1));
        context.SaveChanges();
        var repository = new NotificationPreferenceRepository(context);

        await repository.UpsertAsync(101, 1, false, true);
        context.SaveChanges();
        await repository.UpsertAsync(101, 1, true, false);
        context.SaveChanges();

        var preference = Assert.Single(context.HmrNotificationPreferences);
        Assert.True(preference.SuccessEmailEnabled);
        Assert.False(preference.ErrorEmailEnabled);
    }

    [Fact]
    public async Task SetAll_UpdatesAndFillsOnlyCurrentUsersActiveSupportedTargets()
    {
        using var context = CreateContext();
        SeedUsersAreasAndStreams(context);
        context.HmrServiceAreaUsers.AddRange(
            Assignment(101, 1, 1),
            Assignment(102, 1, 2),
            Assignment(103, 1, 3, DateTime.Today),
            Assignment(201, 2, 1));
        context.HmrNotificationPreferences.AddRange(
            new HmrNotificationPreference
            {
                NotificationPreferenceId = 1001,
                ServiceAreaUserId = 101,
                SubmissionStreamId = 1,
                SuccessEmailEnabled = true,
                ErrorEmailEnabled = true
            },
            new HmrNotificationPreference
            {
                NotificationPreferenceId = 2001,
                ServiceAreaUserId = 201,
                SubmissionStreamId = 1,
                SuccessEmailEnabled = true,
                ErrorEmailEnabled = true
            });
        context.SaveChanges();
        var repository = new NotificationPreferenceRepository(context);

        await repository.SetAllAsync(1, false);
        context.SaveChanges();

        var currentUserPreferences = context.HmrNotificationPreferences
            .Where(x => x.ServiceAreaUserId == 101 || x.ServiceAreaUserId == 102)
            .ToList();
        Assert.Equal(6, currentUserPreferences.Count);
        Assert.All(currentUserPreferences, preference =>
        {
            Assert.False(preference.SuccessEmailEnabled);
            Assert.False(preference.ErrorEmailEnabled);
        });
        Assert.DoesNotContain(context.HmrNotificationPreferences, x =>
            x.ServiceAreaUserId == 103 ||
            (x.ServiceAreaUserId == 101 && x.SubmissionStreamId == 4) ||
            (x.ServiceAreaUserId == 102 && x.SubmissionStreamId == 4));

        var otherUserPreference = context.HmrNotificationPreferences.Single(x => x.ServiceAreaUserId == 201);
        Assert.True(otherUserPreference.SuccessEmailEnabled);
        Assert.True(otherUserPreference.ErrorEmailEnabled);
    }

    [Fact]
    public async Task EnsureDefaults_IsIdempotentAndInternalOnly()
    {
        using var context = CreateContext();
        SeedUsersAreasAndStreams(context);
        context.HmrServiceAreaUsers.AddRange(
            Assignment(101, 1, 1),
            Assignment(201, 2, 1));
        context.SaveChanges();
        var repository = new NotificationPreferenceRepository(context);

        await repository.EnsureDefaultsForInternalUserAsync(1);
        context.SaveChanges();
        await repository.EnsureDefaultsForInternalUserAsync(1);
        context.SaveChanges();
        await repository.EnsureDefaultsForInternalUserAsync(2);
        context.SaveChanges();

        Assert.Equal(3, context.HmrNotificationPreferences.Count());
        Assert.All(context.HmrNotificationPreferences, preference =>
        {
            Assert.Equal(101, preference.ServiceAreaUserId);
            Assert.True(preference.SuccessEmailEnabled);
            Assert.True(preference.ErrorEmailEnabled);
        });
    }

    [Fact]
    public void EmailRecipients_ApplyCurrentPreferenceForInternalUsers_AndAlwaysIncludeBusinessUsers()
    {
        using var context = CreateContext();
        SeedUsersAreasAndStreams(context);
        context.HmrSystemUsers.AddRange(
            User(3, UserTypeDto.BUSINESS),
            User(4, UserTypeDto.INTERNAL, DateTime.Today),
            User(5, UserTypeDto.INTERNAL),
            User(6, UserTypeDto.INTERNAL));
        context.HmrServiceAreaUsers.AddRange(
            Assignment(101, 1, 1),
            Assignment(201, 2, 1),
            Assignment(301, 3, 1),
            Assignment(401, 4, 1),
            Assignment(501, 5, 1, DateTime.Today),
            Assignment(601, 6, 1));
        context.HmrNotificationPreferences.AddRange(
            Preference(2001, 201, success: false, error: true),
            Preference(3001, 301, success: false, error: false),
            Preference(6001, 601, success: true, error: false));
        context.SaveChanges();
        var repository = new UserRepository(
            context,
            CreateMapper(),
            context._currentUser,
            new Mock<IPartyRepository>().Object);

        var successRecipients = repository
            .GetActiveUsersByServiceAreaNumber(1, 1, isError: false)
            .Select(x => x.SystemUserId)
            .OrderBy(x => x)
            .ToArray();
        var errorRecipients = repository
            .GetActiveUsersByServiceAreaNumber(1, 1, isError: true)
            .Select(x => x.SystemUserId)
            .OrderBy(x => x)
            .ToArray();

        Assert.Equal(new decimal[] { 1, 3, 6 }, successRecipients);
        Assert.Equal(new decimal[] { 1, 2, 3 }, errorRecipients);
    }

    private static AppDbContext CreateContext()
    {
        var currentUser = new HmcrCurrentUser
        {
            UserGuid = Guid.NewGuid(),
            Username = "TESTUSER",
            AuthDirName = UserTypeDto.IDIR
        };
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options, NullLogger<AppDbContext>.Instance, currentUser);
    }

    private static void SeedUsersAreasAndStreams(
        AppDbContext context,
        string secondUserType = UserTypeDto.INTERNAL)
    {
        context.HmrSystemUsers.AddRange(
            User(1, UserTypeDto.INTERNAL),
            User(2, secondUserType));
        context.HmrServiceAreas.AddRange(
            Area(1, "South Island"),
            Area(2, "Central Island"),
            Area(3, "North Island"));
        context.HmrSubmissionStreams.AddRange(
            Stream(3, "Wildlife Reporting", TableNames.WildlifeReport),
            Stream(1, "MC Work Reporting", TableNames.WorkReport),
            Stream(4, "Salt Reporting", TableNames.SaltReport),
            Stream(2, "Rockfall Reporting", TableNames.RockfallReport));
    }

    private static HmrSystemUser User(decimal id, string userType, DateTime? endDate = null)
    {
        return new HmrSystemUser
        {
            SystemUserId = id,
            UserGuid = Guid.NewGuid(),
            Username = $"USER{id}",
            UserDirectory = userType == UserTypeDto.INTERNAL ? UserTypeDto.IDIR : UserTypeDto.BCeId,
            UserType = userType,
            FirstName = "Test",
            LastName = $"User{id}",
            Email = $"user{id}@example.com",
            EndDate = endDate
        };
    }

    private static HmrServiceArea Area(decimal number, string name)
    {
        return new HmrServiceArea
        {
            ServiceAreaId = number,
            ServiceAreaNumber = number,
            ServiceAreaName = name,
            DistrictNumber = 1
        };
    }

    private static HmrSubmissionStream Stream(decimal id, string name, string tableName)
    {
        return new HmrSubmissionStream
        {
            SubmissionStreamId = id,
            StreamName = name,
            StagingTableName = tableName
        };
    }

    private static HmrServiceAreaUser Assignment(
        decimal id,
        decimal systemUserId,
        decimal serviceAreaNumber,
        DateTime? endDate = null)
    {
        return new HmrServiceAreaUser
        {
            ServiceAreaUserId = id,
            SystemUserId = systemUserId,
            ServiceAreaNumber = serviceAreaNumber,
            EndDate = endDate
        };
    }

    private static HmrNotificationPreference Preference(
        decimal id,
        decimal serviceAreaUserId,
        bool success,
        bool error)
    {
        return new HmrNotificationPreference
        {
            NotificationPreferenceId = id,
            ServiceAreaUserId = serviceAreaUserId,
            SubmissionStreamId = 1,
            SuccessEmailEnabled = success,
            ErrorEmailEnabled = error
        };
    }

    private static IMapper CreateMapper()
    {
        return new MapperConfiguration(configuration =>
            configuration.AddProfile(new EntityToModelProfile()))
            .CreateMapper();
    }
}
