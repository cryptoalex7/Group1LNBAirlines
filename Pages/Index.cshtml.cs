using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LNBAirlinesGroup1.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        // Check if user is authenticated
        if (HttpContext.Session.GetString("IsAuthenticated") != "true")
        {
            return RedirectToPage("/Login");
        }

        return Page();
    }
}
