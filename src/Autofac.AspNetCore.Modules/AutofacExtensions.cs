using System.Reflection;
using Autofac.Core;

namespace Autofac.AspNetCore.Modules;

public static class AutofacExtensions
{
    public static IModuleAppHookBuilder UseModules(this WebApplicationBuilder builder)
    {
        return new ModuleAppHookBuilder(builder)
            .Scanning(Assembly.GetCallingAssembly());
    }

    public static IModuleAppHookBuilder With<TModule>(this IModuleAppHookBuilder builder, TModule module) where TModule : IModule
    {
        return builder.With(b => b.RegisterModule(module));
    }

    public static void Use(this Action<IApplicationBuilder> action, IApplicationBuilder app)
    {
        action.Invoke(app);
    }
}
