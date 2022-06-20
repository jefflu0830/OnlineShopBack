using Microsoft.Data.SqlClient;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Domain.DTOs.Account;
using OnlineShopBack.Domain.Tool;
using OnlineShopBack.Domain.Enum;
using System;
using System.Data;

namespace OnlineShopBack.Persistent
{


    public class AccountRepository : IAccountRepository
    {
        /*-----------後台帳號相關-----------*/

        //取得全部後台帳號清單
        public DataTable GetAccountAndLevelList(string SQLConnectionString)
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" EXEC pro_onlineShopBack_getAccountAndAccountLevelList ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);

            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
            }
            finally
            {
                //關閉連線
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            return dt;
        }        
        //新增後台帳號  
        public int AddAccount(string SQLConnectionString, AccountDto value)
        {
            //後端驗證
            string addAccErrorStr = "";//記錄錯誤訊息
            

            //帳號資料驗證
            if (string.IsNullOrEmpty(value.Account))
            {
                addAccErrorStr += "[帳號不可為空]\n";
            }
            else
            {
                if (!MyTool.IsENAndNumber(value.Account))
                {
                    addAccErrorStr += "[帳號只能為英數]\n";
                }
                if (value.Account.Length > 20 || value.Account.Length < 3)
                {
                    addAccErrorStr += "[帳號長度應介於3～20個數字之間]\n";
                }
            }

            //密碼資料驗證
            if (string.IsNullOrEmpty(value.Pwd))//空字串判斷and Null值判斷皆用IsNullOrEmpty
            {
                addAccErrorStr += "[密碼不可為空]\n";
            }
            else
            {
                if (!MyTool.IsENAndNumber(value.Pwd))
                {
                    addAccErrorStr += "[密碼只能為英數]\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    addAccErrorStr += "[密碼長度應應介於8～16個數字之間]\n";
                }
            }

            //權限資料驗證
            if (value.Level > 255 || value.Level < 0)
            {
                addAccErrorStr += "[該權限不再範圍內]\n";
            }

            //回傳碼
            int ResultCode = (int)AccountEnum.AddAccountCode.Defult;

            if (!string.IsNullOrEmpty(addAccErrorStr))
            {
                MyTool.WriteErroLog(addAccErrorStr);
                ResultCode = (int)AccountEnum.AddAccountCode.ValidaFail;
            }
            else
            {

                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_addAccount @f_acc, @f_pwd, @f_level";

                    cmd.Parameters.AddWithValue("@f_acc", value.Account);
                    cmd.Parameters.AddWithValue("@f_pwd", MyTool.PswToMD5(value.Pwd));
                    cmd.Parameters.AddWithValue("@f_level", value.Level);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//.ToString();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)AccountEnum.AddAccountCode.ExceptionError;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
                }
            }

            return ResultCode;
        }      
        //編輯後台帳號權限
        public int EditAcc(string SQLConnectionString, int id, AccountDto value)
        {
            string addAccErrorStr = "";//記錄錯誤訊息
            int ResultCode = (int)AccountEnum.EditAccCode.Defult;
            //權限資料驗證
            if (value.Level > 255 || value.Level < 0)
            {
                addAccErrorStr += "[該權限不再範圍內]\n";
            }

            if (!string.IsNullOrEmpty(addAccErrorStr))
            {
                MyTool.WriteErroLog(addAccErrorStr);
                ResultCode = (int)AccountEnum.EditAccCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_putAccountByLevel @Id, @Level";

                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Level", value.Level);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)AccountEnum.EditAccCode.ExceptionError;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
                }
            }
            return ResultCode;
        }
        //後台帳號編輯密碼
        public int EditPwd(string SQLConnectionString, PutPwdDto value)
        {
            string addAccErrorStr = "";//記錄錯誤訊息
            int ResultCode = (int)AccountEnum.EditAccPwdCode.Defult;
            //密碼資料驗證
            if (string.IsNullOrEmpty(value.newPwd) || string.IsNullOrEmpty(value.cfmNewPwd))//空字串判斷and Null值判斷皆用IsNullOrEmpty
            {
                addAccErrorStr += "[新密碼或確認密碼不可為空]\n";
            }
            else
            {
                if (value.newPwd != value.cfmNewPwd)//空字串判斷and Null值判斷皆用IsNullOrEmpty
                {
                    addAccErrorStr += "[新密碼與確認新密碼需相同]\n";
                }

                if (!MyTool.IsENAndNumber(value.newPwd) || !MyTool.IsENAndNumber(value.cfmNewPwd))
                {
                    addAccErrorStr += "[密碼只能為英數]\n";
                }
                if (value.newPwd.Length > 16 || value.newPwd.Length < 8)
                {
                    addAccErrorStr += "[密碼長度應應介於8～16個數字之間]\n";
                }
            }

            if (!string.IsNullOrEmpty(addAccErrorStr))
            {
                MyTool.WriteErroLog(addAccErrorStr);
                ResultCode = (int)AccountEnum.EditAccPwdCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                string SQLReturnCode = "";
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_putAccountByPwd @id, @newPwd, @cfmNewPwd";

                    cmd.Parameters.AddWithValue("@id", value.id);
                    cmd.Parameters.AddWithValue("@newPwd", MyTool.PswToMD5(value.newPwd));
                    cmd.Parameters.AddWithValue("@cfmNewPwd", MyTool.PswToMD5(value.cfmNewPwd));

                    //開啟連線
                    cmd.Connection.Open();
                    SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)AccountEnum.EditAccPwdCode.ExceptionError;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
                }                
            }
            return ResultCode;
        }
        //刪除後臺帳號
        public int DelAcc(string SQLConnectionString, int id)
        {
            SqlCommand cmd = null;
            int ResultCode = (int)AccountEnum.DelAccCode.Defult;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_delAccount @f_acc";

                cmd.Parameters.AddWithValue("@f_acc", id);

                //開啟連線
                cmd.Connection.Open();
                ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
                ResultCode = (int)AccountEnum.DelAccCode.ExceptionError;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            return ResultCode;
        }

        /*-----------後台帳號權限相關-----------*/

        //取得全部後台權限清單
        public DataTable GetAccLvList(string SQLConnectionString) {

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" EXEC pro_onlineShopBack_getAccountLevel ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
            }
            finally
            {
                //關閉連線
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            return dt;
        }

        public DataTable GetAccLvById(string SQLConnectionString,int id)
        {
            string addAccLVErrorStr = "";//記錄錯誤訊息

            if (id > 255 || id < 0)
            {
                addAccLVErrorStr += "[編號長度應介於0～255個數字之間]\n";
            }
            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(addAccLVErrorStr))
            {
                MyTool.WriteErroLog("後端驗證失敗 : "+addAccLVErrorStr);
            }

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();


            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" EXEC pro_onlineShopBack_getAccountLevelById @accLevel ";
                cmd.Parameters.AddWithValue("@accLevel", id);

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception e)
            {
                MyTool.WriteErroLog("例外錯誤 : "+e.Message);
            }
            finally
            {
                //關閉連線
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            return dt;
        }
    }
}
