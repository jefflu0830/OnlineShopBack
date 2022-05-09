using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.Mime.MediaTypeNames;

namespace OnlineShopBack.Pages
{
    public class BackPageModel : PageModel
    {
        public string Session01;
        public void OnGet()
        {
            if (HttpContext.Session.Keys.ToString() == null || 
                string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")))
            {
                Response.Redirect("/Login");
            }

        }
    }
}
