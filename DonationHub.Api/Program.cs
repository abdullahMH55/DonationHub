using System.Diagnostics;
using System.Text.Json.Serialization;
using DonationHub.Api.ExceptionHandling;
using DonationHub.Api.OpenApi;
using DonationHub.Application;
using DonationHub.Application.Common.Options;
using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Options;
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.Data;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Interceptors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace DonationHub.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<SoftDeleteInterceptor>();

        builder.Services.AddDbContext<AppDbContext>((sp, options) =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>())
        );

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        builder.Services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

        builder.Services.AddOptions<SeedAdminOptions>().BindConfiguration(SeedAdminOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

        builder.Services.AddOptions<DonationOptions>().BindConfiguration(DonationOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

        builder.Services.AddAuthorization();
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problem = new ProblemDetails
                {
                    Type = "https://httpstatuses.io/400",
                    Title = CommonErrors.ValidationFailed.Message,
                    Status = StatusCodes.Status400BadRequest
                };
                problem.Extensions["code"] = CommonErrors.ValidationFailed.Code;
                problem.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                problem.Extensions["errors"] = context.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors
                            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "Invalid value."
                                : error.ErrorMessage)
                            .ToArray());

                return new ObjectResult(problem)
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ContentTypes = { "application/problem+json" }
                };
            };
        });
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
            await seeder.SeedAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseExceptionHandler();
        app.UseStatusCodePages(async context =>
        {
            var httpContext = context.HttpContext;
            var error = httpContext.Response.StatusCode switch
            {
                StatusCodes.Status404NotFound => CommonErrors.NotFound,
                StatusCodes.Status405MethodNotAllowed => CommonErrors.MethodNotAllowed,
                StatusCodes.Status415UnsupportedMediaType => CommonErrors.UnsupportedMediaType,
                _ => null
            };

            if (error is null)
                return;

            var problem = new ProblemDetails
            {
                Type = $"https://httpstatuses.io/{httpContext.Response.StatusCode}",
                Title = error.Message,
                Status = httpContext.Response.StatusCode
            };
            problem.Extensions["code"] = error.Code;
            problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            await httpContext.Response.WriteAsJsonAsync(
                problem,
                options: null,
                contentType: "application/problem+json",
                cancellationToken: httpContext.RequestAborted);
        });
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        await app.RunAsync();
    }
}
