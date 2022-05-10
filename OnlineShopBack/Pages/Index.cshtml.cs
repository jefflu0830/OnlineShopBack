using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineShopBack.Tool;

namespace OnlineShopBack.Pages
{
    public class indexModel : PageModel
    {
        public string Roles;

        public void OnGet()
        {
            Roles = "";

            //if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||
            //    SessionDB.sessionDB[HttpContext.Session.GetString("Account")] != HttpContext.Session.Id)

            //session("account")不存在 or 資料庫sessionId 與 瀏覽器sessionId不符
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account"))||
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")] != HttpContext.Session.Id)
            {
                Response.Redirect("/Login");
            }

            Roles = HttpContext.Session.GetString("Roles");


    }
    }
}
