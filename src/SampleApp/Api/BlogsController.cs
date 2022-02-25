using Microsoft.AspNetCore.Mvc;

namespace SampleApp.Api;

[ApiController]
[Route("api/[controller]")]
public class BlogsController : ControllerBase
{
    private readonly IBlogService _service;

    public BlogsController(IBlogService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet]
    public IEnumerable<BlogDto> GetAll()
    {
        return _service.GetAll();
    }

    [HttpGet("{id:int}")]
    public BlogDto Get(int id)
    {
        return _service.Get(id);
    }

    [HttpPut]
    public void Create(BlogDto blog)
    {
        _service.Upsert(blog);
    }

    [HttpDelete("{id:int}")]
    public void Delete(int id)
    {
        _service.Remove(id);
    }

    [HttpGet("{id:int}/posts")]
    public IEnumerable<PostDto> GetPosts(int id)
    {
        return _service.GetPosts(id);
    }

    [HttpGet("{blogId:int}/posts/{postId:int}")]
    public PostDto GetPost(int blogId, int postId)
    {
        return _service.GetPost(postId);
    }

    [HttpPut("{blogId:int}/posts")]
    public void CreatePost(int blogId, PostDto post)
    {
        _service.UpsertPost(blogId, post);
    }

    [HttpDelete("{blogId:int}/posts/{postId:int}")]
    public void DeletePost(int blogId, int postId)
    {
        _service.RemovePost(postId);
    }
}
