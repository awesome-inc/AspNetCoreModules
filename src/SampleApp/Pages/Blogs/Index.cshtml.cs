#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleApp.Api;

namespace SampleApp.Pages.Blogs;

public class IndexModel : PageModel
{
    private readonly IBlogService _service;

    public IndexModel(IBlogService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public IEnumerable<BlogDto> Blogs { get; set; }

    public void OnGet()
    {
        Blogs = _service.GetAll();
    }
}
