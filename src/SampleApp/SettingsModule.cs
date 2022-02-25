using Autofac;
using Autofac.AspNetCore.Modules;

namespace SampleApp;

internal class SettingsModule : Module
{
    public SettingsModule(IConfiguration configuration)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IConfiguration Configuration { get; }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterSettings<DbSettings>(Configuration, "Db");
    }
}
