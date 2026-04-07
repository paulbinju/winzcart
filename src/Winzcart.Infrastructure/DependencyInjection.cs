using Microsoft.Extensions.DependencyInjection;
using Winzcart.Application.Interfaces.Repositories;
using Winzcart.Application.Interfaces.Services;
using Winzcart.Infrastructure.Data;
using Winzcart.Infrastructure.Repositories;
using Winzcart.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Winzcart.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WinzcartDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMerchantRepository, MerchantRepository>();
        services.AddScoped<IBillRepository, BillRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<IUserCouponRepository, UserCouponRepository>();
        services.AddScoped<IPointTransactionRepository, PointTransactionRepository>();
        services.AddScoped<IWeeklyWinnerRepository, WeeklyWinnerRepository>();

        // Services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IOcrService, MockOcrService>();
        services.AddScoped<IImageStorageService, MockImageStorageService>();

        // JWT Authentication Setup
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "super_secret_key_that_is_at_least_32_characters_long_for_hmacsha256!";
        var issuer = jwtSettings["Issuer"] ?? "winzcart";
        var audience = jwtSettings["Audience"] ?? "winzcart-users";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // Set true in prod
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Authorization Policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("MerchantOnly", policy => policy.RequireRole("Merchant"));
            options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
        });

        return services;
    }
}
