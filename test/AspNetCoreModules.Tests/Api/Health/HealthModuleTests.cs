using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;
using SampleApp.Api.Health;
using Xunit;

namespace AspNetCoreModules.Tests.Api.Health;

public class HealthModuleTests
{
    [Fact]
    public void ShouldAddHealthChecks()
    {
        var module = new HealthModule();
        var services = new ServiceCollection();
        module.ConfigureServices(services);
        services.ShouldContain<HealthCheckService>(because: "health check should be configured");
    }

    [Fact]
    public async Task ShouldMapHealthChecksAsync()
    {
        await using var stream = new MemoryStream();
        HttpContext context = new DefaultHttpContext { Response = { Body = stream } };

        var dbReport = new HealthReportEntry(HealthStatus.Healthy, "database", TimeSpan.FromSeconds(0.5), null, null);
        var entries = new Dictionary<string, HealthReportEntry>
        {
            { "database", dbReport }
        };
        var totalDuration = TimeSpan.FromSeconds(1);
        var report = new HealthReport(entries, totalDuration);

        await HealthModule.WriteHealthResponseAsync(context, report);

        context.Response.ContentType.Should().Be(MediaTypeNames.Application.Json);

        stream.Flush();
        stream.Seek(0, SeekOrigin.Begin);
        var json = await new StreamReader(stream).ReadToEndAsync();
        var actual = JObject.Parse(json);
        actual.Should().ContainKey("components");
        var components = actual["components"]!.Value<JArray>();
        components.Should().HaveCount(1);
        var cmp = components![0];
        cmp["name"]!.Value<string>().Should().Be("database");
        cmp["status"]!.Value<string>().Should().Be("Healthy");
    }
}
