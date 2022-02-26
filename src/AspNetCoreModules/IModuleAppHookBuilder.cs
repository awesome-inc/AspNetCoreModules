using System.Reflection;

namespace Autofac.Extensions.DependencyInjection;

public interface IModuleAppHookBuilder
{
    /// <summary>
    ///     Adds the specified <see cref="Assembly" /> for scanning <see cref="IAspNetCoreModule" />.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>The <see cref="IModuleAppHookBuilder" /></returns>
    IModuleAppHookBuilder Scanning(IEnumerable<Assembly> assemblies);

    /// <summary>
    ///     Configures dependencies to instantiate the <see cref="IAspNetCoreModule" /> found by scanning.
    /// </summary>
    /// <param name="configure">The configuration action</param>
    /// <returns>The <see cref="IModuleAppHookBuilder" /></returns>
    IModuleAppHookBuilder With(Action<ContainerBuilder> configure);

    /// <summary>
    ///     An action to be invoked on the final <see cref="IApplicationBuilder" />.
    ///     This will call `Configure()`-method of the scanned and registered modules.
    /// </summary>
    /// <returns></returns>
    Action<IApplicationBuilder> Build();
}
