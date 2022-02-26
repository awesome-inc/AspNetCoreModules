using Autofac.Core;

namespace Autofac.Extensions.DependencyInjection;

public interface IAspNetCoreModule : IModule
{
    void ConfigureServices(IServiceCollection services);
    void Configure(IApplicationBuilder app, IWebHostEnvironment env);
}
