using System.Diagnostics;
using DonationHub.Application.ErrorCatalogue;
using DonationHub.Domain.ResultHandling;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, HttpContext httpContext)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return result.ToProblemResult(httpContext);
    }

    public static IActionResult ToActionResult(this Result result, HttpContext httpContext)
    {
        if (result.IsSuccess)
            return new NoContentResult();

        return result.ToProblemResult(httpContext);
    }

    private static IActionResult ToProblemResult(this Result result, HttpContext httpContext)
    {
        var error = result.Error ?? CommonErrors.UnexpectedError;

        var problem = new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{(int)error.Type}",
            Title = error.Message,
            Status = (int)error.Type
        };
        problem.Extensions["code"] = error.Code;
        problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (error.Errors is not null)
            problem.Extensions["errors"] = error.Errors;

        var objectResult = new ObjectResult(problem)
        {
            StatusCode = (int)error.Type
        };
        objectResult.ContentTypes.Add("application/problem+json");

        return objectResult;
    }
}
