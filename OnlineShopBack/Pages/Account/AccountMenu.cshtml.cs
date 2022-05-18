using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
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

            // �Y�ώ��B��&SQLָ��
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            cmd.CommandText = @"SELECT * FROM T_accountLevel ";

            //�_���B��
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //�P�]�B��
            cmd.Connection.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                AccLevel += dt.Rows[i][0].ToString() + "/";
                AccPosition += dt.Rows[i][1] + "/";
            }


        }
    }
}
