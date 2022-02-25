#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleApp.Api;

namespace SampleApp.Pages.Blogs
{
    public class EditModel : PageModel
    {
        private readonly IBlogService _service;

        public EditModel(IBlogService service)
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

            if (Blog == null)
                return NotFound();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _service.Upsert(Blog);

            return RedirectToPage("./Index");
        }
    }
}
