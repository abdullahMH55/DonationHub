using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Auth.Commands.Login;
using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Application.Services;
using DonationHub.Domain.ResultHandling;
using FluentAssertions;
using Moq;

namespace DonationHub.UnitTests.Application;

public class AuthHandlerTests
{
    [Fact]
    public async Task Login_WhenCredentialsAreInvalid_ReturnsInvalidCredentials()
    {
        var authService = new Mock<IAuthService>();
        authService
            .Setup(service => service.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthResponce>.Failure(AuthErrors.InvalidCredentials));

        var handler = new LoginCommandHandler(authService.Object);
        var command = new LoginCommand(new LoginRequest
        {
            Email = "user@example.com",
            Password = "wrong-password"
        });

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.InvalidCredentials);
    }
}
