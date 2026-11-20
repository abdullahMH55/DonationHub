using DonationHub.Application.Common.Options;
using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Infrastructure.Data;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace DonationHub.UnitTests.Application;

public class AuthServiceTests
{
    private const string ExistingEmail = "existing@example.com";
    private const string ExistingUserName = "existing-user";

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsEmailAlreadyExists()
    {
        var service = CreateService(existingEmail: ExistingEmail, existingUserName: null);

        var result = await service.RegisterAsync(
            new RegisterRequest { Email = ExistingEmail, UserName = "new-user" },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.EmailAlreadyExists);
    }

    [Fact]
    public async Task RegisterAsync_WhenUserNameAlreadyExists_ReturnsUserNameAlreadyExists()
    {
        var service = CreateService(existingEmail: null, existingUserName: ExistingUserName);

        var result = await service.RegisterAsync(
            new RegisterRequest { Email = "new@example.com", UserName = ExistingUserName },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.UserNameAlreadyExists);
    }

    [Fact]
    public async Task CreateStaffAsync_WhenEmailAlreadyExists_ReturnsEmailAlreadyExists()
    {
        var service = CreateService(existingEmail: ExistingEmail, existingUserName: null);

        var result = await service.CreateStaffAsync(
            new CreateStaffRequest
            {
                UserName = "new-staff",
                FullName = "New Staff",
                Email = ExistingEmail,
                Password = "Password1",
                Role = "Admin"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.EmailAlreadyExists);
    }

    [Fact]
    public async Task CreateStaffAsync_WhenUserNameAlreadyExists_ReturnsUserNameAlreadyExists()
    {
        var service = CreateService(existingEmail: null, existingUserName: ExistingUserName);

        var result = await service.CreateStaffAsync(
            new CreateStaffRequest
            {
                UserName = ExistingUserName,
                FullName = "New Staff",
                Email = "new-staff@example.com",
                Password = "Password1",
                Role = "CampaignManager"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.UserNameAlreadyExists);
    }

    [Fact]
    public async Task CreateStaffAsync_WhenRoleIsNotAllowed_ReturnsInvalidRole()
    {
        var service = CreateService(existingEmail: null, existingUserName: null);

        var result = await service.CreateStaffAsync(
            new CreateStaffRequest
            {
                UserName = "new-staff",
                FullName = "New Staff",
                Email = "new-staff@example.com",
                Password = "Password1",
                Role = "Donor"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.InvalidRole);
    }

    private static AuthService CreateService(string? existingEmail, string? existingUserName)
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var emailStore = store.As<IUserEmailStore<ApplicationUser>>();

        emailStore
            .Setup(s => s.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string email, CancellationToken _) =>
                existingEmail is not null && email == existingEmail
                    ? new ApplicationUser { Id = Guid.NewGuid(), Email = email, UserName = email }
                    : null);
        store
            .Setup(s => s.FindByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string userName, CancellationToken _) =>
                existingUserName is not null && userName == existingUserName
                    ? new ApplicationUser { Id = Guid.NewGuid(), Email = ExistingEmail, UserName = userName }
                    : null);

        var normalizer = new Mock<ILookupNormalizer>();
        normalizer.Setup(n => n.NormalizeEmail(It.IsAny<string>())).Returns<string>(value => value);
        normalizer.Setup(n => n.NormalizeName(It.IsAny<string>())).Returns<string>(value => value);

        var userManager = new UserManager<ApplicationUser>(
            store.Object,
            Options.Create(new IdentityOptions()),
            Mock.Of<IPasswordHasher<ApplicationUser>>(),
            new List<IUserValidator<ApplicationUser>>(),
            new List<IPasswordValidator<ApplicationUser>>(),
            normalizer.Object,
            new IdentityErrorDescriber(),
            Mock.Of<IServiceProvider>(),
            new NullLogger<UserManager<ApplicationUser>>());

        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DonationHubUnitTests;Trusted_Connection=True;")
            .Options;

        var jwtOptions = Options.Create(new JwtOptions
        {
            SecretKey = new string('k', 40),
            Issuer = "DonationHub",
            Audience = "DonationHubClients",
            ExpiryMinutes = 15,
            RefreshTokenExpiryDays = 7
        });

        return new AuthService(
            jwtOptions,
            userManager,
            new AppDbContext(dbContextOptions),
            TimeProvider.System);
    }
}
