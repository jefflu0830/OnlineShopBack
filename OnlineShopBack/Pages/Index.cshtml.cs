using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineShopBack.Tool;
using System;

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
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||  //判斷Session[Account]是否為空
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].SId != HttpContext.Session.Id ||//判斷DB SessionId與瀏覽器 SessionId是否一樣
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].ValidTime < DateTime.Now)//判斷是否過期
            {
                Response.Redirect("/Login");                
            }

            Roles = HttpContext.Session.GetString("Roles");


    }
    }
}
