using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineShopBack.Models;
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly OnlineShopContext _OnlineShopContext;
        public LoginController(OnlineShopContext onlineShopContext)
        {
            _OnlineShopContext = onlineShopContext;
        }

        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase"); //SQL連線字串  SQLConnectionString

        [HttpPost]
        public string login(AccountSelectDto value)
        {

            string loginErrorStr = "";//記錄錯誤訊息

            //帳號資料驗證
            if (value.Account == "" || (string.IsNullOrEmpty(value.Account)))
            {
                loginErrorStr += "[帳號不可為空]\n";
            }
            else
            {
                if (!MyTool.IsENAndNumber(value.Account))
                {
                    loginErrorStr += "[＊帳號只能為英數]\n";
                }
                if (value.Account.Length > 20 || value.Account.Length < 3)
                {
                    loginErrorStr += "[＊帳號長度應介於3～20個數字之間]\n";
                }
            };

            //密碼資料驗證
            if (value.Pwd == "" || (string.IsNullOrEmpty(value.Pwd)))
            {
                loginErrorStr += "[密碼不可為空]\n";
            }
            else
            {
                if (!MyTool.IsENAndNumber(value.Pwd))
                {
                    loginErrorStr += "[＊密碼只能為英數]\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    loginErrorStr += "[＊密碼長度應應介於8～16個數字之間]\n";
                }
            }

            //錯誤訊息不為空
            if (loginErrorStr != "")
            {
                return loginErrorStr;
            }
            else
            {
                SqlCommand cmd = null;
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter();

                try
                {


                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel @f_acc, @f_pwd";

                    cmd.Parameters.AddWithValue("@f_acc", value.Account);
                    cmd.Parameters.AddWithValue("@f_pwd", MyTool.PswToMD5(value.Pwd));


                    //開啟連線
                    cmd.Connection.Open();

                    if (cmd.ExecuteScalar() == null)
                    {
                        return "loginFail"; //登入失敗
                    }
                    else
                    {
                        da.SelectCommand = cmd;
                        da.Fill(dt);



                        //Cookie 驗證
                        var claims = new List<Claim>
                        {
                           new Claim(ClaimTypes.Name, value.Account)
                        };

                        if (dt.Rows[0]["f_canUseAccount"].ToString() == "True")
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "canUseAccount"));
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));



                        return "loginOK";  //登入OK
                    }

                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Connection.Close();
                        cmd.Parameters.Clear();

                    }
                }
            }

            #region  EF舊寫法已註解
            /*using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(value.Pwd));//MD5 加密傳密碼進去
                                                                                 //
                var strResult = BitConverter.ToString(result);

                var user = (from a in _OnlineShopContext.TAccount
                            where a.FAcc == value.Account
                            && a.FPwd == strResult.Replace("-", "")
                            select a).SingleOrDefault();

                if (user == null)
                {
                    return "帳號密碼錯誤";
                }
                else
                {
                    //這邊等等寫驗證
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FAcc),
                    //new Claim("FullName", user.FName),
                   // new Claim(ClaimTypes.Role, "Administrator")
                };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return "OK";
                }
            }*/
            #endregion

        }

        [HttpDelete("Logout")]
        public void logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        [HttpGet("NoLogin")]
        public string noLogin()
        {
            return "未登入";
        }
    }
}
