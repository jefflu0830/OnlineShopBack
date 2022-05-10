using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Models;
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System;
using System.Collections.Generic;
using System.Data;

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

        //登入
        [HttpPost]
        public string login(AccountDto value)
        {

            //查詢伺服器狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "輸入參數有誤";
            }

            if (User.Identity.IsAuthenticated)
            {
                return "請先登出再進行登入";
            }

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
                    else //登入成功
                    {



                        da.SelectCommand = cmd;
                        da.Fill(dt);

                        string Roles = "";

                        //添加 可使用帳號管理
                        if ((bool)dt.Rows[0]["f_canUseAccount"])
                        {
                            Roles += "canUseAccount/";
                        };
                        //添加 可使用會員管理
                        if ((bool)dt.Rows[0]["f_canUseMember"])
                        {
                            Roles += "canUseMember/";
                        };

                        //Session傳遞
                        HttpContext.Session.SetString("Account", value.Account);
                        HttpContext.Session.SetString("AccPosition", dt.Rows[0]["f_accPosition"].ToString());
                        HttpContext.Session.SetString("Roles", Roles);


                        //if (SessionDB.sessionDB.ContainsKey(value.Account) &&
                        //    SessionDB.sessionDB[value.Account] != HttpContext.Session.Id)
                        //{
                        //    SessionDB.sessionDB.Remove(value.Account);
                        //    SessionDB.sessionDB.Add(value.Account, HttpContext.Session.Id);
                        //    return "有使用者正在使用此帳號";
                        //}
                        //else
                        //{
                        //    SessionDB.sessionDB.Add(value.Account, HttpContext.Session.Id);
                        //    return "loginOK";  //登入OK
                        //}



                        //資料庫中 Account為空 or 存的sessionId與現在的不符
                        if (!string.IsNullOrWhiteSpace(dt.Rows[0]["f_sessionId"].ToString()) &&
                            dt.Rows[0]["f_sessionId"].ToString() != HttpContext.Session.Id)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = @"UPDATE t_account WITH(ROWLOCK) SET f_sessionId = @sessionId WHERE f_acc = @f_acc ";
                            cmd.Parameters.AddWithValue("@sessionId", HttpContext.Session.Id);
                            cmd.Parameters.AddWithValue("@f_acc", value.Account);
                            cmd.ExecuteScalar();

                            SessionDB.SessionId = dt.Rows[0]["f_sessionId"].ToString();



                            return "重複登入";  //重複登入
                        }
                        else
                        {
                            //登入成功 紀錄session 在DB中
                            cmd.Parameters.Clear();
                            cmd.CommandText = @"UPDATE t_account WITH(ROWLOCK) SET f_sessionId = @sessionId WHERE f_acc = @f_acc ";
                            cmd.Parameters.AddWithValue("@sessionId", HttpContext.Session.Id);
                            cmd.Parameters.AddWithValue("@f_acc", value.Account);
                            cmd.ExecuteScalar();

                            return "loginOK";  //登入OK
                        }
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


            //HttpContext.Session.GetString("Account");
            //Session.Remove("Account")

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
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {

                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                //清空sessionId
                cmd.CommandText = @"UPDATE t_account WITH(ROWLOCK) SET f_sessionId = '' WHERE f_acc = @f_acc ";
                cmd.Parameters.AddWithValue("@f_acc", HttpContext.Session.GetString("Account"));
                //開啟連線
                cmd.Connection.Open();
                cmd.ExecuteScalar();
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Connection.Close();
                    cmd.Parameters.Clear();

                }
            }

            //SessionDB.sessionDB.Remove(HttpContext.Session.GetString("Account"));
            HttpContext.Session.Remove("Account");

        }
        [HttpGet("NoLogin")]
        public string noLogin()
        {
            return "未登入";
        }
    }
}
