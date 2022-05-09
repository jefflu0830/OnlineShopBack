using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using System;
using System.Data;
using OnlineShopBack.Tool;
using System.Text;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");



        //會員相關--------------------------------------------------

        #region 會員相關列舉(Enum)
        private enum DelMemberErrorCode //刪除會員
        {
            //<summary >
            //會員刪除成功
            //</summary >
            DelOK = 0,
            //<summary >
            //無此會員
            //</summary >
            MemIsNull = 100
        }

        private enum PutMemberErrorCode //編輯會員
        {
            //<summary >
            //1編輯成功
            //</summary >
            PutOK = 0,
            //<summary >
            //無此會員
            //</summary >
            MemIsNull = 100,
            //<summary >
            //無此會員
            //</summary >
            LvIsNull = 101,
            //<summary >
            //無此會員
            //</summary >
            SuspensionNull = 102
        }

        private enum PutShopGoldCode //購物金調整
        {
            //<summary >
            //調整成功
            //</summary >
            PutOK = 0,
            //<summary >
            //無此會員
            //</summary >
            MemIsNull = 100,
            //<summary >
            //原始購物金與帳號不相符
            //</summary >
            Incompatible = 101,
            //<summary >
            //調整後購物金不得小於0 or 大於20000
            //</summary >
            PutShopGoldError = 102
        }
        #endregion


        //取得會員資料
        [HttpGet("GetMember")]
        public string GetMember()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // 資料庫連線&SQL指令
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            cmd.CommandText = @"EXEC pro_onlineShopBack_getMemberList ";

            //開啟連線
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //關閉連線
            cmd.Connection.Close();

            //DataTable轉Json;
            var result = Tool.MyTool.DataTableJson(dt);

            return result;
        }

        //取得指定會員資料
        [HttpGet("GetMemberByAcc")]
        public string GetMemberByAcc([FromQuery] string acc)
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @"EXEC pro_onlineShopBack_getMemberByAcc @Acc";

                cmd.Parameters.AddWithValue("@Acc", acc);

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);

                //關閉連線
                cmd.Connection.Close();

                //DataTable轉Json;
                var result = Tool.MyTool.DataTableJson(dt);

                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                cmd.Connection.Close();

            }
        }

        //刪除會員
        [HttpDelete("DelMember")]
        public string DelMember([FromQuery] int id)
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

                cmd.CommandText = @"EXEC pro_onlineShopBack_delMember @id";

                cmd.Parameters.AddWithValue("@id", id);

                //開啟連線
                cmd.Connection.Open();
                addAccLVErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccLVErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)DelMemberErrorCode.MemIsNull:
                        return "無此會員";
                    case (int)DelMemberErrorCode.DelOK:
                        return "會員刪除成功";
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

        //編輯會員(等級&狀態)
        [HttpPut("PutMember")]
        public string PutMember([FromQuery] int id, [FromBody] MemberDto value)
        {
            string addAccErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //等級資料驗證
            if (value.Level > 255 || value.Level < 0)
            {
                addAccErrorStr += "[等級編號不再範圍內]\n";
            }

            //狀態資料驗證
            if (value.Suspension > 255 || value.Suspension < 0)
            {
                addAccErrorStr += "[狀態編號不再範圍內]\n";
            }

            if (!string.IsNullOrEmpty(addAccErrorStr))
            {
                return addAccErrorStr;
            }

            SqlCommand cmd = null;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_putMemberByLvAndSuspension @Id, @Lv, @Suspension";

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Lv", value.Level);
                cmd.Parameters.AddWithValue("@Suspension", value.Suspension);

                //開啟連線
                cmd.Connection.Open();
                addAccErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)PutMemberErrorCode.MemIsNull:
                        return "無此會員帳號";
                    case (int)PutMemberErrorCode.LvIsNull:
                        return "無此等級";
                    case (int)PutMemberErrorCode.SuspensionNull:
                        return "無此狀態";
                    case (int)PutMemberErrorCode.PutOK:
                        return "更新成功";
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

        //調整購物金
        [HttpPut("PutShopGold")]
        public string PutShopGold([FromBody] PutShopGlodDto value)
        {
            string addAccErrorStr = "";//記錄錯誤訊息


            //最後修改金額(PutShopGold)=(原始金額(NowAmount)+調整金額(AdjustAmount))
            int? PutShopGold = 0;

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //最後修改金額(PutShopGold)=(原始金額(NowAmount)+調整金額(AdjustAmount))
            PutShopGold = value.NowAmount + value.AdjustAmount;

            //購物金資料驗證
            if (value.AdjustAmount == 0)
            {
                addAccErrorStr += "[調整金額]不得為零\n";
            }
            if (value.AdjustAmount > 5000)
            {
                addAccErrorStr += "[調整金額]每次增加應小於5000\n";
            }
            if (value.AdjustAmount < -5000)
            {
                addAccErrorStr += "[調整金額]每次減少不得小於5000\n";
            }

            //購物金資料驗證
            if (PutShopGold > 20000 || PutShopGold<0)
            {
                addAccErrorStr += "調整後不得小於0 or 大於20000";
            }

            if (!string.IsNullOrEmpty(addAccErrorStr))
            {
                return addAccErrorStr;
            }

            SqlCommand cmd = null;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_putMemberByShopGold @MemAcc,  @NowAmount, @PutShopGold ";

                cmd.Parameters.AddWithValue("@MemAcc", value.MemAcc);
                cmd.Parameters.AddWithValue("@NowAmount", value.NowAmount);
                cmd.Parameters.AddWithValue("@PutShopGold", PutShopGold);

                //開啟連線
                cmd.Connection.Open();
                addAccErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)PutShopGoldCode.Incompatible:
                        return "原始購物金與帳號不相符";
                    case (int)PutShopGoldCode.MemIsNull:
                        return "會員不存在";
                    case (int)PutShopGoldCode.PutShopGoldError:
                        return "調整後購物金不得小於0 or 大於20000";
                    case (int)PutShopGoldCode.PutOK:
                        return "更新成功";
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


        //會員等級相關----------------------------------------------

        #region 會員等級相關列舉(Enum)
        private enum AddMemLvErrorCode //增加會員等級
        {
            //<summary >
            //會員刪除成功
            //</summary >
            AddOK = 0,
            //<summary >
            //無此會員
            //</summary >
            DuplicateMemLv = 100
        }

        private enum DelMemLvErrorCode //刪除會員等級
        {
            //<summary >
            //會員刪除成功
            //</summary >
            DelOK = 0,
            //<summary >
            //無此會員
            //</summary >
            ProhibitDel = 100
        }

        private enum PutMemLVErrorCode // 更新權限
        {
            //<summary >
            //權限更新成功
            //</summary >
            PutOK = 0,
            //<summary >
            //尚未建立此權限
            //</summary >
            LvIsNull = 100

        }

        #endregion

        //取得會員等級資料List
        [HttpGet("GetMemLvList")]
        public string GetMemLvList()
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
                cmd.CommandText = @" EXEC pro_onlineShopBack_getMemberLevel ";

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

        //增加會員等級 
        [HttpPost("AddMemLv")]
        public string AddMemLv([FromBody] MemLvDto value)
        {

            string addMemLvErrorStr = "";//記錄錯誤訊息
            //資料驗證

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //等級編號 
            if (value.memLv == null)
            {
                addMemLvErrorStr += "[編號不可為空]\n";
            }
            else
            {
                if (value.memLv > 255 || value.memLv < 0)
                {
                    addMemLvErrorStr += "[編號長度應介於0～255個數字之間]\n";
                }
            }
            //等級名稱
            if (string.IsNullOrEmpty(value.LvName))
            {
                addMemLvErrorStr += "[權限名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.LvName))
                {
                    addMemLvErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (value.LvName.Length > 10 || value.LvName.Length < 0)
                {
                    addMemLvErrorStr += "[名稱應介於0～10個字之間]\n";
                }
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(addMemLvErrorStr))
            {
                return addMemLvErrorStr;
            }

            SqlCommand cmd = null;
            //DataTable dt = new DataTable();
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
                addMemLvErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addMemLvErrorStr);

                switch (SQLReturnCode)
                {

                    case (int)AddMemLvErrorCode.AddOK:
                        return "會員等級新增成功";
                    case (int)AddMemLvErrorCode.DuplicateMemLv:
                        return "會員等級編號重複";
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
        [HttpPut("PutMemLv")]
        public string PutMemLv([FromQuery] int MemLv, [FromBody] MemLvDto value)
        {
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
                addAccLVErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccLVErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)PutMemLVErrorCode.LvIsNull:
                        return "尚未建立此會員等級";
                    case (int)PutMemLVErrorCode.PutOK:
                        return "會員等級更新成功";
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

        //刪除會員等級
        [HttpDelete("DelMemLv")]
        public string DelMemLv([FromQuery] int memLv)
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

                cmd.CommandText = @"EXEC pro_onlineShopBack_delMemberLevelById @memLv";

                cmd.Parameters.AddWithValue("@memLv", memLv);

                //開啟連線
                cmd.Connection.Open();
                addAccLVErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccLVErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)DelMemLvErrorCode.ProhibitDel:
                        return "此等級不可刪除";
                    case (int)DelMemLvErrorCode.DelOK:
                        return "會員等級刪除成功";
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



        //會員狀態相關----------------------------------------------

        #region 會員等級相關列舉(Enum)
        private enum AddSuspensionCode //增加會員狀態
        {
            //<summary >
            //會員刪除成功
            //</summary >
            AddOK = 0,
            //<summary >
            //無此會員
            //</summary >
            Duplicate = 100
        }

        private enum DelSuspensionCode //刪除會員狀態
        {
            //<summary >
            //刪除成功
            //</summary >
            DelOK = 0,
            //<summary >
            //禁止刪除
            //</summary >
            ProhibitDel = 100,
            //<summary >
            //編號尚未建立
            //</summary >
            isNull = 101
        }

        private enum PutSuspensionCode // 更新狀態
        {
            //<summary >
            //權限更新成功
            //</summary >
            PutOK = 0,
            //<summary >
            //尚未建立此權限
            //</summary >
            IsNull = 100

        }

        #endregion

        //取得狀態資料List
        [HttpGet("GetSuspensionList")]
        public string GetSuspensionList()
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
                cmd.CommandText = @" EXEC pro_onlineShopBack_getGetSuspensionLv ";

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

        //增加狀態 
        [HttpPost("AddSuspension")]
        public string AddSuspension([FromBody] suspensionDto value)
        {

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
            //DataTable dt = new DataTable();
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
                ErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(ErrorStr);

                switch (SQLReturnCode)
                {

                    case (int)AddSuspensionCode.AddOK:
                        return "狀態新增成功";
                    case (int)AddSuspensionCode.Duplicate:
                        return "狀態重複";
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

        //刪除等級
        [HttpDelete("DelSuspension")]
        public string DelSuspension([FromQuery] int id)
        {
            string ErrorStr = "";//記錄錯誤訊息

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

                cmd.CommandText = @"EXEC pro_onlineShopBack_delSuspensionById @id";

                cmd.Parameters.AddWithValue("@id", id);

                //開啟連線
                cmd.Connection.Open();
                ErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(ErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)DelSuspensionCode.ProhibitDel:
                        return "不可刪除";
                    case (int)DelSuspensionCode.DelOK:
                        return "刪除成功";
                    case (int)DelSuspensionCode.isNull:
                        return "狀態尚未建立";
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

        //更新狀態
        [HttpPut("PutSuspension")]
        public string PutSuspension([FromQuery] int id, [FromBody] suspensionDto value)
        {
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
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_putSuspensionById @id, @Name ";

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@Name", value.suspensionName);

                //開啟連線
                cmd.Connection.Open();
                addAccLVErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addAccLVErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)PutSuspensionCode.IsNull:
                        return "尚未建立此狀態";
                    case (int)PutSuspensionCode.PutOK:
                        return "狀態更新成功";
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
