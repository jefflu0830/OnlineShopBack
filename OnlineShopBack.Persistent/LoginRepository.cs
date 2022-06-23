using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using OnlineShopBack.Domain.DTOs;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Domain.Tool;
using OnlineShopBack.Enum;
using System;
using System.Data;

namespace OnlineShopBack.Persistent
{

    public class LoginRepository : ILoginRepository
    {

        private HttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
        private readonly string _SQLConnectionString = null;//SQL連線字串

        public LoginRepository(IConfigHelperRepository configHelperRepository)
        {
            //SQL連線字串
            _SQLConnectionString = configHelperRepository.SQLConnectionStrings();
        }

        public int Login(LoginDto value)
        {
            string loginErrorStr = "";//記錄錯誤訊息
            int ResultCode = (int)LoginEnum.LoginReturnCode.Defult;

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
                ResultCode = (int)LoginEnum.LoginReturnCode.ValidaFail;
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
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel @f_acc, @f_pwd";

                    cmd.Parameters.AddWithValue("@f_acc", value.Account);
                    cmd.Parameters.AddWithValue("@f_pwd", MyTool.PswToMD5(value.Pwd));

                    //開啟連線
                    cmd.Connection.Open();

                    if (cmd.ExecuteScalar() == null)
                    {
                        ResultCode = (int)LoginEnum.LoginReturnCode.AccOrPwdError;
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
                        _httpContextAccessor.HttpContext.Session.SetString("Account", value.Account);

                        _httpContextAccessor.HttpContext.Session.SetString("AccPosition", dt.Rows[0]["f_accPosition"].ToString());
                        _httpContextAccessor.HttpContext.Session.SetString("Roles", Roles);
                        //關閉清空連線
                        cmd.Connection.Close();
                        cmd.Parameters.Clear();

                        DataTable SessionIdDt = new DataTable();

                        //登入成功 紀錄session 在DB中
                        cmd.CommandText = @" EXEC pro_onlineShopBack_putAccountBySessionId @sessionId, @f_acc, @LogInOrLogOut ";
                        cmd.Parameters.AddWithValue("@sessionId", _httpContextAccessor.HttpContext.Session.Id);
                        cmd.Parameters.AddWithValue("@f_acc", value.Account);
                        cmd.Parameters.AddWithValue("@LogInOrLogOut", true); //登入(1)or登出(0)
                        //開啟連線
                        cmd.Connection.Open();
                        da.SelectCommand = cmd;
                        da.Fill(SessionIdDt);

                        ResultCode = (int)SessionIdDt.Rows[0]["ReturnCode"];

                        SessionDB.SessionInfo SessionInfo = new SessionDB.SessionInfo();
                        SessionInfo.SId = SessionIdDt.Rows[0]["f_sessionId"].ToString(); //updata完後的sessionId
                        SessionInfo.ValidTime = DateTime.Now.AddMinutes(30);//失效時間 =(現在時間在加30分鐘)

                        SessionDB.sessionDB.AddOrUpdate(_httpContextAccessor.HttpContext.Session.GetString("Account"),
                                                        SessionInfo,
                                                       (key, oldValue) => oldValue = SessionInfo);
                    }
                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)LoginEnum.LoginReturnCode.ExceptionError;
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

        public void LogOut()
        {
            SqlCommand cmd = null;
            try
            {

                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
                //清空DB的sessionId
                cmd.CommandText = @" EXEC pro_onlineShopBack_putAccountBySessionId @sessionId, @f_acc, @LogInOrLogOut ";
                cmd.Parameters.AddWithValue("@sessionId", "");
                cmd.Parameters.AddWithValue("@f_acc", _httpContextAccessor.HttpContext.Session.GetString("Account"));
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
            SessionDB.sessionDB.TryRemove(_httpContextAccessor.HttpContext.Session.GetString("Account"), out _);
            _httpContextAccessor.HttpContext.Session.Clear();

        }
    }
}
