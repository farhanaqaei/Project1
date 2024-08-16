using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Project1.Application.Logs;
using Project1.Application.Products;
using Project1.Core.Generals.Interfaces;
using Project1.Core.Logs.Interfaces;
using Project1.Core.Products.Interfaces;
using Project1.Core.Users.Interfaces;
using Project1.Infrastructure.Cache;
using Project1.Infrastructure.Data;
using Project1.Infrastructure.LogData;
using Project1.Infrastructure.UserManagement.Entities;
using Project1.Infrastructure.UserManagement.Implementations;
using System.Text;

namespace Project1.IOC;

public static class ConfigServiceCollectionExtension
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        AddDbContext(services, config);
        AddLogDbContext(services, config);
        AddIdentity(services);
        AddJWtAuthentication(services, config);

        services.AddScoped<ICacheManager, CacheManager>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }

    public static void AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });
    }

    public static void AddLogDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<LogDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("LogConnection"));
        });
    }

    public static void AddJWtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var jwtSettings = config.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    //ValidIssuer = config.GetSection("ValidIssuer").Value,
                    //ValidAudience = config.GetSection("ValidAudience").Value,
                };
            });
    }

    public static void AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
}
