namespace Autofac.Extensions.DependencyInjection;

public static class SettingsExtensions
{
    /// <summary>
    ///     Binds <typeparamref name="TSettings" /> to the specified configuration and
    ///     registers the configured instance to the specified <paramref name="builder" />.
    /// </summary>
    /// <typeparam name="TSettings">The settings class</typeparam>
    /// <param name="builder">builder to register settings to</param>
    /// <param name="configuration">configuration to bind settings to</param>
    /// <param name="section">section where to bind settings</param>
    public static void RegisterSettings<TSettings>(this ContainerBuilder builder,
        IConfiguration configuration, string section)
        where TSettings : class, new()
    {
        var settings = configuration
            .GetSection(section)
            .Get<TSettings>() ?? new TSettings();

        builder
            .RegisterInstance(settings).AsSelf()
            .SingleInstance();
    }
}
