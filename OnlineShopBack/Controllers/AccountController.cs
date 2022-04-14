using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShopBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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


        // GET: api/<AccuntController>
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
        [HttpPost]
        //public string Post([FromBody] TAccount value)
        public string Post([FromBody] AccountSelectDto value)
        {
            using (var md5 = MD5.Create())
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
            }
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
