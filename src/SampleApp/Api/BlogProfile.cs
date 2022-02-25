using AutoMapper;
using SampleApp.Data;

namespace SampleApp.Api;

internal class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<Blog, BlogDto>().ReverseMap();
        CreateMap<Post, PostDto>().ReverseMap();
    }
}

public record BlogDto(int Id, string? Url);

public record PostDto(int Id, string? Title, string? Content);
