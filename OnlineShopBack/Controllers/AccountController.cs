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


        //權限相關-------------------------------------------------------------------

        #region 權限相關列舉(Enum)
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
            IsUsing = 100,
            //<summary >
            //尚未建立此權限
            //</summary >
            LvIsNull = 101
        }

        private enum PutACCountLVErrorCode // 更新權限
        {
            //<summary >
            //權限更新成功
            //</summary >
            PutOK = 0,
            //<summary >
            //尚未建立此權限
            //</summary >
            LvIsNull = 100,
            //<summary >
            //尚未建立此權限
            //</summary >
            CantPut = 101

        }

        #endregion

        //權限資料List
        [HttpGet("GetAccLvList")]
        public string GetAccLvList()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                //cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel";
                cmd.CommandText = @" EXEC pro_onlineShopBack_getAccountLevel ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception e)
            {
                return e.Message;
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
            //DataTable轉Json;
            var result = MyTool.DataTableJson(dt);

            return result;
        }

        //依照ID查詢權限資料
        [HttpGet("IdGetAccLV")]
        public string IdGetAccLV([FromQuery] int id)
        {
            string addAccLVErrorStr = "";//記錄錯誤訊息

            if (id > 255 || id < 0)
            {
                addAccLVErrorStr += "[編號長度應介於0～255個數字之間]\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(addAccLVErrorStr))
            {
                return addAccLVErrorStr;
            }

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                //cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel";
                cmd.CommandText = @" EXEC pro_onlineShopBack_getAccountLevelById @accLevel ";
                cmd.Parameters.AddWithValue("@accLevel", id);

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception e)
            {
                return e.Message;
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

            //DataTable轉Json;
            var result = MyTool.DataTableJson(dt);

            return result;
        }

        //增加權限
        [HttpPost("AddAccLv")]
        public string AddAccLv([FromBody] AccountLevelDto value)
        {

            string addAccLVErrorStr = "";//記錄錯誤訊息
            //資料驗證

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //權限編號 
            if (value.accLevel == null)
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
               (value.canUseAccount > 1 || value.canUseAccount < 0) ||
               (value.canUseMember > 1 || value.canUseMember < 0))
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
            catch (Exception e)
            {
                return e.Message;
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

        //更新權限
        [HttpPut("PutAccLv")]
        public string PutAccLv([FromQuery] int id, [FromBody] AccountLevelDto value)
        {
            string addAccLVErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //權限編號 
            if (id == 0)
            {
                addAccLVErrorStr += "此權限編號為最高權限無法更改\n";
            }
            if (id > 255 || id < 0)
            {
                addAccLVErrorStr += "[編號長度應介於0～255個數字之間]\n";
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
               (value.canUseAccount > 1 || value.canUseAccount < 0) ||
               (value.canUseMember > 1 || value.canUseMember < 0))
            {
                addAccLVErrorStr += "[選擇權限格式錯誤]\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(addAccLVErrorStr))
            {
                return addAccLVErrorStr;
            }

            SqlCommand cmd = null;
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
                    case (int)PutACCountLVErrorCode.CantPut:
                        return "此權限不可更改";
                    case (int)PutACCountLVErrorCode.LvIsNull:
                        return "此權限尚未建立";
                    case (int)PutACCountLVErrorCode.PutOK:
                        return "權限更新成功";
                    default:
                        return "失敗";
                }
            }
            catch (Exception e)
            {
                return e.Message;
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

        //刪除權限
        [HttpDelete("DelAccLv")]
        public string DelAccLv([FromQuery] int id)
        {
            string addAccLVErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //權限編號 
            if (id == 0)
            {
                addAccLVErrorStr += "此權限編號為最高權限無法更改\n";
            }
            if (id > 255 || id < 0)
            {
                addAccLVErrorStr += "[編號長度應介於0～255個數字之間]\n";
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

                //帳號重複驗證寫在SP中
                cmd.CommandText = @"EXEC pro_onlineShopBack_delAccountLevel @accLevel";

                cmd.Parameters.AddWithValue("@accLevel", id);

                //開啟連線
                cmd.Connection.Open();
                addAccLVErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccLVErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)DelACCountLVErrorCode.LvIsNull:
                        return "此權限尚未建立";
                    case (int)DelACCountLVErrorCode.IsUsing:
                        return "此權限目前有人正在使用";
                    case (int)DelACCountLVErrorCode.DelOK:
                        return "刪除成功";
                    default:
                        return "失敗";
                }
            }
            catch (Exception e)
            {
                return e.Message;
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

        #region 帳號相關列舉(Enum)
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
        private enum PutACCountErrorCode //更新帳號
        {
            //<summary >
            //帳號刪除成功
            //</summary >
            PutOK = 0,
            //<summary >
            //此帳號不可更改
            //</summary >
            DontPut = 100,
            //<summary >
            //尚未建立權限
            //</summary >
            LvIsNull = 101

        }
        private enum DelACCountErrorCode //刪除帳號
        {
            //<summary >
            //帳號刪除成功
            //</summary >
            DelOK = 0,
            //<summary >
            //此帳號不可刪除
            //</summary >
            DontDel = 100,
            //<summary >
            //無此帳號
            //</summary >
            AccIsNull = 101

        }

        #endregion

        //Select帳號資料Left join權限資料
        [HttpGet("GetAcc")]
        public string GetAcc()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                //cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel";
                cmd.CommandText = @" EXEC pro_onlineShopBack_getAccountAndAccountLevelList ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception e)
            {
                return e.Message;
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
            //DataTable轉Json;
            var result = MyTool.DataTableJson(dt);

            return result;
        }

        //Select帳號資料Left join權限資料where ID
        [HttpGet("IdGetAcc")]
        public string IdGetAccount([FromQuery] int id)
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
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
            }
            catch (Exception e)
            {
                return e.Message;
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
            //DataTable轉Json;
            var result = MyTool.DataTableJson(dt);

            return result;
        }

        //增加帳號
        [HttpPost("AddAcc")]
        public string AddAcc([FromBody] AccountSelectDto value)
        {
            //後端驗證
            //如字串字數特殊字元驗證

            string addAccErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

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
            catch (Exception e)
            {
                return e.Message;
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

        //更新帳號
        [HttpPut("PutAcc")]
        public string PutAcc([FromQuery] int id, [FromBody] AccountSelectDto value)
        {
            string addAccErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
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

                cmd.CommandText = @"EXEC pro_onlineShopBack_putAccount @Id, @Level";

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Level", value.Level);

                //開啟連線
                cmd.Connection.Open();
                addAccErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)PutACCountErrorCode.DontPut:
                        return "此帳號不可做更改";
                    case (int)PutACCountErrorCode.LvIsNull:
                        return "尚未建立此權限";
                    case (int)PutACCountErrorCode.PutOK:
                        return "帳號更新成功";
                    default:
                        return "失敗";
                }
            }
            catch (Exception e)
            {
                return e.Message;
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

        //更新帳號
        [HttpPut("PutPwd")]
        public string PutPwd([FromBody] PutPwdDto value)
        {
            string addAccErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
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

                cmd.CommandText = @"EXEC pro_onlineShopBack_putAccount @Acc, @OldPwd, @NewPwd";

                cmd.Parameters.AddWithValue("@Acc", value.Acc);
                cmd.Parameters.AddWithValue("@OldPwd", value.OldPwd);
                cmd.Parameters.AddWithValue("@NewPwd", value.NewPwd);

                //開啟連線
                cmd.Connection.Open();
                addAccErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)PutACCountErrorCode.DontPut:
                        return "此帳號不可做更改";
                    case (int)PutACCountErrorCode.LvIsNull:
                        return "尚未建立此權限";
                    case (int)PutACCountErrorCode.PutOK:
                        return "帳號更新成功";
                    default:
                        return "失敗";
                }
            }
            catch (Exception e)
            {
                return e.Message;
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

        //刪除帳號
        [HttpDelete("DelAcc")]
        public string DelAcc([FromQuery] int id)
        {
            string addAccLVErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            SqlCommand cmd = null;
            //DataTable dt = new DataTable();
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_delAccount @f_acc";

                cmd.Parameters.AddWithValue("@f_acc", id);

                //開啟連線
                cmd.Connection.Open();
                addAccLVErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccLVErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)DelACCountErrorCode.AccIsNull:
                        return "無此帳號";
                    case (int)DelACCountErrorCode.DontDel:
                        return "此帳號不可刪除";
                    case (int)DelACCountErrorCode.DelOK:
                        return "帳號刪除成功";
                    default:
                        return "失敗";
                }
            }
            catch (Exception e)
            {
                return e.Message;
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

