using AutoMapper;
using SampleApp.Data;
using AppContext = SampleApp.Data.AppContext;

namespace SampleApp.Api;

public class BlogService : IBlogService
{
    private readonly AppContext _context;
    private readonly IMapper _mapper;

    public BlogService(AppContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public IEnumerable<BlogDto> GetAll()
    {
        return _mapper.ProjectTo<BlogDto>(_context.Blogs);
    }

    public BlogDto Get(int id)
    {
        var blog = _context.Blogs.Find(id) ?? throw new KeyNotFoundException();
        return _mapper.Map<BlogDto>(blog);
    }

    public void Upsert(BlogDto dto)
    {
        var blog = _mapper.Map<Blog>(dto);
        if (_context.Blogs.Find(dto.Id) != null)
        {
            _context.Blogs.Update(blog);
        }
        else
        {
            _context.Blogs.Add(blog);
        }

        _context.SaveChanges();
    }

    public void Remove(int id)
    {
        var blog = _context.Blogs.Find(id) ?? throw new KeyNotFoundException();
        _context.Blogs.Remove(blog);
        _context.SaveChanges();
    }

    public IEnumerable<PostDto> GetPosts(int id)
    {
        var posts = _context.Posts.Where(p => p.BlogId == id);
        return _mapper.ProjectTo<PostDto>(posts);
    }

    public PostDto GetPost(int id)
    {
        var post = _context.Posts.Find(id) ?? throw new KeyNotFoundException();
        return _mapper.Map<PostDto>(post);
    }

    public void UpsertPost(int blogId, PostDto dto)
    {
        var blog = _context.Blogs.Find(blogId) ?? throw new KeyNotFoundException();
        var post = _mapper.Map<Post>(dto);
        if (_context.Posts.Find(dto.Id) != null)
        {
            _context.Posts.Update(post);
        }
        else
        {
            blog.Posts.Add(post);
        }

        _context.SaveChanges();
    }

    public void RemovePost(int id)
    {
        var post = _context.Posts.Find(id) ?? throw new KeyNotFoundException();
        _context.Posts.Remove(post);
        _context.SaveChanges();
    }
}
