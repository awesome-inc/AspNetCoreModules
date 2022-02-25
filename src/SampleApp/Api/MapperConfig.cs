using AutoMapper;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace SampleApp.Api;

internal static class MapperConfig
{
    internal static readonly IConfigurationProvider Configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<BlogProfile>();
        }
    );

    public static IMapper CreateMapper()
    {
        return Configuration.CreateMapper();
    }
}
