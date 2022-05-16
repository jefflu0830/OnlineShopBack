using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineShopBack.Tool;
using System;
using System.Data;

namespace OnlineShopBack.Pages.Product
{
    public class BasePage
    {
        public class BasePageModel : PageModel
        {
            //驗證登入
            public bool LoginValidate()
            {
                if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||  //判斷Session[Account]是否為空
                    SessionDB.sessionDB[HttpContext.Session.GetString("Account")].SId != HttpContext.Session.Id ||//判斷DB SessionId與瀏覽器 SessionId是否一樣
                    SessionDB.sessionDB[HttpContext.Session.GetString("Account")].ValidTime < DateTime.Now)//判斷是否過期
                {
                    TempData["message"] = "此帳號已從另一地點登入,稍後轉跳至登入頁面";
                    //Response.Redirect("/Login");
                    return false;

                }
                else
                {
                    return true;
                }
            }

            //驗證腳色
            public bool RolesValidate()
            {


                if (!HttpContext.Session.GetString("Roles").Contains("canUseProduct"))
                {
                    TempData["message"] = "無使用權限";
                    //Response.Redirect("/index");
                    return false;

                }
                else
                {
                    return true;
                }
            }
        }
    }
}
