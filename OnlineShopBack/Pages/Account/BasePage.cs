using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System.Data;

namespace OnlineShopBack.Pages.Account
{
    public class BasePage
    {
        public class BasePageModel : PageModel
        {
            //驗證登入&&腳色
            public void AccountValidate()
            {
                if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")))
                {
                    Response.Redirect("/Login");
                    return;
                }
                else if (!HttpContext.Session.GetString("Roles").Contains("canUseAccount"))
                {
                    Response.Redirect("/index");
                    return;
                }
            }
        }
    }
}