using Microsoft.Data.SqlClient;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;
using OnlineShopBack.Domain.Enum;
using OnlineShopBack.Domain.DTOs.Member;

namespace OnlineShopBack.Persistent
{

    public class MemberRepository : IMemberRepository
    {
        private readonly string _SQLConnectionString = null;//SQL連線字串


        public MemberRepository(IConfigHelperRepository configHelperRepository)
        {
            //SQL連線字串
            _SQLConnectionString = configHelperRepository.SQLConnectionStrings();
        }

        /*----------前台會員相關----------*/
        //取得前台會員資料
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
                cmd.CommandText = @"EXEC pro_onlineShopBack_getMemberList ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);

                //關閉連線
                cmd.Connection.Close();
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
            return dt;
        }

        //取得指定會員資料
        public DataTable GetMemberByAcc(string Acc)
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
                cmd.CommandText = @"EXEC pro_onlineShopBack_getMemberByAcc @acc";

                cmd.Parameters.AddWithValue("@acc", Acc);

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);

                //關閉連線
                cmd.Connection.Close();


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
            return dt;
        }

        //刪除會員
        public int DelMember(int id)
        {
            int ResultCode = (int)MemberEnum.DelMemberCode.Defult;

            SqlCommand cmd = null;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_delMember @id";

                cmd.Parameters.AddWithValue("@id", id);

                //開啟連線
                cmd.Connection.Open();
                ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
                ResultCode = (int)MemberEnum.DelMemberCode.ExceptionError;
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

        //編輯會員(等級&狀態)
        public int EditMember(int id, MemberDto value)
        {
            int ResultCode = (int)MemberEnum.PutMemberCode.Defult;

            string addAccErrorStr = "";//記錄錯誤訊息
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
                MyTool.WriteErroLog(addAccErrorStr);
                ResultCode = (int)MemberEnum.PutMemberCode.ValidaFail;
            }
            else
            {

                SqlCommand cmd = null;

                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_putMemberByLvAndSuspension @Id, @Lv, @Suspension";

                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Lv", value.Level);
                    cmd.Parameters.AddWithValue("@Suspension", value.Suspension);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)MemberEnum.PutMemberCode.ExceptionError;//執行Transact-SQL
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

        //調整購物金
        public int EditShopGold(PutShopGlodDto value)
        {
            int ResultCode = (int)MemberEnum.EditShopGoldCode.Defult;

            string addAccErrorStr = "";//記錄錯誤訊息


            //最後修改金額(PutShopGold)=(原始金額(NowAmount)+調整金額(AdjustAmount))
            int? PutShopGold = 0;

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
            if (PutShopGold > 200000 || PutShopGold < 0)
            {
                addAccErrorStr += "調整後不得小於0 or 大於200000";
            }

            if (!string.IsNullOrEmpty(addAccErrorStr))
            {
                MyTool.WriteErroLog(addAccErrorStr);
                ResultCode = (int)MemberEnum.EditShopGoldCode.ValidaFail;
            }

            SqlCommand cmd = null;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_putMemberByShopGold @MemAcc,  @NowAmount, @PutShopGold ";

                cmd.Parameters.AddWithValue("@MemAcc", value.MemAcc);
                cmd.Parameters.AddWithValue("@NowAmount", value.NowAmount);
                cmd.Parameters.AddWithValue("@PutShopGold", PutShopGold);

                //開啟連線
                cmd.Connection.Open();
                ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL
            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
                ResultCode = (int)MemberEnum.EditShopGoldCode.ExceptionError;
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

        /*----------前台會員等級相關----------*/
        //取得會員等級資料List
        public DataTable GetMemLvList()
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = null;
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
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
            return dt;
        }

        //添加會員等級 
        public int AddMemLv(MemLvDto value)
        {
            int ResultCode = (int)MemberEnum.AddMemLvCode.Defult;

            string AddMemLvErrorStr = "";//記錄錯誤訊息
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
                MyTool.WriteErroLog(AddMemLvErrorStr);
                ResultCode = (int)MemberEnum.AddMemLvCode.ValidaFail;
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
                    cmd.CommandText = @"EXEC pro_onlineShopBack_addMemberLevel @memLv, @LvName";

                    cmd.Parameters.AddWithValue("@memLv", value.memLv);
                    cmd.Parameters.AddWithValue("@LvName", value.LvName);

                    //開啟連線

                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)MemberEnum.AddMemLvCode.ExceptionError;
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

        //更新權限
        public int EditMemLv(int MemLv, MemLvDto value)
        {
            int ResultCode = (int)MemberEnum.PutMemLVCode.Defult;

            string addAccLVErrorStr = "";//記錄錯誤訊息
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
                MyTool.WriteErroLog(addAccLVErrorStr);
                ResultCode = (int)MemberEnum.PutMemLVCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;

                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_putMemberLevelById @MemLv, @LvName ";

                    cmd.Parameters.AddWithValue("@MemLv", MemLv);
                    cmd.Parameters.AddWithValue("@LvName", value.LvName);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL
                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)MemberEnum.PutMemLVCode.ExceptionError;
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

        //刪除會員等級
        public int DelMemLv(int memLv)
        {
            int ResultCode = (int)MemberEnum.DelMemLvCode.Defult;
            string addAccLVErrorStr = "";//記錄錯誤訊息
            //權限編號 
            if (memLv > 255 || memLv < 0)
            {
                addAccLVErrorStr += "[會員等級編號長度應介於0～255個數字之間]\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(addAccLVErrorStr))
            {
                MyTool.WriteErroLog(addAccLVErrorStr);
                ResultCode = (int)MemberEnum.DelMemLvCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_delMemberLevelById @memLv";

                    cmd.Parameters.AddWithValue("@memLv", memLv);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)MemberEnum.DelMemLvCode.ExceptionError;
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

        /*----------前台會員狀態相關----------*/
        //取得狀態資料List
        public DataTable GetSuspensionList()
        {

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
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
            return dt;
        }

        //增加狀態 
        public int AddSuspension(suspensionDto value)
        {
            int ResultCode = (int)MemberEnum.AddSuspensionCode.Defult;

            string ErrorStr = "";//記錄錯誤訊息
            //資料驗證
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
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)MemberEnum.AddSuspensionCode.ValidaFail;
            }
            else
            {

                SqlCommand cmd = null;
                string SQLReturnCode = "";
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    //重複驗證寫在SP中
                    cmd.CommandText = @"EXEC pro_onlineShopBack_addsuspension @Lv, @Name";

                    cmd.Parameters.AddWithValue("@Lv", value.suspensionLv);
                    cmd.Parameters.AddWithValue("@Name", value.suspensionName);

                    //開啟連線

                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)MemberEnum.AddSuspensionCode.ExceptionError;
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

        //更新狀態
        public int EditSuspension(int id, suspensionDto value)
        {
            int ResultCode = (int)MemberEnum.EditSuspensionCode.Defult;
            string addAccLVErrorStr = "";//記錄錯誤訊息
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
                MyTool.WriteErroLog(addAccLVErrorStr);
                ResultCode = (int)MemberEnum.EditSuspensionCode.ValidaFail;
            }

            SqlCommand cmd = null;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_putSuspension @id, @Name ";

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@Name", value.suspensionName);

                //開啟連線
                cmd.Connection.Open();
                ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
                ResultCode = (int)MemberEnum.EditSuspensionCode.ExceptionError;
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

        //刪除等級
        public int DelSuspension(int id)
        {
            int ResultCode = (int)MemberEnum.DelSuspensionCode.Defult;

            string ErrorStr = "";//記錄錯誤訊息

            //權限編號 
            if (id > 255 || id < 0)
            {
                ErrorStr += "[會員等級編號長度應介於0～255個數字之間]\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)MemberEnum.DelSuspensionCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_delSuspensionById @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)MemberEnum.DelSuspensionCode.ExceptionError;
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
