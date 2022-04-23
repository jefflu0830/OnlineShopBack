﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Models;
using OnlineShopBack.Pages.Account;
using OnlineShopBack.Services;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OnlineShopBack.Tool;


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

        [HttpGet]
        public IEnumerable<AccountSelectDto> Get()
        {
            var result = _OnlineShopContext.TAccount
                .Select(a => new AccountSelectDto
                {
                    Id = a.FId,
                    Account = a.FAcc,
                    Pwd = a.FPwd,
                    Level = a.FLevel
                });

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
            //後端驗證
            //如字串字數特殊字元驗證

            string addAccErrorStr = "";


            if (value.Account == "" || value.Pwd == "")
            {
                addAccErrorStr += "[帳號或密碼不可為空]";
            }

            if (value.Account != "")
            {
                if (!MyTool.IsENAndNumber(value.Account))
                {
                    addAccErrorStr += "[＊帳號只能為英數]\n";
                }
                if (value.Account.Length > 20 || value.Account.Length < 3)
                {
                    addAccErrorStr += "[＊帳號長度應介於3～20個數字之間]\n";
                }
            };
            if (value.Pwd != "")
            {
                if (!MyTool.IsENAndNumber(value.Pwd))
                {
                    addAccErrorStr += "[＊密碼只能為英數]\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    addAccErrorStr += "[＊密碼長度應應介於8～16個數字之間]\n";
                }
            }
            if (value.Level>255 || value.Level<0)
            {
                addAccErrorStr += "[＊該權限不再範圍內]\n";
            }

            if (addAccErrorStr != "") 
            {
                return addAccErrorStr;
            }
            else
            {
                SqlCommand cmd = null;
                //DataTable dt = new DataTable();
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(SQLConnectionString);
                    

                    //帳號重複驗證寫在SP中

                    cmd.CommandText = @"EXEC pro_onlineShopBack_addAccount @f_acc, @f_pwd, @f_level";

                    cmd.Parameters.AddWithValue("@f_acc", value.Account);
                    cmd.Parameters.AddWithValue("@f_pwd", Tool.MyTool.PswToMD5(value.Pwd));
                    cmd.Parameters.AddWithValue("@f_level", value.Level);

                    #region //SQL回傳是 Return時的接法
                    //SqlParameter returnValue = new SqlParameter("XXX", SqlDbType.Int);
                    //returnValue.Direction = ParameterDirection.ReturnValue;
                    //cmd.Parameters.Add(returnValue);


                    //return returnValue.Value.ToString();
                    #endregion

                    //開啟連線
                    cmd.Connection.Open();
                    addAccErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                    return addAccErrorStr;

                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
                }
                 
            }

            #region EF舊寫法已註解
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
            #endregion

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
    }

}

