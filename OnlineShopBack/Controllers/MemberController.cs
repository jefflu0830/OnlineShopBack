#region 功能與歷史修改描述
/*
    描述:前台會員系統相關
    日期:2022-05-05
*/
#endregion
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;
using OnlineShopBack.Enum;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Domain.DTOs.Member;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {

        private readonly IMemberRepository _MemberService = null;
        public MemberController(IMemberRepository MemberService)
        {
            _MemberService = MemberService;
        }
        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        /*----------前台會員相關----------*/

        //取得前台會員資料
        [HttpGet("GetMember")]
        public string GetMember()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            DataTable dt = new DataTable();
            dt = _MemberService.GetAccountAndLevelList();
            //DataTable轉Json;
            string result = MyTool.DataTableJson(dt);

            return result;
        }

        //取得指定會員資料
        [HttpGet("GetMemberByAcc")]
        public string GetMemberByAcc([FromQuery] string Acc)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            if (RolesValidate())
            {
                return "無使用權限";
            }

            DataTable dt = new DataTable();
            dt = _MemberService.GetMemberByAcc(Acc);
            //DataTable轉Json;
            string result = MyTool.DataTableJson(dt);

            return result;

        }

        //刪除會員
        [HttpDelete("DelMember")]
        public string DelMember([FromQuery] int id)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }
            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _MemberService.DelMember(id);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //編輯會員(等級&狀態)
        [HttpPut("EditMember")]
        public string EditMember([FromQuery] int id, [FromBody] MemberDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }



            int ResultCode = _MemberService.EditMember(id, value);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //調整購物金
        [HttpPut("EditShopGold")]
        public string EditShopGold([FromBody] PutShopGlodDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _MemberService.EditShopGold(value);

            return "[{\"st\": " + ResultCode + "}]";
        }

        /*----------前台會員等級相關----------*/

        //取得會員等級資料List
        [HttpGet("GetMemLvList")]
        public string GetMemLvList()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                //cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel";
                cmd.CommandText = @" EXEC pro_onlineShopBack_getMemberLevel ";

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
            //DataTable轉Json;
            string result = MyTool.DataTableJson(dt);

            return result;
        }

        //添加會員等級 
        [HttpPost("AddMemLv")]
        public string AddMemLv([FromBody] MemLvDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            string AddMemLvErrorStr = "";//記錄錯誤訊息
            //資料驗證

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //等級編號 
            if (value.memLv == null)
            {
                AddMemLvErrorStr += "[編號不可為空]\n";
            }
            else
            {
                if (value.memLv > 255 || value.memLv < 0)
                {
                    AddMemLvErrorStr += "[編號長度應介於0～255個數字之間]\n";
                }
            }
            //等級名稱
            if (string.IsNullOrEmpty(value.LvName))
            {
                AddMemLvErrorStr += "[權限名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.LvName))
                {
                    AddMemLvErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (value.LvName.Length > 10 || value.LvName.Length < 0)
                {
                    AddMemLvErrorStr += "[名稱應介於0～10個字之間]\n";
                }
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(AddMemLvErrorStr))
            {
                return AddMemLvErrorStr;
            }

            SqlCommand cmd = null;
            string SQLReturnCode = "";
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                //重複驗證寫在SP中
                cmd.CommandText = @"EXEC pro_onlineShopBack_addMemberLevel @memLv, @LvName";

                cmd.Parameters.AddWithValue("@memLv", value.memLv);
                cmd.Parameters.AddWithValue("@LvName", value.LvName);

                //開啟連線

                cmd.Connection.Open();
                SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
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

                case (int)MemberEnum.AddMemLvErrorCode.AddOK:
                    return "會員等級新增成功";
                case (int)MemberEnum.AddMemLvErrorCode.DuplicateMemLv:
                    return "會員等級編號重複";
                default:
                    return "失敗";
            }

        }

        //更新權限
        [HttpPut("EditMemLv")]
        public string EditMemLv([FromQuery] int MemLv, [FromBody] MemLvDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            string addAccLVErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //權限編號 
            if (MemLv > 255 || MemLv < 0)
            {
                addAccLVErrorStr += "[會員等級編號長度應介於0～255個數字之間]\n";
            }

            //權限名稱
            if (string.IsNullOrEmpty(value.LvName))
            {
                addAccLVErrorStr += "[等級名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.LvName))
                {
                    addAccLVErrorStr += "[等級名稱應為中文,英文及數字]\n";
                }
                if (value.LvName.Length > 10 || value.LvName.Length < 0)
                {
                    addAccLVErrorStr += "[等級名稱應介於0～10個字之間]\n";
                }
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

                cmd.CommandText = @"EXEC pro_onlineShopBack_putMemberLevelById @MemLv, @LvName ";

                cmd.Parameters.AddWithValue("@MemLv", MemLv);
                cmd.Parameters.AddWithValue("@LvName", value.LvName);

                //開啟連線
                cmd.Connection.Open();
                SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL
            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
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
                case (int)MemberEnum.PutMemLVErrorCode.LvIsNull:
                    return "尚未建立此會員等級";
                case (int)MemberEnum.PutMemLVErrorCode.PutOK:
                    return "會員等級更新成功";
                default:
                    return "失敗";
            }

        }

        //刪除會員等級
        [HttpDelete("DelMemLv")]
        public string DelMemLv([FromQuery] int memLv)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            string addAccLVErrorStr = "";//記錄錯誤訊息

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

                cmd.CommandText = @"EXEC pro_onlineShopBack_delMemberLevelById @memLv";

                cmd.Parameters.AddWithValue("@memLv", memLv);

                //開啟連線
                cmd.Connection.Open();
                SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
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
                case (int)MemberEnum.DelMemLvErrorCode.ProhibitDel:
                    return "此等級不可刪除";
                case (int)MemberEnum.DelMemLvErrorCode.DelOK:
                    return "會員等級刪除成功";
                case (int)MemberEnum.DelMemLvErrorCode.IsNull:
                    return "此等級尚未建立";
                case (int)MemberEnum.DelMemLvErrorCode.IsUsing:
                    return "有使用者正在套用此等級";
                default:
                    return "失敗";
            }
        }


        /*----------前台會員狀態相關----------*/

        //取得狀態資料List
        [HttpGet("GetSuspensionList")]
        public string GetSuspensionList()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                //cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel";
                cmd.CommandText = @" EXEC pro_onlineShopBack_getGetSuspensionLv ";

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
            //DataTable轉Json;
            string result = MyTool.DataTableJson(dt);

            return result;
        }

        //增加狀態 
        [HttpPost("AddSuspension")]
        public string AddSuspension([FromBody] suspensionDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            string ErrorStr = "";//記錄錯誤訊息
            //資料驗證

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //等級編號 
            if (value.suspensionLv == null)
            {
                ErrorStr += "[編號不可為空]\n";
            }
            else
            {
                if (value.suspensionLv > 255 || value.suspensionLv < 0)
                {
                    ErrorStr += "[編號長度應介於0～255個數字之間]\n";
                }
            }
            //等級名稱
            if (string.IsNullOrEmpty(value.suspensionName))
            {
                ErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.suspensionName))
                {
                    ErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (value.suspensionName.Length > 10 || value.suspensionName.Length < 0)
                {
                    ErrorStr += "[名稱應介於0～10個字之間]\n";
                }
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                return ErrorStr;
            }

            SqlCommand cmd = null;
            string SQLReturnCode = "";
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                //重複驗證寫在SP中
                cmd.CommandText = @"EXEC pro_onlineShopBack_addsuspension @Lv, @Name";

                cmd.Parameters.AddWithValue("@Lv", value.suspensionLv);
                cmd.Parameters.AddWithValue("@Name", value.suspensionName);

                //開啟連線

                cmd.Connection.Open();
                SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
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

                case (int)MemberEnum.AddSuspensionCode.AddOK:
                    return "狀態新增成功";
                case (int)MemberEnum.AddSuspensionCode.Duplicate:
                    return "狀態重複";
                default:
                    return "失敗";
            }

        }

        //刪除等級
        [HttpDelete("DelSuspension")]
        public string DelSuspension([FromQuery] int id)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            string ErrorStr = "";//記錄錯誤訊息

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

                cmd.CommandText = @"EXEC pro_onlineShopBack_delSuspensionById @id";

                cmd.Parameters.AddWithValue("@id", id);

                //開啟連線
                cmd.Connection.Open();
                SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
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
                case (int)MemberEnum.DelSuspensionCode.ProhibitDel:
                    return "不可刪除";
                case (int)MemberEnum.DelSuspensionCode.DelOK:
                    return "刪除成功";
                case (int)MemberEnum.DelSuspensionCode.isNull:
                    return "狀態尚未建立";
                default:
                    return "失敗";
            }
        }

        //更新狀態
        [HttpPut("EditSuspension")]
        public string EditSuspension([FromQuery] int id, [FromBody] suspensionDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            string addAccLVErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //權限編號 
            if (id > 255 || id < 0)
            {
                addAccLVErrorStr += "[編號長度應介於0～255個數字之間]\n";
            }

            //權限名稱
            if (string.IsNullOrEmpty(value.suspensionName))
            {
                addAccLVErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.suspensionName))
                {
                    addAccLVErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (value.suspensionName.Length > 10 || value.suspensionName.Length < 0)
                {
                    addAccLVErrorStr += "[名稱應介於0～10個字之間]\n";
                }
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

                cmd.CommandText = @"EXEC pro_onlineShopBack_putSuspension @id, @Name ";

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@Name", value.suspensionName);

                //開啟連線
                cmd.Connection.Open();
                SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
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
                case (int)MemberEnum.PutSuspensionCode.IsNull:
                    return "尚未建立此狀態";
                case (int)MemberEnum.PutSuspensionCode.PutOK:
                    return "狀態更新成功";
                default:
                    return "失敗";
            }

        }

        /*-------------------------------------------*/
        //登入檢查
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
        //權限檢查
        private bool RolesValidate()
        {

            if (HttpContext.Session.GetString("Roles").Contains("canUseMember"))
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
