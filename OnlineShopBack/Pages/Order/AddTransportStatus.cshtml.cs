#region �����c�vʷ�޸�����
/*
    ����:�������͠�B���
    ����:2022-05-05
*/
#endregion
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;
using static OnlineShopBack.Pages.Order.BasePage;
using OnlineShopBack.Domain.DTOs.Order;
using System.Linq;
using System.Text.Json;

namespace OnlineShopBack.wwwroot.js.Order
{
    public class AddTransportStatusModel : BasePageModel
    {
        //SQL�B���ִ�  SQLConnectionString
        //private static string SQLConnectionString = AppConfigurationService.SQLConnectionString;
        private static string SQLConnectionString = AppConfigurationService.GetConnectionStr();
        public string TransportJson;
        public string TransportStatusJson;
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
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // �Y�ώ��B��&SQLָ��
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" SELECT f_transport, f_transportName FROM t_transport  
                                     SELECT f_transport, f_transportStatus,f_transportStatusName FROM t_transportStatus ";

                //�_���B��
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
                //�P�]�B��
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }

            TransportDto[] TransportList = ds.Tables[0].Rows.Cast<DataRow>()
                .Select(row => TransportDto.GetTransportList(row))
                .Where(accTuple => accTuple.Item1 == true)
                .Select(accTuple => accTuple.Item2)
                .ToArray();

            TransportDto[] TransportStatusList = ds.Tables[1].Rows.Cast<DataRow>()
                .Select(row => TransportDto.GetTransportStatusList(row))
                .Where(accTuple => accTuple.Item1 == true)
                .Select(accTuple => accTuple.Item2)
                .ToArray();


            TransportJson = JsonSerializer.Serialize(TransportList);
            TransportStatusJson = JsonSerializer.Serialize(TransportStatusList);
        }
    }
}
