using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreModules.Test;

public class SampleModule : AspNetCoreModule
{
    private readonly ILogger<SampleModule> _logger;
    private readonly SampleSettings _settings;

    public SampleModule(SampleSettings settings, ILogger<SampleModule> logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        if (!_settings.Enabled)
        {
            return;
        }

        _logger.LogInformation("Configure sample services...");
        // TODO
        base.ConfigureServices(services);
    }

    protected override void Load(ContainerBuilder builder)
    {
        if (!_settings.Enabled)
        {
            return;
        }

        _logger.LogInformation("Configure sample...");
        base.Load(builder);
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!_settings.Enabled)
        {
            return;
        }

        _logger.LogInformation($"Configure sample app ({env.EnvironmentName})...");
        base.Configure(app, env);
    }
}
