using Autofac.AspNetCore.Modules;

namespace SampleApp;

internal class SwaggerModule : AspNetCoreModule
{
    public override void ConfigureServices(IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            return;
        }

        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
