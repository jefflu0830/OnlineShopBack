using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System.Data;
using static OnlineShopBack.Pages.Account.BasePage;

namespace OnlineShopBack.Pages.Account
{
    public class AccountMenuModel : BasePageModel
    {
        private static string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");
        public string AccLevel;
        public string AccPosition;
        public void OnGet()
        {
            AccountValidate();

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // ÙYÁÏŽìßB¾€&SQLÖ¸Áî
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            cmd.CommandText = @"SELECT * FROM T_accountLevel "; 

            //é_†¢ßB¾€
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //êPé]ßB¾€
            cmd.Connection.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                AccLevel += dt.Rows[i][0].ToString() + "/";
                AccPosition += dt.Rows[i][1] + "/";
            }

            
        }
    }
}
