using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Models;
using OnlineShopBack.Pages.Account;
using OnlineShopBack.Services;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OnlineShopBack.Tool;
using Microsoft.AspNetCore.Authorization;
using System;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineShopBack.Controllers
{
    [Authorize(Roles = "canUseAccount")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        //已註解
        #region GetAccount  EF舊寫法用所需
        //private readonly OnlineShopContext _OnlineShopContext;
        //public AccountController(OnlineShopContext onlineShopContext)
        //{
        //    _OnlineShopContext = onlineShopContext;
        //}
        #endregion

        //已註解
        #region  GetAccount舊寫法EF
        //[HttpGet("GetAccount")]
        //public IEnumerable<AccountSelectDto> GetAccount()
        //{
        //    var result = _OnlineShopContext.TAccount
        //        .Select(a => new AccountSelectDto
        //        {
        //            Id = a.FId,
        //            Account = a.FAcc,
        //            Pwd = a.FPwd,
        //            Level = a.FLevel
        //        });
        //    return result;

        //}
        #endregion

        //Select帳號資料Left join權限資料
        [HttpGet("GetAccount")]
        public string GetAccount()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // 資料庫連線&SQL指令
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            //cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel";
            cmd.CommandText = @" EXEC pro_onlineShopBack_getAccountAndAccountLevel01 ";  

            //開啟連線
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //關閉連線
            cmd.Connection.Close();

            //DataTable轉Json;
            var result = MyTool.DataTableJson(dt);

            return result;
        }

        //Select帳號資料Left join權限資料where ID
        [HttpGet("GetAccount/{id}")]
        public string IdGetAccount(int id)
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // 資料庫連線&SQL指令
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            //cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel";
            cmd.CommandText = @" SELECT f_acc FROM t_Account ";
            cmd.Parameters.AddWithValue("@accLevel", id);

            //開啟連線
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //關閉連線
            cmd.Connection.Close();

            //DataTable轉Json;
            var result = MyTool.DataTableJson(dt);

            return result;
        }

        //Select帳號權限資料
        [HttpGet("GetAccountLV")]
        public string GetAccountLV()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // 資料庫連線&SQL指令
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            //cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel";
            cmd.CommandText = @" EXEC pro_onlineShopBack_getAccountLevel ";  

            //開啟連線
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //關閉連線
            cmd.Connection.Close();

            //DataTable轉Json;
            var result = MyTool.DataTableJson(dt);

            return result;
        }

        //Select帳號權限資料where ID
        [HttpGet("GetAccountLV/{id}")]
        public string IdGetAccountLV(int id)
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // 資料庫連線&SQL指令
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            //cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel";
            cmd.CommandText = @" EXEC pro_onlineShopBack_getAccountLevel01 @accLevel ";
            cmd.Parameters.AddWithValue("@accLevel", id);

            //開啟連線
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //關閉連線
            cmd.Connection.Close();

            //DataTable轉Json;
            var result = MyTool.DataTableJson(dt);

            return result;
        }



        //權限相關-------------------------------------------------------------------

        //列舉(Enum)
        #region 權限相關列舉
        private enum addACCountLVErrorCode //新增權限
        {
            //<summary >
            //權限新增成功
            //</summary >
            addOK = 0,

            //<summary >
            //權限重複
            //</summary >
            duplicateAccountLv = 100
        }

        private enum DelACCountLVErrorCode //刪除權限
        {
            //<summary >
            //權限刪除成功
            //</summary >
            DelOK = 0,
            //<summary >
            //有帳號使用此權限中
            //</summary >
            IsUsing = 100


        }

        private enum PutACCountLVErrorCode // 更新權限
        {
            //<summary >
            //權限更新成功
            //</summary >
            PutOK = 0
        }

        #endregion

        //[POST]  增加帳號權限 t_accountLevel
        [HttpPost("AddAccountLevel")]
        public string AddAccountLevel([FromBody] AccountLevelDto value)
        {

            string addAccLVErrorStr = "";//記錄錯誤訊息
            //資料驗證

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }          

            //權限編號 
            if (value.accLevel== null)
            {
                addAccLVErrorStr += "[編號不可為空]\n";
            }  
            else
            {
                if (value.accLevel > 255 || value.accLevel < 0)
                {
                    addAccLVErrorStr += "[編號長度應介於0～255個數字之間]\n";
                }
            }
            //權限名稱
            if (string.IsNullOrEmpty(value.accPosition)) 
            {
                addAccLVErrorStr += "[權限名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.accPosition))
                {
                    addAccLVErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (value.accPosition.Length > 10 || value.accPosition.Length < 0)
                {
                    addAccLVErrorStr += "[名稱應介於0～10個字之間]\n";
                }
            }
            //是否有權使用帳號管理 or 會員管理
            if (value.canUseAccount == null || value.canUseMember == null ||
               (value.canUseAccount >1 || value.canUseAccount<0) ||
               (value.canUseMember > 1 || value.canUseMember<0))
            {
                addAccLVErrorStr += "[選擇權限格式錯誤]\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(addAccLVErrorStr))
            {
                return addAccLVErrorStr;
            }

            SqlCommand cmd = null;
            //DataTable dt = new DataTable();
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                //重複驗證寫在SP中
                cmd.CommandText = @"EXEC pro_onlineShopBack_addAccountLevel @accLevel, @accPosission, @canUseAccount, @canUseMember";

                cmd.Parameters.AddWithValue("@accLevel", value.accLevel);
                cmd.Parameters.AddWithValue("@accPosission", value.accPosition);
                cmd.Parameters.AddWithValue("@canUseAccount", value.canUseAccount);
                cmd.Parameters.AddWithValue("@canUseMember", value.canUseMember);

                //開啟連線

                cmd.Connection.Open();
                addAccLVErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccLVErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)addACCountLVErrorCode.addOK:
                        return "權限新增成功";
                    case (int)addACCountLVErrorCode.duplicateAccountLv:
                        return "權限編號重複";
                    default:
                        return "失敗";
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
        }

        //更新帳號權限
        [HttpPut("PutAccountLevel/{id}")]
        public string PutAccountLevel(int id, [FromBody] AccountLevelDto value)
        {
            string addAccLVErrorStr = "";//記錄錯誤訊息

            SqlCommand cmd = null;
            //DataTable dt = new DataTable();
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                //帳號重複驗證寫在SP中
                cmd.CommandText = @"EXEC pro_onlineShopBack_putAccountLevel @accLevel, @accPosission, @canUseAccount, @canUseMember ";

                cmd.Parameters.AddWithValue("@accLevel", id);
                cmd.Parameters.AddWithValue("@accPosission", value.accPosition);
                cmd.Parameters.AddWithValue("@canUseAccount", value.canUseAccount);
                cmd.Parameters.AddWithValue("@canUseMember", value.canUseMember);

                //開啟連線
                cmd.Connection.Open();
                addAccLVErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccLVErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)PutACCountLVErrorCode.PutOK:
                        return "權限更新成功";
                    default:
                        return "失敗";
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

        }

        //刪除帳號權限
        [HttpDelete("DelAccountLevel/{id}")]
        public string DelAccountLevel(int id)
        {
            string addAccLVErrorStr = "";//記錄錯誤訊息



            SqlCommand cmd = null;
            //DataTable dt = new DataTable();
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                //帳號重複驗證寫在SP中
                cmd.CommandText = @"EXEC pro_onlineShopBack_delAccountLevel @accLevel";

                cmd.Parameters.AddWithValue("@accLevel", id);

                //開啟連線
                cmd.Connection.Open();
                addAccLVErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccLVErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)DelACCountLVErrorCode.IsUsing:
                        return "此權限目前有人正在使用";
                    case (int)DelACCountLVErrorCode.DelOK:
                        return "刪除成功";
                    default:
                        return "失敗";
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
        }


        //帳號相關------------------------------------------------------------------

        //列舉(Enum)
        #region 帳號相關列舉
        private enum addACCountErrorCode //新增帳號
        {
            //<summary >
            //帳號新增成功
            //</summary >
            AddOK = 0,

            //<summary >
            //帳號重複
            //</summary >
            duplicateAccount = 101,

            //<summary >
            //該權限未建立
            //</summary >
            permissionIsNull = 102
        }
        private enum DelACCountErrorCode //刪除帳號
        {
            //<summary >
            //帳號刪除成功
            //</summary >
            DelOK = 0
        }

        #endregion

        //[POST]  增加帳號 t_account
        [HttpPost("AddAccount")]
        public string AddAccount([FromBody] AccountSelectDto value)
        {
            //後端驗證
            //如字串字數特殊字元驗證

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

            #region  可改為只輸出一行
            //if (string.IsNullOrEmpty(value.Account) || !MyTool.IsENAndNumber(value.Account) || value.Account.Length > 20 || value.Account.Length < 3)
            //{
            //    addAccErrorStr += "[帳號格式不符合]\n";
            //}
            #endregion

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

            if (!string.IsNullOrEmpty(addAccErrorStr))
            {
                return addAccErrorStr;
            }

            SqlCommand cmd = null;
            //DataTable dt = new DataTable();
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                //帳號重複驗證寫在SP中

                cmd.CommandText = @"EXEC pro_onlineShopBack_addAccount @f_acc, @f_pwd, @f_level";

                cmd.Parameters.AddWithValue("@f_acc", value.Account);
                cmd.Parameters.AddWithValue("@f_pwd", MyTool.PswToMD5(value.Pwd));
                cmd.Parameters.AddWithValue("@f_level", value.Level);

                #region //SQL回傳是 Return時的接法
                //SqlParameter returnValue = new SqlParameter("XXX", SqlDbType.Int);
                //returnValue.Direction = ParameterDirection.ReturnValue;
                //cmd.Parameters.Add(returnValue);


                //return returnValue.Value.ToString();
                #endregion

                //開啟連線

                cmd.Connection.Open();
                addAccErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)addACCountErrorCode.duplicateAccount:
                        return "此帳號已存在";

                    case (int)addACCountErrorCode.permissionIsNull:
                        return "該權限未建立";

                    case (int)addACCountErrorCode.AddOK:
                        return "帳號新增成功";
                    default:
                        return "失敗";
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



            #region EF舊寫法已註解
            /*using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(value.Pwd));//MD5 加密傳密碼進去
                var strResult = BitConverter.ToString(result);

                TAccount insert = new TAccount
                {
                    FAcc = value.Account,
                    FPwd = strResult.Replace("-",""),
                    FLevel = value.Level 
                };
                _OnlineShopContext.Add(insert);
                _OnlineShopContext.SaveChanges();
               return "新增成功"; 
            }*/
            #endregion

        }

        // PUT 更新帳號
        [HttpPut("PutAccount/{id}")]
        public string PutAccount(int id, [FromBody] AccountSelectDto value)
        {
            string addAccLVErrorStr = "";//記錄錯誤訊息

            SqlCommand cmd = null;
            //DataTable dt = new DataTable();
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                //帳號重複驗證寫在SP中
                cmd.CommandText = @"EXEC pro_onlineShopBack_putAccount @Id, @Pwd, @Level";

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Pwd", MyTool.PswToMD5(value.Pwd));
                cmd.Parameters.AddWithValue("@Level", value.Level);

                //開啟連線
                cmd.Connection.Open();
                addAccLVErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccLVErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)PutACCountLVErrorCode.PutOK:
                        return "帳號更新成功";
                    default:
                        return "失敗";
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
        }

        // DELETE 刪除帳號
        [HttpDelete("AccountDel/{id}")]
        public string AccountDel(int id)
        {
            string addAccLVErrorStr = "";//記錄錯誤訊息
            SqlCommand cmd = null;
            //DataTable dt = new DataTable();
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                //帳號重複驗證寫在SP中
                cmd.CommandText = @"EXEC pro_onlineShopBack_delAccount @f_acc";

                cmd.Parameters.AddWithValue("@f_acc", id);

                //開啟連線
                cmd.Connection.Open();
                addAccLVErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccLVErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)DelACCountErrorCode.DelOK:
                        return "帳號刪除成功";
                    default:
                        return "失敗";
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
        }

    }

}

