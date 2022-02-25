using Autofac.Core;

namespace Autofac.AspNetCore.Modules;

public interface IAspNetCoreModule : IModule
{
    void ConfigureServices(IServiceCollection services);
    void Configure(IApplicationBuilder app, IWebHostEnvironment env);
}
