using DonationHub.Application.Features.Categories.Create;
using DonationHub.Application.Features.Categories.Delete;
using DonationHub.Application.Features.Categories.GetAll;
using DonationHub.Application.Features.Categories.GetById;
using DonationHub.Application.Features.Categories.Update;
using Microsoft.Extensions.DependencyInjection;

namespace DonationHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CreateCategoryHandler>();
        services.AddScoped<UpdateCategoryHandler>();
        services.AddScoped<DeleteCategoryHandler>();
        services.AddScoped<GetCategoryByIdHandler>();
        services.AddScoped<GetAllCategoriesHandler>();
        return services;
    }
}
