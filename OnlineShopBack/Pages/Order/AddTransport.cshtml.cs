using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System;
using System.Data;

namespace OnlineShopBack.Pages.Order
{
    public class AddTransportModel : PageModel
    {
        //SQLßB¾€×Ö´®  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");
        public string TransportJson;

        public void OnGet()
        {

            SqlCommand cmd = null;
            DataTable dt = new DataTable();            
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // ÙYÁÏŽìßB¾€&SQLÖ¸Áî
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" SELECT f_transport, f_transportName FROM t_transport ";

                //é_†¢ßB¾€
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception e)
            {
                //return e.Message;
            }
            finally
            {
                //êPé]ßB¾€
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            //DataTableÞDJson;            
            TransportJson = MyTool.DataTableJson(dt);


        }
    }
}
