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
    }
}
