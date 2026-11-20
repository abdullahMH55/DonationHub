using System.Diagnostics;
using System.Text;
using DonationHub.Application.Common.Options;
using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Auth;
using DonationHub.Application.Repositories;
using DonationHub.Application.Services;
using DonationHub.Infrastructure.Data;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Repositories;
using DonationHub.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using IAuthService = DonationHub.Application.Services.IAuthService;

namespace DonationHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IdentitySeeder>();

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

        var jwt = configuration.GetSection(JwtOptions.SectionName);
        var secretKey = jwt["SecretKey"];

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();

                    if (context.Response.HasStarted)
                        return;

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/problem+json";

                    var problem = new ProblemDetails
                    {
                        Type = "https://httpstatuses.io/401",
                        Title = AuthErrors.Unauthorized.Message,
                        Status = StatusCodes.Status401Unauthorized
                    };
                    problem.Extensions["code"] = AuthErrors.Unauthorized.Code;
                    problem.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

                    await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
                },
                OnForbidden = async context =>
                {
                    if (context.Response.HasStarted)
                        return;

                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/problem+json";

                    var problem = new ProblemDetails
                    {
                        Type = "https://httpstatuses.io/403",
                        Title = AuthErrors.Forbidden.Message,
                        Status = StatusCodes.Status403Forbidden
                    };
                    problem.Extensions["code"] = AuthErrors.Forbidden.Code;
                    problem.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

                    await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
                }
            };
        });

        return services;
    }
}
