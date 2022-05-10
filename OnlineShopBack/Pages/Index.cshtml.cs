using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineShopBack.Tool;

namespace OnlineShopBack.Pages
{
    public class BackPageModel : BasePage
    {
        public string Roles;

        public void OnGet()
        {
            Roles = "";

            //if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||
            //    SessionDB.sessionDB[HttpContext.Session.GetString("Account")] != HttpContext.Session.Id)
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||
                SessionDB.SessionId != HttpContext.Session.Id)
            {
                Response.Redirect("/Login");
                return;
            }

            Roles = HttpContext.Session.GetString("Roles");


    }
    }
}
