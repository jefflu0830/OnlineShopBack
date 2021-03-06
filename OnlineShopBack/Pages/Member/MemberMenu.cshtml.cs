#region 功能與歷史修改描述
/*
    描述:前台會員清單頁面
    日期:2022-05-05
*/
#endregion
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using System.Data;
using static OnlineShopBack.Pages.Member.BasePage;

namespace OnlineShopBack.Pages.Member
{
    public class MemberMenuModel : BasePageModel
    {
        //private static string SQLConnectionString = AppConfigurationService.SQLConnectionString;
        private static string SQLConnectionString = AppConfigurationService.GetConnectionStr();
        public string Level;
        public string LevelName;
        public string Suspension;
        public string SuspensionName;
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
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();

            // 資料庫連線&SQL指令
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            cmd.CommandText = @"SELECT f_memberLevel, f_LevelName  FROM t_memberLevel  
                                SELECT f_suspensionLv, f_suspensionName FROM t_suspensionLevel "; //改成SP

            //開啟連線
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(ds);

            //關閉連線
            cmd.Connection.Close();
            //t_memberLevel
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Level += ds.Tables[0].Rows[i][0].ToString() + "/";
                LevelName += ds.Tables[0].Rows[i][1] + "/";
            }
            //t_suspensionLevel
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                Suspension += ds.Tables[1].Rows[i][0].ToString() + "/";
                SuspensionName += ds.Tables[1].Rows[i][1] + "/";
            }
        }
    }
}
