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
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System;
using System.Data;
using OnlineShopBack.Enum;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase"); //SQL連線字串  SQLConnectionString

        //登入
        [HttpPost]
        public string login(LoginDto value)
        {
            string ResultCode = "";

            //查詢伺服器狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "輸入參數有誤";
            }

            //if (User.Identity.IsAuthenticated)
            //{
            //    return "請先登出再進行登入";
            //}

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
                MyTool.WriteErroLog(loginErrorStr);
                ResultCode = ((int)LoginEnum.LoginReturnCode.BackEndError).ToString();
            }
            else
            {
                SqlCommand cmd = null;
                SqlDataAdapter da = new SqlDataAdapter();

                try
                {
                    DataTable dt = new DataTable();

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
                        ResultCode = ((int)LoginEnum.LoginReturnCode.AccOrPwdError).ToString();
                        //return "登入失敗"; //登入失敗
                    }
                    else //驗證成功
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
                        //添加 可使用商品管理
                        if ((bool)dt.Rows[0]["f_canUseProduct"])
                        {
                            Roles += "canUseProduct/";
                        };
                        //添加 可使用訂單管理
                        if ((bool)dt.Rows[0]["f_canUseOrder"])
                        {
                            Roles += "canUseOrder/";
                        };

                        //Session傳遞
                        HttpContext.Session.SetString("Account", value.Account);
                        HttpContext.Session.SetString("AccPosition", dt.Rows[0]["f_accPosition"].ToString());
                        HttpContext.Session.SetString("Roles", Roles);
                        //關閉清空連線
                        cmd.Connection.Close();
                        cmd.Parameters.Clear();

                        DataTable SessionIdDt = new DataTable();

                        //登入成功 紀錄session 在DB中
                        cmd.CommandText = @" EXEC pro_onlineShopBack_putAccountBySessionId @sessionId, @f_acc, @LogInOrLogOut ";
                        cmd.Parameters.AddWithValue("@sessionId", HttpContext.Session.Id);
                        cmd.Parameters.AddWithValue("@f_acc", value.Account);
                        cmd.Parameters.AddWithValue("@LogInOrLogOut", true); //登入(1)or登出(0)
                                                                             //開啟連線
                        cmd.Connection.Open();
                        da.SelectCommand = cmd;
                        da.Fill(SessionIdDt);

                        ResultCode = SessionIdDt.Rows[0]["ReturnCode"].ToString();

                        SessionDB.SessionInfo SessionInfo = new SessionDB.SessionInfo();
                        SessionInfo.SId = SessionIdDt.Rows[0]["f_sessionId"].ToString(); //updata完後的sessionId
                        SessionInfo.ValidTime = DateTime.Now.AddMinutes(30);//失效時間 =(現在時間在加30分鐘)

                        SessionDB.sessionDB.AddOrUpdate(HttpContext.Session.GetString("Account"),
                                                        SessionInfo,
                                                       (key, oldValue) => oldValue = SessionInfo);
                    }
                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
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

            return ResultCode;
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
            SqlCommand cmd = null;
            try
            {

                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                //清空DB的sessionId
                cmd.CommandText = @" EXEC pro_onlineShopBack_putAccountBySessionId @sessionId, @f_acc, @LogInOrLogOut ";
                cmd.Parameters.AddWithValue("@sessionId", "");
                cmd.Parameters.AddWithValue("@f_acc", HttpContext.Session.GetString("Account"));
                cmd.Parameters.AddWithValue("@LogInOrLogOut", false); //登入(1)or登出(0)
                //開啟連線
                cmd.Connection.Open();
                cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Connection.Close();
                    cmd.Parameters.Clear();
                }
            }

            //清空Dictionary & Session[Account]中的值
            SessionDB.sessionDB.TryRemove(HttpContext.Session.GetString("Account"), out _);
            HttpContext.Session.Clear();

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
