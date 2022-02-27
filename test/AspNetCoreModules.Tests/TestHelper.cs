using System;
using System.Linq;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SampleApp.Data;

namespace AspNetCoreModules.Tests;

public static class TestHelper
{
    public static SampleAppContext GetDbContext(string? databaseName = null)
    {
        var builder = new DbContextOptionsBuilder<SampleAppContext>();
        builder.TestConfigure(databaseName);
        return new SampleAppContext(builder.Options);
    }

    public static void TestConfigure(this DbContextOptionsBuilder builder, string? databaseName = null)
    {
        databaseName ??= Guid.NewGuid().ToString();
        builder
            .UseInMemoryDatabase(databaseName)
            .EnableSensitiveDataLogging();
    }

    public static void ShouldContain<TService>(this IServiceCollection services,
        ServiceLifetime lifeTime = ServiceLifetime.Singleton, string? because = null)
    {
        services.Should().Contain(service => service.ServiceType == typeof(TService)
                                             && service.Lifetime == lifeTime, because);
    }

    public static void ShouldConfigureHealthCheck<T>(this IAspNetCoreModule module) where T : IHealthCheck
    {
        var services = new ServiceCollection();
        module.ConfigureServices(services);
        services.ShouldConfigureHealthCheck<T>();
    }

    public static void ShouldConfigureHealthCheck<T>(this IServiceCollection services) where T : IHealthCheck
    {
        services.ShouldContain<HealthCheckService>(because: "health check should be configured");
        var provider = services.BuildServiceProvider();
        var healthCheck = provider.GetRequiredService<IOptions<HealthCheckServiceOptions>>()
            .Value.Registrations.Single().Factory(provider);
        healthCheck.Should().BeAssignableTo(typeof(T));
    }
}
