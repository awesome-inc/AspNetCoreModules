using Autofac.AspNetCore.Modules;
using Autofac.Extensions.DependencyInjection;
using SampleApp;

var builder = WebApplication.CreateBuilder(args);

// Add AutoFac: https://stackoverflow.com/questions/69754985/adding-autofac-to-net-core-6-0-using-the-new-single-file-template
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((_, config) => config.AddEnvironmentVariables());

// Add services to the container.
builder.Services.AddRazorPages();

// 1. Register Autofac/AspNetModules
var modules = builder.UseModules()
    .With(new SettingsModule(builder.Configuration))
    .Build();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// 2. Use Autofac/AspNetModules
modules.Use(app);

app.Run();
