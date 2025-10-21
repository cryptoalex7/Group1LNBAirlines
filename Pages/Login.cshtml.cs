using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace LNBAirlinesGroup1.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        // Demo users - In a real application, this would come from a database
        private readonly Dictionary<string, UserInfo> _users = new()
        {
            { "john.smith", new UserInfo { Password = "password123", Name = "John Smith", Role = "Flight Attendant", EmployeeId = "FA001" } },
            { "sarah.johnson", new UserInfo { Password = "password123", Name = "Sarah Johnson", Role = "Gate Agent", EmployeeId = "GA002" } },
            { "mike.davis", new UserInfo { Password = "password123", Name = "Mike Davis", Role = "Ground Crew", EmployeeId = "GC003" } },
            { "admin", new UserInfo { Password = "admin123", Name = "System Administrator", Role = "Administrator", EmployeeId = "ADM001" } }
        };

        public IActionResult OnGet()
        {
            // Handle logout
            if (Request.Query["action"] == "logout")
            {
                HttpContext.Session.Clear();
                ViewData["LogoutMessage"] = "You have been successfully signed out.";
                return Page();
            }

            // Check if user is already logged in
            if (IsUserLoggedIn())
            {
                return RedirectToPage("/");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validate credentials
            if (_users.TryGetValue(Username, out var user) && user.Password == Password)
            {
                // Set session variables
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("Username", Username);
                HttpContext.Session.SetString("FullName", user.Name);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("EmployeeId", user.EmployeeId);

                // Redirect to dashboard
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Invalid username or password. Please try again.";
            return Page();
        }

        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetString("IsAuthenticated") == "true";
        }

        private class UserInfo
        {
            public string Password { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public string EmployeeId { get; set; } = string.Empty;
        }
    }
}
