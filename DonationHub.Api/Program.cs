using DonationHub.Api.OpenApi;
using DonationHub.Application;
using DonationHub.Application.Common.Options;
using DonationHub.Application.Options;
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.Data;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace DonationHub.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).AddInterceptors(new SoftDeleteInterceptor())
        );

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        builder.Services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

        builder.Services.AddOptions<SeedAdminOptions>().BindConfiguration(SeedAdminOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

        builder.Services.AddOptions<DonationOptions>().BindConfiguration(DonationOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
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

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        await app.RunAsync();
    }
}
