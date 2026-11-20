using DonationHub.Application.Common.Behaviors;
using DonationHub.Domain.ResultHandling;
using FluentAssertions;
using FluentValidation;
using MediatR;

namespace DonationHub.UnitTests.Application;

public class ValidationBehaviorTests
{
    private sealed record TestRequest(string Name) : IRequest<Result<string>>;

    private sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShortCircuitsAndReturnsValidationFailed()
    {
        var behavior = new ValidationBehavior<TestRequest, Result<string>>(new[] { new TestRequestValidator() });
        var nextCalled = false;

        var result = await behavior.Handle(
            new TestRequest(string.Empty),
            _ =>
            {
                nextCalled = true;
                return Task.FromResult(Result<string>.Success("ok"));
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Validation.Failed");
        result.Error.Errors.Should().ContainKey("Name");
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_CallsNext()
    {
        var behavior = new ValidationBehavior<TestRequest, Result<string>>(new[] { new TestRequestValidator() });
        var nextCalled = false;

        var result = await behavior.Handle(
            new TestRequest("valid"),
            _ =>
            {
                nextCalled = true;
                return Task.FromResult(Result<string>.Success("ok"));
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
        nextCalled.Should().BeTrue();
    }
}
