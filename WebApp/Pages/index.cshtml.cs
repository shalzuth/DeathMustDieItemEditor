using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public bool Subscriber { get; set; }
        public IndexModel(IHttpContextAccessor httpContextAccessor, ILogger<IndexModel> logger)
        {
            Subscriber = true;
        }

        public void OnGet()
        {

        }
    }
}
