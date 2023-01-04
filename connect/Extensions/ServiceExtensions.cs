using System.Text;
using connect.Data;
using connect.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace connect.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Extension Method to Add Configuration Services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns>A reference to this object after the operation has completed</returns>
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        string dbConnectionString;
        string jwtSecret;

        // Depending on if in development or production, use either Server-provided
        // connection string, or development connection string from env var.
        if (env == "Development")
        {
            // Use connection string from file.
            dbConnectionString = configuration.GetConnectionString("PostgreSQL");
            jwtSecret = configuration["JWT_SECRET"];
        }
        else
        {
            jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            // Database
            // Use connection string provided at runtime.
            var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            var host = Environment.GetEnvironmentVariable("HostName");

            var uriString = connUrl;
            var uri = new Uri(uriString);
            var db = uri.AbsolutePath.Trim('/');
            var user = uri.UserInfo.Split(':')[0];
            var passwd = uri.UserInfo.Split(':')[1];
            var port = uri.Port > 0 ? uri.Port : 5432;
            dbConnectionString = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}", uri.Host, db, user, passwd, port);

        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(dbConnectionString);
        });
        services.AddIdentity<User, IdentityRole>(setupAction =>
        {
            setupAction.User.RequireUniqueEmail = true;
            setupAction.Password.RequireNonAlphanumeric = false;
            setupAction.Password.RequireDigit = false;
            setupAction.Password.RequiredLength = 4;
            setupAction.Password.RequireLowercase = false;
            setupAction.Password.RequireUppercase = false;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
          .AddSignInManager<SignInManager<User>>()
          .AddRoles<IdentityRole>()
          .AddRoleManager<RoleManager<IdentityRole>>()
          .AddRoleValidator<RoleValidator<IdentityRole>>();

        services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false
            };
        });
        return services;
    }
}
