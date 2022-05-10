using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineShopBack.Pages.Account
{
    public class AddMemberLevelModel : PageModel
    {
        public void OnGet()
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")))
            {
                Response.Redirect("/Login");
                return;
            }
            else if (!HttpContext.Session.GetString("Roles").Contains("canUseMember"))
            {
                Response.Redirect("/index");
                return;
            }
        }
    }
}
