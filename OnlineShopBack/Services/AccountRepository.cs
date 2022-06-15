using Microsoft.Data.SqlClient;
using OnlineShopBack.Domain.Repository;
using static OnlineShopBack.Tool.MyTool;
using System;
using System.Data;

namespace OnlineShopBack.Persistent
{
    public class AccountRepository : IAccountRepository
    {
        //private string SQLConnectionString = "Data Source=I577;Initial Catalog=OnlineShop;Integrated Security=True";
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
                WriteErroLog(e.Message);
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
