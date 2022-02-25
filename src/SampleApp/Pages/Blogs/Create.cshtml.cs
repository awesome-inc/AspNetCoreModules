#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleApp.Api;

namespace SampleApp.Pages.Blogs
{
    public class CreateModel : PageModel
    {
        private readonly IBlogService _service;

        public CreateModel(IBlogService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public BlogDto Blog { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();
            _service.Upsert(Blog);
            return RedirectToPage("./Index");
        }
    }
}
