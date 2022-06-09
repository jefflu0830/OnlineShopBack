using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System;
using System.Data;
using static OnlineShopBack.Pages.Order.BasePage;

namespace OnlineShopBack.Pages.Order
{
    public class AddTransportModel :  BasePageModel
    {
        //SQL�B���ִ�  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");
        public string TransportJson;

        public void OnGet()
        {

            //��C����
            if (!LoginValidate())
            {
                Response.Redirect("/Login");
                return;
            }
            //��C�_ɫ
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
                // �Y�ώ��B��&SQLָ��
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" SELECT f_transport, f_transportName FROM t_transport ";

                //�_���B��
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
                //�P�]�B��
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            //DataTable�DJson;            
            TransportJson = MyTool.DataTableJson(dt);


        }
    }
}
