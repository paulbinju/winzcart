using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Winzcart.Application.Interfaces.Services;

namespace Winzcart.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        // Services will be registered in Infrastructure or API layer to keep Application free of Web / DI specifics where possible,
        // but for standard clean arch, we can register them here if they are pure application services.
        services.AddScoped<IUserService, Services.UserService>();
        services.AddScoped<IMerchantService, Services.MerchantService>();
        services.AddScoped<IBillService, Services.BillService>();
        services.AddScoped<ICouponService, Services.CouponService>();
        services.AddScoped<IPointsService, Services.PointsService>();
        services.AddScoped<IAdminService, Services.AdminService>();
        services.AddScoped<IWinnerService, Services.WinnerService>();

        return services;
    }
}
