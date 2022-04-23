using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Models;
using OnlineShopBack.Services;
using System.Collections.Generic;
using System.Data;


namespace OnlineShopBack.Pages.Account
{
    public class AddAccountModel : PageModel
    {
        private static string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        public string Acclevel01;
        public string Acclevel02;

        public void OnGet()
        {
            var item = new List<SelectListItem>();
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

            string accLevel = "";
            string accPosition = "";

            for (int i=0; i<dt.Rows.Count;i++ )
            {
                accLevel += dt.Rows[i][0].ToString() +"/";
                accPosition += dt.Rows[i][1]+"/";
                Acclevel02 += dt.Rows[i][1]+"/";
            }

            ViewData["accLevel"] += accLevel;
            ViewData["accPosition"] += accPosition;
        }
    }
}
