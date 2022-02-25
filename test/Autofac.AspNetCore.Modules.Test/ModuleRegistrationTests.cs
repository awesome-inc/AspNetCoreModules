using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Autofac.AspNetCore.Modules.Test;

public class ModuleRegistrationTests
{
    [Fact]
    public void ShouldRegisterModules()
    {
        var settings = new SampleSettings { Enabled = true };
        var logger = Substitute.For<ILogger<SampleModule>>();
        
        var builder = WebApplication.CreateBuilder();
        // Add AutoFac: https://stackoverflow.com/questions/69754985/adding-autofac-to-net-core-6-0-using-the-new-single-file-template
        builder.Host
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());
        
        // 1. Configure services 
        var hook = builder.UseModules()
            .With(b =>
            {
                b.RegisterInstance(settings);
                b.RegisterInstance(logger);
            })
            .Build();
        logger.ReceivedWithAnyArgs(1).LogInformation(null, null, null, null);
        logger.ClearReceivedCalls();
        
        // 2. Load
        using var app = builder.Build();
        logger.ReceivedWithAnyArgs(1).LogInformation(null, null, null, null);
        logger.ClearReceivedCalls();
        
        // 3. Configure
        hook.Use(app);
        logger.ReceivedWithAnyArgs(1).LogInformation(null, null, null, null);
    }
}
