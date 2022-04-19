using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using System.Data;
using System.Text;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");
        //取得會員資料
        [HttpGet("{id}")]
        //public IEnumerable<AccountSelectDto> Get()
        public string Get(string id)
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();

            // 資料庫連線
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);

            SqlDataAdapter da = new SqlDataAdapter();

            cmd.CommandText = @"EXEC pro_onlineShopBack_selectMember ";

            //開啟連線
            cmd.Connection.Open();

            da.SelectCommand = cmd;
            da.Fill(dt);
            cmd.Connection.Close();

            //return JsonConvert.SerializeObject(dt);
            var result = DataTableJson(dt);


            /*var result = _OnlineShopContext.TAccount
                .Select(a => new AccountSelectDto
                {
                    Id = a.FId,
                    Account = a.FAcc,
                    Pwd = a.FPwd,
                    Level = a.FLevel
                });*/

            return result;
        }

        //DataTable 转换成JSON数据
        public string DataTableJson(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sb.Append("\"");
                    sb.Append(dt.Columns[j].ColumnName);
                    sb.Append("\":\"");
                    sb.Append(dt.Rows[i][j].ToString());
                    sb.Append("\",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
