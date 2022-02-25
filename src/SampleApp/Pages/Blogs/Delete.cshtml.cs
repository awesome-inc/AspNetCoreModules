#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleApp.Api;

namespace SampleApp.Pages.Blogs
{
    public class DeleteModel : PageModel
    {
        private readonly IBlogService _service;

        public DeleteModel(IBlogService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [BindProperty]
        public BlogDto Blog { get; set; }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
                return NotFound();
            Blog = _service.Get(id.Value);
            return Blog == null ? NotFound() : Page();
        }

        public IActionResult OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();
            _service.Remove(id.Value);
            return RedirectToPage("./Index");
        }
    }
}
