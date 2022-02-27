using System.Net.Mime;
using System.Text.Json;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SampleApp.Api.Health;

internal class HealthModule : AspNetCoreModule
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks();
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/api/health", new HealthCheckOptions
            {
                //AllowCachingResponses = true,
                ResponseWriter = WriteHealthResponseAsync
            });

            endpoints.MapHealthChecks("/api/health/startup");
            endpoints.MapHealthChecks("/api/health/liveness", new HealthCheckOptions { Predicate = _ => false });
            endpoints.MapHealthChecks("/api/health/readiness", new HealthCheckOptions { Predicate = _ => false });
        });
    }

    internal static async Task WriteHealthResponseAsync(HttpContext context, HealthReport report)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
        var result = JsonSerializer.Serialize(report.PrettyJson());
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(result);
    }
}
