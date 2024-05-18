using Microsoft.EntityFrameworkCore;

namespace Rovema.Coravel.Data;

public static class ContextExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<CoravelContext>();

        context.Database.Migrate();
    }
}
