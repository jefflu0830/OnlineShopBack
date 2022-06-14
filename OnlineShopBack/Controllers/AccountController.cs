#region 功能與歷史修改描述
/*
    描述:後台帳號系統相關
    日期:2022-05-05
*/
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
    public class AccountController : ControllerBase
    {
        //取得SQL連線字串
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        //帳號資料left join權限資料
        [HttpGet("GetAcc")]
        public string GetAcc()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

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
            string result = MyTool.DataTableJson(dt);

            return result;
        }

        //增加帳號
        [HttpPost("AddAcc")]
        public string AddAcc([FromBody] AccountDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

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
            string SQLReturnCode = ""; 
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
                SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

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

            int ResultCode = int.Parse(SQLReturnCode);

            switch (ResultCode)
            {
                case (int)AccountEnum.AddACCountErrorCode.duplicateAccount:
                    return "此帳號已存在";

                case (int)AccountEnum.AddACCountErrorCode.permissionIsNull:
                    return "該權限未建立";

                case (int)AccountEnum.AddACCountErrorCode.AddOK:
                    return "帳號新增成功";
                default:
                    return "失敗";
            }

        }
        /*-----------後臺帳號相關-----------*/
        //編輯帳號_權限
        [HttpPut("EditAcc")]
        public string EditAcc([FromQuery] int id, [FromBody] AccountDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

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
            string SQLReturnCode = "";
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
                SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

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
            int ResultCode = int.Parse(SQLReturnCode);

            switch (ResultCode)
            {
                case (int)AccountEnum.PutAccErrorCode.ProhibitPut:
                    return "此帳號不可做更改";
                case (int)AccountEnum.PutAccErrorCode.LvIsNull:
                    return "尚未建立此權限";
                case (int)AccountEnum.PutAccErrorCode.PutOK:
                    return "帳號更新成功";
                default:
                    return "失敗";
            }
        }

        //更新帳號_密碼
        [HttpPut("EditPwd")]
        public string EditPwd([FromBody] PutPwdDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

            string addAccErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

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
                return addAccErrorStr;
            }

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

            int ResultCode = int.Parse(SQLReturnCode);

            switch (ResultCode)
            {
                case (int)AccountEnum.PutAccPwdErrorCode.confirmError:
                    return "新密碼與確認新密碼不相同";
                case (int)AccountEnum.PutAccPwdErrorCode.AccIsNull:
                    return "此帳號不存在";
                case (int)AccountEnum.PutAccPwdErrorCode.PutOK:
                    return "密碼修改成功";
                default:
                    return "失敗";
            }
        }

        //刪除帳號
        [HttpDelete("DelAcc")]
        public string DelAcc([FromQuery] int id)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

            

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            SqlCommand cmd = null;
            string SQLReturnCode = "";
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_delAccount @f_acc";

                cmd.Parameters.AddWithValue("@f_acc", id);

                //開啟連線
                cmd.Connection.Open();
                SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

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

            int ResultCode = int.Parse(SQLReturnCode);

            switch (ResultCode)
            {
                case (int)AccountEnum.DelACCountErrorCode.AccIsNull:
                    return "無此帳號";
                case (int)AccountEnum.DelACCountErrorCode.ProhibitDel:
                    return "此帳號不可刪除";
                case (int)AccountEnum.DelACCountErrorCode.DelOK:
                    return "帳號刪除成功";
                default:
                    return "失敗";
            }
        }

        /*-----------後臺帳號權限相關-----------*/
        //權限資料List
        [HttpGet("GetAccLvList")]
        public string GetAccLvList()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

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
            string result = MyTool.DataTableJson(dt);

            return result;
        }

        //依照ID查詢權限資料
        [HttpGet("IdGetAccLV")]
        public string IdGetAccLV([FromQuery] int id)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

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
            string result = MyTool.DataTableJson(dt);

            return result;
        }

        //增加權限
        [HttpPost("AddAccLv")]
        public string AddAccLv([FromBody] AccountLevelDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

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
               (value.canUseMember > 1 || value.canUseMember < 0) ||
               (value.canUseOrder > 1 || value.canUseOrder < 0))
            {
                addAccLVErrorStr += "[選擇權限格式錯誤]\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(addAccLVErrorStr))
            {
                return addAccLVErrorStr;
            }

            SqlCommand cmd = null;
            string SQLReturnCode = "";
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

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
                SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

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

            int ResultCode = int.Parse(SQLReturnCode);

            switch (ResultCode)
            {

                case (int)AccountEnum.addACCountLVErrorCode.addOK:
                    return "權限新增成功";
                case (int)AccountEnum.addACCountLVErrorCode.duplicateAccountLv:
                    return "權限編號重複";
                default:
                    return "失敗";
            }
        }

        //更新權限
        [HttpPut("EditAccLv")]
        public string PutAccLv([FromQuery] int id, [FromBody] AccountLevelDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

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
               (value.canUseMember > 1 || value.canUseMember < 0) ||
               (value.canUseProduct > 1 || value.canUseProduct < 0)||
               (value.canUseOrder > 1 || value.canUseOrder < 0))
            {
                addAccLVErrorStr += "[選擇權限格式錯誤]\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(addAccLVErrorStr))
            {
                return addAccLVErrorStr;
            }

            SqlCommand cmd = null;
            string SQLReturnCode = "";
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_putAccountLevel @accLevel, @accPosission, @canUseAccount, @canUseMember, @canUseProduct, @canUseOrder ";

                cmd.Parameters.AddWithValue("@accLevel", id);
                cmd.Parameters.AddWithValue("@accPosission", value.accPosition);
                cmd.Parameters.AddWithValue("@canUseAccount", value.canUseAccount);
                cmd.Parameters.AddWithValue("@canUseMember", value.canUseMember);
                cmd.Parameters.AddWithValue("@canUseProduct", value.canUseProduct);
                cmd.Parameters.AddWithValue("@canUseOrder", value.canUseOrder);
                //開啟連線
                cmd.Connection.Open();
                SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

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
            int ResultCode = int.Parse(SQLReturnCode);

            switch (ResultCode)
            {
                case (int)AccountEnum.PutACCountLVErrorCode.prohibitPutlv:
                    return "此權限不可更改";
                case (int)AccountEnum.PutACCountLVErrorCode.LvIsNull:
                    return "此權限尚未建立";
                case (int)AccountEnum.PutACCountLVErrorCode.PutOK:
                    return "權限更新成功";
                default:
                    return "失敗";
            }

        }

        //刪除權限
        [HttpDelete("DelAccLv")]
        public string DelAccLv([FromQuery] int id)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

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
            int SQLReturnCode;
            int aa;
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
                SQLReturnCode = int.Parse(cmd.ExecuteScalar().ToString());
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

            switch (SQLReturnCode)
            {
                case (int)AccountEnum.DelACCountLVErrorCode.LvIsNull:
                    return "此權限尚未建立";
                case (int)AccountEnum.DelACCountLVErrorCode.IsUsing:
                    return "此權限目前有人正在使用";
                case (int)AccountEnum.DelACCountLVErrorCode.DelOK:
                    return "刪除成功";
                default:
                    return "失敗";
            }
        }

        //登入&權限檢查
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
        private bool RolesValidate()
        {
            if (HttpContext.Session.GetString("Roles").Contains("canUseAccount"))
            {
                return false;
            }else
            {
                return true;
            }
        }
    }
    

}

