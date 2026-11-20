using DonationHub.Application.Features.Auth.Commands.Register;
using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Application.Features.Campaigns.Command.Create;
using DonationHub.Application.Features.Campaigns.Command.Update;
using DonationHub.Application.Features.Campaigns.DTOs;
using FluentAssertions;

namespace DonationHub.UnitTests.Application;

public class ValidatorTests
{
    private static readonly DateTime Now = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void CreateCampaignValidator_RejectsInvalidInput()
    {
        var validator = new CreateCampaignCommandValidator();
        var command = new CreateCampaignCommand(
            new CreateCampaignRequest
            {
                TitleAr = string.Empty,
                TitleEn = string.Empty,
                GoalAmount = 0,
                EndDate = Now.AddDays(1),
                CategoryId = Guid.Empty
            },
            Guid.NewGuid());

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Request.TitleAr");
        result.Errors.Should().Contain(error => error.PropertyName == "Request.TitleEn");
        result.Errors.Should().Contain(error => error.PropertyName == "Request.GoalAmount");
        result.Errors.Should().Contain(error => error.PropertyName == "Request.CategoryId");
    }

    [Fact]
    public void UpdateCampaignValidator_RejectsMissingRowVersion()
    {
        var validator = new UpdateCampaignCommandValidator();
        var command = new UpdateCampaignCommand(
            Guid.NewGuid(),
            new UpdateCampaignRequest
            {
                TitleAr = "عنوان",
                TitleEn = "Title",
                GoalAmount = 1000,
                EndDate = Now.AddDays(1),
                CategoryId = Guid.NewGuid(),
                RowVersion = Array.Empty<byte>()
            });

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Request.RowVersion");
    }

    [Fact]
    public void RegisterValidator_RejectsWeakPassword()
    {
        var validator = new RegisterCommandValidator();
        var command = new RegisterCommand(new RegisterRequest
        {
            UserName = "user",
            FullName = "User Name",
            Email = "user@example.com",
            Password = "password"
        });

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Request.Password");
    }

    [Fact]
    public void RegisterValidator_RejectsUnsupportedPreferredLanguage()
    {
        var validator = new RegisterCommandValidator();
        var command = new RegisterCommand(new RegisterRequest
        {
            UserName = "user",
            FullName = "User Name",
            Email = "user@example.com",
            Password = "Password1",
            PreferredLanguage = "fr"
        });

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Request.PreferredLanguage");
    }

    [Fact]
    public void RegisterValidator_AcceptsEnglishAndArabic()
    {
        var validator = new RegisterCommandValidator();

        foreach (var language in new[] { "en", "ar", "AR" })
        {
            var command = new RegisterCommand(new RegisterRequest
            {
                UserName = "user",
                FullName = "User Name",
                Email = "user@example.com",
                Password = "Password1",
                PreferredLanguage = language
            });

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
