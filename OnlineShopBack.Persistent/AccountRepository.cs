using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Domain.DTOs.Account;
using OnlineShopBack.Domain.Enum;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;

namespace OnlineShopBack.Persistent
{
    public class AccountRepository : IAccountRepository
    {

        private readonly string _SQLConnectionString = null;//SQL連線字串
        public AccountRepository(IConfigHelperRepository configHelperRepository)
        {
            //SQL連線字串
            _SQLConnectionString = configHelperRepository.SQLConnectionStrings();
        }

        /*-----------後台帳號相關-----------*/

        //取得全部後台帳號清單
        public DataTable GetAccountAndLevelList()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
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
        public int AddAccount(AccountDto value)
        {
            //後端驗證
            string ValidaFailString = "";//記錄錯誤訊息


            //帳號資料驗證
            if (string.IsNullOrEmpty(value.Account))
            {
                ValidaFailString += "[帳號不可為空]\n";
            }
            else
            {
                if (!MyTool.IsENAndNumber(value.Account))
                {
                    ValidaFailString += "[帳號只能為英數]\n";
                }
                if (value.Account.Length > 20 || value.Account.Length < 3)
                {
                    ValidaFailString += "[帳號長度應介於3～20個數字之間]\n";
                }
            }

            //密碼資料驗證
            if (string.IsNullOrEmpty(value.Pwd))//空字串判斷and Null值判斷皆用IsNullOrEmpty
            {
                ValidaFailString += "[密碼不可為空]\n";
            }
            else
            {
                if (!MyTool.IsENAndNumber(value.Pwd))
                {
                    ValidaFailString += "[密碼只能為英數]\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    ValidaFailString += "[密碼長度應應介於8～16個數字之間]\n";
                }
            }

            //權限資料驗證
            if (value.Level > 255 || value.Level < 0)
            {
                ValidaFailString += "[該權限不再範圍內]\n";
            }

            //回傳碼
            int ResultCode = (int)AccountEnum.AddAccountCode.Defult;

            if (!string.IsNullOrEmpty(ValidaFailString))
            {
                MyTool.WriteErroLog(ValidaFailString);
                ResultCode = (int)AccountEnum.AddAccountCode.ValidaFail;
            }
            else
            {

                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

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
        public int EditAcc(int id, AccountDto value)
        {
            string ValidaFailString = "";//記錄錯誤訊息
            int ResultCode = (int)AccountEnum.EditAccCode.Defult;
            //權限資料驗證
            if (value.Level > 255 || value.Level < 0)
            {
                ValidaFailString += "[該權限不再範圍內]\n";
            }

            if (!string.IsNullOrEmpty(ValidaFailString))
            {
                MyTool.WriteErroLog(ValidaFailString);
                ResultCode = (int)AccountEnum.EditAccCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

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
        public int EditPwd(PutPwdDto value)
        {
            string ValidaFailString = "";//記錄錯誤訊息
            int ResultCode = (int)AccountEnum.EditAccPwdCode.Defult;
            //密碼資料驗證
            if (string.IsNullOrEmpty(value.newPwd) || string.IsNullOrEmpty(value.cfmNewPwd))//空字串判斷and Null值判斷皆用IsNullOrEmpty
            {
                ValidaFailString += "[新密碼或確認密碼不可為空]\n";
            }
            else
            {
                if (value.newPwd != value.cfmNewPwd)//空字串判斷and Null值判斷皆用IsNullOrEmpty
                {
                    ValidaFailString += "[新密碼與確認新密碼需相同]\n";
                }

                if (!MyTool.IsENAndNumber(value.newPwd) || !MyTool.IsENAndNumber(value.cfmNewPwd))
                {
                    ValidaFailString += "[密碼只能為英數]\n";
                }
                if (value.newPwd.Length > 16 || value.newPwd.Length < 8)
                {
                    ValidaFailString += "[密碼長度應應介於8～16個數字之間]\n";
                }
            }

            if (!string.IsNullOrEmpty(ValidaFailString))
            {
                MyTool.WriteErroLog(ValidaFailString);
                ResultCode = (int)AccountEnum.EditAccPwdCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_putAccountByPwd @id, @newPwd, @cfmNewPwd";

                    cmd.Parameters.AddWithValue("@id", value.id);
                    cmd.Parameters.AddWithValue("@newPwd", MyTool.PswToMD5(value.newPwd));
                    cmd.Parameters.AddWithValue("@cfmNewPwd", MyTool.PswToMD5(value.cfmNewPwd));

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

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
        public int DelAcc(int id)
        {
            SqlCommand cmd = null;
            int ResultCode = (int)AccountEnum.DelAccCode.Defult;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);

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
        public DataTable GetAccLvList()
        {

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
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
        //依照後臺帳號id取得該後台帳號資料
        public DataTable GetAccLvById(int id)
        {
            string ValidaFailString = "";//記錄錯誤訊息

            if (id > 255 || id < 0)
            {
                ValidaFailString += "[編號長度應介於0～255個數字之間]\n";
            }
            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ValidaFailString))
            {
                MyTool.WriteErroLog("後端驗證失敗 : " + ValidaFailString);
            }

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();


            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
                cmd.CommandText = @" EXEC pro_onlineShopBack_getAccountLevelById @accLevel ";
                cmd.Parameters.AddWithValue("@accLevel", id);

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception e)
            {
                MyTool.WriteErroLog("例外錯誤 : " + e.Message);
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

        //新增後臺帳號權限
        public int AddAccLv(AccountLevelDto value)
        {
            string ValidaFailString = "";//記錄錯誤訊息
            int ResultCode = (int)AccountEnum.AddAccountLVCode.Defult;
            //權限編號 
            if (value.accLevel == null)
            {
                ValidaFailString += "[編號不可為空]\n";
            }
            else
            {
                if (value.accLevel > 255 || value.accLevel < 0)
                {
                    ValidaFailString += "[編號長度應介於0～255個數字之間]\n";
                }
            }
            //權限名稱
            if (string.IsNullOrEmpty(value.accPosition))
            {
                ValidaFailString += "[權限名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.accPosition))
                {
                    ValidaFailString += "[名稱應為中文,英文及數字]\n";
                }
                if (value.accPosition.Length > 10 || value.accPosition.Length < 0)
                {
                    ValidaFailString += "[名稱應介於0～10個字之間]\n";
                }
            }
            //是否有權使用帳號管理 or 會員管理
            if (value.canUseAccount == null || value.canUseMember == null ||
               (value.canUseAccount > 1 || value.canUseAccount < 0) ||
               (value.canUseMember > 1 || value.canUseMember < 0) ||
               (value.canUseOrder > 1 || value.canUseOrder < 0))
            {
                ValidaFailString += "[選擇權限格式錯誤]\n";
            }

            //錯誤訊息有值 return後端監測失敗代碼,log記錄錯誤項目
            if (!string.IsNullOrEmpty(ValidaFailString))
            {
                MyTool.WriteErroLog(ValidaFailString);
                ResultCode = (int)AccountEnum.AddAccountLVCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    //重複驗證寫在SP中
                    cmd.CommandText = @"EXEC pro_onlineShopBack_addAccountLevel @accLevel, @accPosission, @canUseAccount, @canUseMember, @canUseProduct, @canUseOrder ";

                    cmd.Parameters.AddWithValue("@accLevel", value.accLevel);
                    cmd.Parameters.AddWithValue("@accPosission", value.accPosition);
                    cmd.Parameters.AddWithValue("@canUseAccount", value.canUseAccount);
                    cmd.Parameters.AddWithValue("@canUseMember", value.canUseMember);
                    cmd.Parameters.AddWithValue("@canUseProduct", value.canUseProduct);
                    cmd.Parameters.AddWithValue("@canUseOrder", value.canUseOrder);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)AccountEnum.AddAccountLVCode.ExceptionError;
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

        //後臺帳號權限編輯
        public int EditAccLv(int id, AccountLevelDto value)
        {
            int ResultCode = (int)AccountEnum.EditAccLvCode.Defult;

            string ValidaFailString = "";//記錄錯誤訊息
            //權限編號 
            if (id == 0)
            {
                ValidaFailString += "此權限編號為最高權限無法更改\n";
            }
            if (id > 255 || id < 0)
            {
                ValidaFailString += "[編號長度應介於0～255個數字之間]\n";
            }

            //權限名稱
            if (string.IsNullOrEmpty(value.accPosition))
            {
                ValidaFailString += "[權限名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.accPosition))
                {
                    ValidaFailString += "[名稱應為中文,英文及數字]\n";
                }
                if (value.accPosition.Length > 10 || value.accPosition.Length < 0)
                {
                    ValidaFailString += "[名稱應介於0～10個字之間]\n";
                }
            }
            //是否有權使用帳號管理 or 會員管理
            if (value.canUseAccount == null || value.canUseMember == null ||
               (value.canUseAccount > 1 || value.canUseAccount < 0) ||
               (value.canUseMember > 1 || value.canUseMember < 0) ||
               (value.canUseProduct > 1 || value.canUseProduct < 0) ||
               (value.canUseOrder > 1 || value.canUseOrder < 0))
            {
                ValidaFailString += "[選擇權限格式錯誤]\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ValidaFailString))
            {
                MyTool.WriteErroLog(ValidaFailString);
                ResultCode = (int)AccountEnum.EditAccLvCode.ValidaFail;
            }
            else
            {

                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_putAccountLevel @accLevel, @accPosission, @canUseAccount, @canUseMember, @canUseProduct, @canUseOrder ";

                    cmd.Parameters.AddWithValue("@accLevel", id);
                    cmd.Parameters.AddWithValue("@accPosission", value.accPosition);
                    cmd.Parameters.AddWithValue("@canUseAccount", value.canUseAccount);
                    cmd.Parameters.AddWithValue("@canUseMember", value.canUseMember);
                    cmd.Parameters.AddWithValue("@canUseProduct", value.canUseProduct);
                    cmd.Parameters.AddWithValue("@canUseOrder", value.canUseOrder);
                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)AccountEnum.EditAccLvCode.ExceptionError;
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

        public int DelAccLv(int id)
        {
            int ResultCode = (int)AccountEnum.DelAccLVCode.Defult;

            string ValidaFailString = "";//記錄錯誤訊息

            //權限編號 
            if (id == 0)
            {
                ValidaFailString += "此權限編號為最高權限無法更改\n";
            }
            if (id > 255 || id < 0)
            {
                ValidaFailString += "[編號長度應介於0～255個數字之間]\n";
            }
            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ValidaFailString))
            {
                MyTool.WriteErroLog(ValidaFailString);
                ResultCode = (int)AccountEnum.DelAccLVCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    //帳號重複驗證寫在SP中
                    cmd.CommandText = @"EXEC pro_onlineShopBack_delAccountLevel @accLevel";

                    cmd.Parameters.AddWithValue("@accLevel", id);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();
                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)AccountEnum.DelAccLVCode.ExceptionError;
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
    }
}
