using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Data;

namespace AspNetCoreModules.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup: class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<SampleAppContext>))!;

            services.Remove(descriptor);

            services.AddDbContext<SampleAppContext>(options => options.TestConfigure());

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<SampleAppContext>();
            // var logger = scopedServices
            //     .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

            db.Database.EnsureCreated();

            // try
            // {
            //     Utilities.InitializeDbForTests(db);
            // }
            // catch (Exception ex)
            // {
            //     logger.LogError(ex, "An error occurred seeding the " +
            //                         "database with test messages. Error: {Message}", ex.Message);
            // }
        });
    }
}
