using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using System.Data;
using System.Text;

namespace OnlineShopBack.Controllers
{
    [Authorize(Roles = "canUseMember")]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");



        //[POST]  增加會員等級 t_memberLevel
        [HttpPost("AddMemberLevel")]
        public string AddMemberLevel([FromBody] AccountSelectDto value)  // t_memberLevel 的DTO
        {
            return "AddMemberLevel API ";
        }


        //取得會員資料
        [HttpGet("GetMember")]
        //public IEnumerable<AccountSelectDto> Get()
        public string GetMember()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // 資料庫連線&SQL指令
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            cmd.CommandText = @"EXEC pro_onlineShopBack_getMember ";

            //開啟連線
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //關閉連線
            cmd.Connection.Close();

            //DataTable轉Json;
            var result = Tool.MyTool.DataTableJson(dt);

            return result;
        }
    }
}
