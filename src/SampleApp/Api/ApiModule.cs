using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;

namespace SampleApp.Api;

public class ApiModule : AspNetCoreModule
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                "{controller}/{action=Index}/{id?}");
        });
    }

    protected override void Load(ContainerBuilder builder)
    {
        // common auto mapper
        builder.RegisterInstance(MapperConfig.CreateMapper())
            .As<IMapper>()
            .SingleInstance();

        builder.RegisterType<BlogService>().As<IBlogService>().SingleInstance();
    }
}
