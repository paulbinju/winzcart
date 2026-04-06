using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Winzcart.API.Exceptions;
using Winzcart.Application;
using Winzcart.Infrastructure;
using Winzcart.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT Authorization
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Winzcart.API", Version = "v1" });

    // Add JWT Authentication support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Dependency Injection
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Exception Handler
builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.WebHost.UseUrls("http://0.0.0.0:10000");
var app = builder.Build();

// Auto-migrate in Development
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<WinzcartDbContext>();
        try
        {
            // Apply migrations (will create DB if doesn't exist)
            context.Database.Migrate();
            
            // Seed Admin User if not exists
            var userRepo = scope.ServiceProvider.GetRequiredService<Winzcart.Application.Interfaces.Repositories.IUserRepository>();
            Task.Run(async () => {
                var admin = await userRepo.GetByEmailAsync("admin@winzcart.com");
                if (admin == null)
                {
                    string hash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                    await userRepo.AddAsync(new Winzcart.Domain.Entities.AppUser
                    {
                        Name = "Super Admin",
                        Email = "admin@winzcart.com",
                        PasswordHash = hash,
                        Role = Winzcart.Domain.Enums.UserRole.Admin,
                        IsActive = true
                    });
                    
                    var uow = scope.ServiceProvider.GetRequiredService<Winzcart.Application.Interfaces.Services.IUnitOfWork>();
                    await uow.SaveChangesAsync();
                }
            }).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        }
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
