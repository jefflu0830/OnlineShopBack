using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System;
using System.Data;

namespace OnlineShopBack.wwwroot.js.Order
{
    public class AddTransportStatusModel : PageModel
    {
        //SQLßB¾€×Ö´®  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");
        public string TransportJson;
        public string TransportStatusJson;
        public void OnGet()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // ÙYÁÏŽìßB¾€&SQLÖ¸Áî
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" SELECT f_transport, f_transportName FROM t_transport  
                                     SELECT f_transport, f_transportStatus,f_transportStatusName FROM t_transportStatus ";

                //é_†¢ßB¾€
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
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
            TransportJson = MyTool.DataTableJson(ds.Tables[0]);
            TransportStatusJson = MyTool.DataTableJson(ds.Tables[1]);
        }
    }
}
