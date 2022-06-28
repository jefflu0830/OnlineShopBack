#region 功能與歷史修改描述
/*
    描述:後台帳號清單頁面
    日期:2022-05-05
*/
#endregion

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;
using static OnlineShopBack.Pages.Account.BasePage;

namespace OnlineShopBack.Pages.Account
{
    public class AccountMenuModel : BasePageModel
    {
        //private static string SQLConnectionString = AppConfigurationService.SQLConnectionString;
        private static string SQLConnectionString = AppConfigurationService.GetConnectionStr();
        public string AccLevel;
        public string AccPosition;
        public void OnGet()
        {
            //驗證登入
            if (!LoginValidate())
            {
                Response.Redirect("/Login");
                return;
            }
            //驗證腳色
            if (!RolesValidate())
            {
                Response.Redirect("/index");
                return;
            }

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" SELECT f_accLevel, f_accPosition FROM T_accountLevel ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);

                //關閉連線
                cmd.Connection.Close();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    AccLevel += dt.Rows[i][0].ToString() + "/";
                    AccPosition += dt.Rows[i][1] + "/";
                }
            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Connection.Close();
                    cmd.Parameters.Clear();

                }
            }


        }
    }
}
