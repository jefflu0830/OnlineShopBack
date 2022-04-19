using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Models;
using OnlineShopBack.Services;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            SqlCommand cmd = null;
            DataTable dt = new DataTable();

            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                SqlDataAdapter da = new SqlDataAdapter();

                cmd.CommandText = @"EXEC pro_onlineShopBack_selectAccount @f_acc, @f_pwd";

                cmd.Parameters.AddWithValue("@f_acc", value.Account);
                cmd.Parameters.AddWithValue("@f_pwd", PswToMD5(value.Pwd));

                //開啟連線
                cmd.Connection.Open();

                da.SelectCommand = cmd;
                da.Fill(dt);
                cmd.Connection.Close();
                

                if (dt.Rows.Count == 0)
                {
                    return "帳號密碼錯誤";
                }
                else
                {
                    return "登入成功";
                }

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }



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
            
        }

        [HttpDelete]
        public void logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        [HttpGet("NoLogin")]
        public string noLogin()
        {
            return "未登入";
        }

        //MD5 加密
        public static string PswToMD5(string pwd)
        {
            var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.ASCII.GetBytes(pwd));
            var strResult = BitConverter.ToString(result);
            var md5Pwd = strResult.Replace("-", "");
            return md5Pwd;
        }
    }
}
