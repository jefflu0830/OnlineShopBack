using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace OnlineShopBack.Pages
{
    public class IndexModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

            if (HttpContext.Session.GetString("Account") != null ||
               !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")))
            {
                Response.Redirect("/index");
            }

        }
    }
}
