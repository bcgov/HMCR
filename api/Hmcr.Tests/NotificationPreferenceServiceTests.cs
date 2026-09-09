using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.NotificationPreference;
using Hmcr.Model.Dtos.User;
using Moq;

namespace Hmcr.Tests;

public class NotificationPreferenceServiceTests
{
    [Fact]
    public async Task BusinessUser_IsNotEligible_AndCannotReadPreferences()
    {
        var repository = new Mock<INotificationPreferenceRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var service = CreateService(UserTypeDto.BUSINESS, 17, repository, unitOfWork);

        Assert.False(service.IsCurrentUserEligible);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetCurrentUserPreferencesAsync());
    }

    [Fact]
    public async Task GetPreferences_UsesAuthenticatedCurrentUserId()
    {
        var repository = new Mock<INotificationPreferenceRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var expected = new NotificationPreferencesDto();
        repository.Setup(x => x.GetForUserAsync(42)).ReturnsAsync(expected);
        var service = CreateService(UserTypeDto.INTERNAL, 42, repository, unitOfWork);

        var actual = await service.GetCurrentUserPreferencesAsync();

        Assert.Same(expected, actual);
        repository.Verify(x => x.GetForUserAsync(42), Times.Once);
    }

    [Fact]
    public async Task UpdatePreference_RejectsUnassignedServiceAreaWithoutWriting()
    {
        var repository = new Mock<INotificationPreferenceRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository
            .Setup(x => x.GetActiveServiceAreaUserIdAsync(42, 7))
            .ReturnsAsync((decimal?)null);
        var service = CreateService(UserTypeDto.INTERNAL, 42, repository, unitOfWork);

        var result = await service.UpdateCurrentUserPreferenceAsync(7, 3, Update(false, true));

        Assert.Equal(NotificationPreferenceUpdateResult.ServiceAreaNotAssigned, result);
        repository.VerifyNoOtherCalls();
        unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdatePreference_RejectsUnsupportedStreamWithoutWriting()
    {
        var repository = new Mock<INotificationPreferenceRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository
            .Setup(x => x.GetActiveServiceAreaUserIdAsync(42, 7))
            .ReturnsAsync((decimal?)71);
        repository
            .Setup(x => x.IsSupportedSubmissionStreamAsync(99))
            .ReturnsAsync(false);
        var service = CreateService(UserTypeDto.INTERNAL, 42, repository, unitOfWork);

        var result = await service.UpdateCurrentUserPreferenceAsync(7, 99, Update(false, true));

        Assert.Equal(NotificationPreferenceUpdateResult.UnsupportedSubmissionStream, result);
        repository.VerifyNoOtherCalls();
        unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdatePreference_UpsertsOnlyTheAuthenticatedUsersAssignment_AndCommits()
    {
        var repository = new Mock<INotificationPreferenceRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository
            .Setup(x => x.GetActiveServiceAreaUserIdAsync(42, 7))
            .ReturnsAsync((decimal?)71);
        repository
            .Setup(x => x.IsSupportedSubmissionStreamAsync(3))
            .ReturnsAsync(true);
        repository
            .Setup(x => x.UpsertAsync(71, 3, false, true))
            .Returns(Task.CompletedTask);
        unitOfWork.Setup(x => x.Commit()).Returns(true);
        var service = CreateService(UserTypeDto.INTERNAL, 42, repository, unitOfWork);

        var result = await service.UpdateCurrentUserPreferenceAsync(7, 3, Update(false, true));

        Assert.Equal(NotificationPreferenceUpdateResult.Updated, result);
        repository.VerifyAll();
        unitOfWork.Verify(x => x.Commit(), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateAllPreferences_IsScopedToAuthenticatedCurrentUser_AndCommits(bool enabled)
    {
        var repository = new Mock<INotificationPreferenceRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository.Setup(x => x.SetAllAsync(42, enabled)).Returns(Task.CompletedTask);
        unitOfWork.Setup(x => x.Commit()).Returns(true);
        var service = CreateService(UserTypeDto.INTERNAL, 42, repository, unitOfWork);

        await service.UpdateAllCurrentUserPreferencesAsync(enabled);

        repository.VerifyAll();
        unitOfWork.Verify(x => x.Commit(), Times.Once);
    }

    private static NotificationPreferenceService CreateService(
        string userType,
        decimal systemUserId,
        Mock<INotificationPreferenceRepository> repository,
        Mock<IUnitOfWork> unitOfWork)
    {
        var currentUser = new HmcrCurrentUser
        {
            UserInfo = new UserCurrentDto
            {
                SystemUserId = systemUserId,
                UserType = userType
            }
        };

        return new NotificationPreferenceService(repository.Object, unitOfWork.Object, currentUser);
    }

    private static NotificationPreferenceUpdateDto Update(bool successEnabled, bool errorEnabled)
    {
        return new NotificationPreferenceUpdateDto
        {
            SuccessfulUploadsEnabled = successEnabled,
            UploadErrorsEnabled = errorEnabled
        };
    }
}
