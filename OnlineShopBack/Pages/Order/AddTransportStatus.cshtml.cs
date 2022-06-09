using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System;
using System.Data;
using static OnlineShopBack.Pages.Order.BasePage;

namespace OnlineShopBack.wwwroot.js.Order
{
    public class AddTransportStatusModel : BasePageModel
    {
        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");
        public string TransportJson;
        public string TransportStatusJson;
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
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" SELECT f_transport, f_transportName FROM t_transport  
                                     SELECT f_transport, f_transportStatus,f_transportStatusName FROM t_transportStatus ";

                //開啟連線
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
                //關閉連線
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            //DataTable轉Json;            
            TransportJson = MyTool.DataTableJson(ds.Tables[0]);
            TransportStatusJson = MyTool.DataTableJson(ds.Tables[1]);
        }
    }
}
