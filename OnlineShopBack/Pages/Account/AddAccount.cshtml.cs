using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Controllers;
using OnlineShopBack.Models;
using OnlineShopBack.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OnlineShopBack.Pages.Account
{
    public class AddAccountModel : PageModel
    {
        private static string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");


        public void OnGet()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // ÙYÁÏŽìßB¾€&SQLÖ¸Áî
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            cmd.CommandText = @"EXEC pro_onlineShopBack_getMember ";

            //é_†¢ßB¾€
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //êPé]ßB¾€
            cmd.Connection.Close();          

        }
    }
}
