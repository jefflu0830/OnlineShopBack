using Microsoft.AspNetCore.Authorization;
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

        public string AccLevel;
        public string AccPosition;

        public void OnGet()
        {
            var item = new List<SelectListItem>();
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // �Y�ώ��B��&SQLָ��
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            cmd.CommandText = @"SELECT * FROM T_accountLevel "; //�ĳ�SP

            //�_���B��
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //�P�]�B��
            cmd.Connection.Close();

            for (int i=0; i<dt.Rows.Count;i++ )
            {
                AccLevel += dt.Rows[i][0].ToString() +"/";
                AccPosition += dt.Rows[i][1]+"/";
            }
        }
    }
}
