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

            //session("account")������ or �Y�ώ�sessionId �c �g�[��sessionId����
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||  //�Д�Session[Account]�Ƿ���
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].SId != HttpContext.Session.Id ||//�Д�DB SessionId�c�g�[�� SessionId�Ƿ�һ��
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].ValidTime < DateTime.Now)//�Д��Ƿ��^��
            {
                Response.Redirect("/Login");                
            }

            Roles = HttpContext.Session.GetString("Roles");


    }
    }
}
