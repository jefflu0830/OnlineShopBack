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

            //session("account")������ or �Y�ώ�sessionId �c �g�[��sessionId����
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account"))||
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")] != HttpContext.Session.Id)
            {
                Response.Redirect("/Login");
            }

            Roles = HttpContext.Session.GetString("Roles");


    }
    }
}
