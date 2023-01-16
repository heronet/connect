using connect.Data;
using Microsoft.EntityFrameworkCore;

namespace connect.Extensions;

public static class SeedDbExtension
{
    public static void SeedDb(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ApplicationDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
    }
}
