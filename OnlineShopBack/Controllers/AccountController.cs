﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Models;
using OnlineShopBack.Services;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly OnlineShopContext _OnlineShopContext;
        public AccountController(OnlineShopContext onlineShopContext)
        {
            _OnlineShopContext = onlineShopContext;
        }


        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase"); 


        //取得會員資料
        [HttpGet]
        //public IEnumerable<AccountSelectDto> Get()
        public string Get()
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
            var result=DataTableJson(dt);


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

        // GET api/<AccuntController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AccuntController>
        [HttpGet("AddAccount")]
        [HttpPost]
        public string Post([FromBody] AccountSelectDto value)
        {
            SqlCommand cmd = null;
            //DataTable dt = new DataTable();
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                //SqlDataAdapter da = new SqlDataAdapter();

                cmd.CommandText = @"EXEC pro_onlineShopBack_addAccount @f_acc, @f_pwd, @f_level";

                cmd.Parameters.AddWithValue("@f_acc", value.Account);
                cmd.Parameters.AddWithValue("@f_pwd", LoginController.PswToMD5(value.Pwd));
                cmd.Parameters.AddWithValue("@f_level", value.Level);

                //開啟連線
                cmd.Connection.Open();
                cmd.ExecuteNonQuery(); //執行Transact-SQL
                cmd.Connection.Close();

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }

            /*using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(value.Pwd));//MD5 加密傳密碼進去
                var strResult = BitConverter.ToString(result);

                TAccount insert = new TAccount
                {
                    FAcc = value.Account,
                    FPwd = strResult.Replace("-",""),
                    FLevel = value.Level 
                };
                _OnlineShopContext.Add(insert);
                _OnlineShopContext.SaveChanges();
               return "新增成功"; 
            }*/
            return "新增成功";
        }
        // PUT api/<AccuntController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        // DELETE api/<AccuntController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
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

