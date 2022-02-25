using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autofac.AspNetCore.Modules.Test;

public class SampleModule : AspNetCoreModule
{
    private readonly SampleSettings _settings;
    private readonly ILogger<SampleModule> _logger;

    public SampleModule(SampleSettings settings, ILogger<SampleModule> logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public override void ConfigureServices(IServiceCollection services)
    {
        if (!_settings.Enabled) return;
        _logger.LogInformation("Configure sample services...");
        // TODO
        base.ConfigureServices(services);
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!_settings.Enabled) return;
        _logger.LogInformation(message: $"Configure sample app ({env.EnvironmentName})...");
        base.Configure(app, env);
    }

    protected override void Load(ContainerBuilder builder)
    {
        if (!_settings.Enabled) return;
        _logger.LogInformation(message: $"Configure sample...");
        base.Load(builder);
    }
}
