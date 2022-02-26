namespace Autofac.Extensions.DependencyInjection;

public abstract class AspNetCoreModule : Module, IAspNetCoreModule
{
    public virtual void ConfigureServices(IServiceCollection services) { }
    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env) { }
}
