using OnlineShopBack.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineShopBack.Persistent
{

    public class AccountLevel
    {
        public Account account;
        public Level level;
    }

    public class Account { }

    public class Level { }
    public class AccountRepository : IAccountRepository
    {
        //public (Account[], Level[]) GetAccountAndLevelList()
        //public (Account, Level)[] GetAccountAndLevelList()
        public AccountLevel[] GetAccountAndLevelList(AccountRequestDTO aaa)
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

            List<(string, int)> list = new List<(string, int)>();

            foreach(DataRow raw in dt.Rows)
            {

            }

            return list.ToArray();
        }
    }
}
