using Hmcr.Api.Controllers;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.NotificationPreference;
using Hmcr.Model.Dtos.User;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hmcr.Tests;

public class UsersControllerNotificationPreferenceTests
{
    [Fact]
    public async Task GetPreferences_ReturnsForbidForIneligibleUser()
    {
        var preferences = new Mock<INotificationPreferenceService>(MockBehavior.Strict);
        preferences.SetupGet(x => x.IsCurrentUserEligible).Returns(false);
        var controller = CreateController(preferences);

        var result = await controller.GetCurrentUserNotificationPreferencesAsync();

        Assert.IsType<ForbidResult>(result.Result);
        preferences.VerifyGet(x => x.IsCurrentUserEligible, Times.Once);
        preferences.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetPreferences_ReturnsCanonicalResponseForEligibleUser()
    {
        var expected = new NotificationPreferencesDto();
        var preferences = new Mock<INotificationPreferenceService>(MockBehavior.Strict);
        preferences.SetupGet(x => x.IsCurrentUserEligible).Returns(true);
        preferences.Setup(x => x.GetCurrentUserPreferencesAsync()).ReturnsAsync(expected);
        var controller = CreateController(preferences);

        var result = await controller.GetCurrentUserNotificationPreferencesAsync();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
        preferences.VerifyAll();
    }

    [Theory]
    [InlineData(NotificationPreferenceUpdateResult.Updated, typeof(NoContentResult))]
    [InlineData(NotificationPreferenceUpdateResult.ServiceAreaNotAssigned, typeof(NotFoundResult))]
    [InlineData(NotificationPreferenceUpdateResult.UnsupportedSubmissionStream, typeof(BadRequestObjectResult))]
    public async Task UpdatePreference_MapsDomainResultToHttpStatus(
        NotificationPreferenceUpdateResult domainResult,
        Type expectedResultType)
    {
        var update = new NotificationPreferenceUpdateDto
        {
            SuccessfulUploadsEnabled = false,
            UploadErrorsEnabled = true
        };
        var preferences = new Mock<INotificationPreferenceService>(MockBehavior.Strict);
        preferences.SetupGet(x => x.IsCurrentUserEligible).Returns(true);
        preferences
            .Setup(x => x.UpdateCurrentUserPreferenceAsync(7, 3, update))
            .ReturnsAsync(domainResult);
        var controller = CreateController(preferences);

        var result = await controller.UpdateCurrentUserNotificationPreferenceAsync(7, 3, update);

        Assert.IsType(expectedResultType, result);
        preferences.VerifyAll();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateAllPreferences_ForwardsOnlyEnabledValue_AndReturnsNoContent(bool enabled)
    {
        var preferences = new Mock<INotificationPreferenceService>(MockBehavior.Strict);
        preferences.SetupGet(x => x.IsCurrentUserEligible).Returns(true);
        preferences
            .Setup(x => x.UpdateAllCurrentUserPreferencesAsync(enabled))
            .Returns(Task.CompletedTask);
        var controller = CreateController(preferences);

        var result = await controller.UpdateAllCurrentUserNotificationPreferencesAsync(
            new NotificationPreferenceBulkUpdateDto { Enabled = enabled });

        Assert.IsType<NoContentResult>(result);
        preferences.VerifyAll();
    }

    private static UsersController CreateController(Mock<INotificationPreferenceService> preferences)
    {
        return new UsersController(
            new Mock<IUserService>().Object,
            new Mock<IKeycloakService>().Object,
            preferences.Object,
            new HmcrCurrentUser
            {
                UserInfo = new UserCurrentDto
                {
                    SystemUserId = 42,
                    UserType = UserTypeDto.INTERNAL
                }
            });
    }
}
