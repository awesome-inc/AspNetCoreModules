using System.Reflection;

namespace Autofac.Extensions.DependencyInjection;

internal class ModuleAppHookBuilder : IModuleAppHookBuilder
{
    private readonly List<Assembly> _assemblies = new();
    private readonly WebApplicationBuilder _builder;
    private readonly List<IAspNetCoreModule> _modules = new();
    private Action<ContainerBuilder>? _configure;
    private bool _configured;
    private bool _used;
    private Func<IAspNetCoreModule,int> _orderSelector = DefaultOrderSelector;

    public ModuleAppHookBuilder(WebApplicationBuilder builder)
    {
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    public IModuleAppHookBuilder Scanning(IEnumerable<Assembly> assemblies)
    {
        _assemblies.AddRange(assemblies);
        return this;
    }

    public IModuleAppHookBuilder With(Action<ContainerBuilder> configure)
    {
        _configure = configure ?? throw new ArgumentNullException(nameof(configure));
        return this;
    }

    public IModuleAppHookBuilder Ordered(Func<IAspNetCoreModule, int> orderSelector)
    {
        _orderSelector = orderSelector ?? throw new ArgumentNullException(nameof(orderSelector));
        return this;
    }

    public Action<IApplicationBuilder> Build()
    {
        if (_configured)
        {
            throw new InvalidOperationException("Already configured");
        }

        if (!_assemblies.Any())
        {
            throw new InvalidOperationException("No assemblies to scan for modules");
        }

        var moduleFinder = new ContainerBuilder();
        moduleFinder.RegisterAssemblyTypes(_assemblies.ToArray())
            .Where(t => typeof(IAspNetCoreModule).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
            .As<IAspNetCoreModule>();

        _configure?.Invoke(moduleFinder);

        using var moduleContainer = moduleFinder.Build();

        var resolved = Sorted(moduleContainer.Resolve<IEnumerable<IAspNetCoreModule>>());
        
        _modules.AddRange(resolved);
        _modules.ForEach(module =>
            module.ConfigureServices(_builder.Services)
        );

        _builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            _modules.ForEach(module => builder.RegisterModule(module))
        );

        _configured = true;
        return Use;
    }

    internal IEnumerable<IAspNetCoreModule> Sorted(IEnumerable<IAspNetCoreModule> resolved)
    {
        return resolved.OrderBy(_orderSelector);
    }

    public IModuleAppHookBuilder Scanning(params Assembly[] assemblies)
    {
        return Scanning(assemblies.ToList());
    }

    private void Use(IApplicationBuilder app)
    {
        if (!_configured)
        {
            throw new InvalidOperationException("Not yet configured");
        }

        if (_used)
        {
            throw new InvalidOperationException("Already invoked on the app");
        }

        _modules.ForEach(module => module.Configure(app, _builder.Environment));
        _used = true;
    }
    
    private static int DefaultOrderSelector(IAspNetCoreModule module)
    {
        int order = (module as IHaveOrder)?.Order ?? 0;
        order += module.GetType().GetCustomAttribute<OrderAttribute>()?.Order ?? 0;
        return order;
    }
    
}
