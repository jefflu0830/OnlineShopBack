using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        [HttpPost]
        public string login(AccountSelectDto value)
        {

            using (var md5 = MD5.Create())
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
            }
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
    }
}
