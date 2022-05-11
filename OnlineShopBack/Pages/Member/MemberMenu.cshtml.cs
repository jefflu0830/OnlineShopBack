using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using System.Data;
using static OnlineShopBack.Pages.Member.BasePage;

namespace OnlineShopBack.Pages.Member
{
    public class MemberMenuModel : BasePageModel
    {
        private static string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");
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
            cmd.CommandText = @"SELECT * FROM t_memberLevel 
                                SELECT * FROM t_suspensionLevel "; //改成SP

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
