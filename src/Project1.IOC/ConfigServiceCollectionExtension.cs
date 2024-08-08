using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project1.Application.Logs;
using Project1.Application.Products;
using Project1.Core.Generals.Interfaces;
using Project1.Core.Logs.Interfaces;
using Project1.Core.Products.Interfaces;
using Project1.Infrastructure.Data;
using Project1.Infrastructure.LogData;

namespace Project1.IOC;

public static class ConfigServiceCollectionExtension
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        AddDbContext(services, config);
        AddLogDbContext(services, config);

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
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

    //public static void AddJWtAuthentication(this IServiceCollection services, IConfiguration config)
    //{
    //    services.AddAuthentication(opt =>
    //    {
    //        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //    })
    //        .AddJwtBearer(options =>
    //        {
    //            options.TokenValidationParameters = new TokenValidationParameters
    //            {
    //                ValidateIssuer = true,
    //                ValidateAudience = true,
    //                ValidateLifetime = true,
    //                ClockSkew = TimeSpan.Zero,
    //                ValidateIssuerSigningKey = true,
    //                ValidIssuer = config.GetSection("ValidIssuer").Value,
    //                ValidAudience = config.GetSection("ValidAudience").Value,
    //                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("SecretKey").Value))
    //            };
    //        });
    //}

    //public static void AddIdentityCore(this IServiceCollection services)
    //{
    //    services.AddIdentityCore<User>(options =>
    //    {
    //        options.User.RequireUniqueEmail = true;
    //        options.Password.RequireDigit = true;
    //        options.Password.RequiredLength = 6;
    //        options.Password.RequireLowercase = false;
    //        options.Password.RequireUppercase = false;
    //        options.Password.RequireNonAlphanumeric = false;
    //    })
    //        .AddEntityFrameworkStores<ApplicationDbContext>();
    //}
}
