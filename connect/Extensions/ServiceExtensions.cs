using System.Text;
using connect.Data;
using connect.Models;
using connect.Services;
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
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? configuration["JWT_SECRET"];
        var uriString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? configuration.GetConnectionString("DATABASE_URL");
        var uri = new Uri(uriString);
        var db = uri.AbsolutePath.Trim('/');
        var user = uri.UserInfo.Split(':')[0];
        var passwd = uri.UserInfo.Split(':')[1];
        var port = uri.Port > 0 ? uri.Port : 5432;
        var dbConnectionString = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}", uri.Host, db, user, passwd, port);


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

        services.AddAuthentication(options =>
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
            // SignalR
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/hubs")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
        services.AddScoped<TokenService>();
        services.AddCors(options =>
        {
            options.AddPolicy("any", policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(Environment.GetEnvironmentVariable("CORS_ORIGIN") ?? configuration["CORS_ORIGIN"])
                    .AllowCredentials();
            });
        });
        return services;
    }
}
