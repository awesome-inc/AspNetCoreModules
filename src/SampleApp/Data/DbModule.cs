using Autofac.AspNetCore.Modules;
using Microsoft.EntityFrameworkCore;

namespace SampleApp.Data;

public class DbModule: AspNetCoreModule
{
    public DbModule(DbSettings settings)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public DbSettings Settings { get; }

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AppContext>(options =>
        {
            if (Settings.Enabled)
            {
                options
                    .UseSqlite(Settings.Connection)
                    .UseSnakeCaseNamingConvention()
                    .UseLazyLoadingProxies();
            }
            else
            {
                options.UseInMemoryDatabase("apps-in-memory");
            }
        });
        

        services.AddHealthChecks()
            // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0#database-probe
            .AddDbContextCheck<AppContext>("database");
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppContext>();
        if (!context.Database.IsInMemory())
            context.Database.Migrate();
    }
}
