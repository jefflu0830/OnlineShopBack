#region 功能與歷史修改描述
/*
    描述:登入系統相關
    日期:2022-05-05
*/
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Services;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase"); //SQL連線字串  SQLConnectionString

        private readonly ILoginRepository _LoginService = null;
        public LoginController(ILoginRepository accountService)
        {
            _LoginService = accountService;
        }


        //登入
        [HttpPost]
        public string login(Domain.DTOs.LoginDto value)
        {


            //查詢伺服器狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "輸入參數有誤";
            }

            int ResultCode = _LoginService.Login(value);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //登出
        [HttpDelete("Logout")]
        public void logout()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return;
            }
            _LoginService.LogOut();

        }

        private bool loginValidate()
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||                        //判斷Session[Account]是否為空
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].SId != HttpContext.Session.Id ||//判斷DB SessionId與瀏覽器 SessionId是否一樣
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].ValidTime < DateTime.Now)       //判斷是否過期
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
