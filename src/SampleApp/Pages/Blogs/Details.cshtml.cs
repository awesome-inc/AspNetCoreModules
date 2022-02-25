#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleApp.Api;

namespace SampleApp.Pages.Blogs;

public class DetailsModel : PageModel
{
    private readonly IBlogService _service;

    public DetailsModel(IBlogService service)
    {
        _service = service;
    }

    public BlogDto Blog { get; set; }

    public IActionResult OnGet(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Blog = _service.Get(id.Value);
        return Blog == null ? NotFound() : Page();
    }
}
