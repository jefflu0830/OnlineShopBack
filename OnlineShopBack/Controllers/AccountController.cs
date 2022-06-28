#region 功能與歷史修改描述
/*
    描述:後台帳號系統相關
    日期:2022-05-05
*/
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Domain.DTOs.Account;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Domain.Tool;
using OnlineShopBack.Services;
using System;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountService = null;
        public AccountController(IAccountRepository accountService)
        {
            _accountService = accountService;
        }


        /*-----------後臺帳號相關-----------*/

        //取得全部帳號資訊
        [HttpGet("GetAcc")]
        public string GetAcc()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

            DataTable dt = _accountService.GetAccountAndLevelList();

            AccountDto[] accountList = dt.Rows.Cast<DataRow>()   //dt.Rows 要轉成 IEnumerable 要下 Cast<>  要轉成 DataRow型態 因此變成 Cast<DataRow>()
                .Select(row => AccountDto.GrenerateInstance(row))//將dt丟入 Dto中的 GrenerateInstance 進行處理
                .Where(accTuple => accTuple.Item1 == true)       //篩選條件 為  第一項回傳直為 true
                .Select(accTuple => accTuple.Item2)              //篩選完 在Select一次  出第二項
                .ToArray();                                     //最後要轉成 Array

            string Result = JsonSerializer.Serialize(accountList);//序列化回傳  回傳型態 string
            return Result;
        }

        //增加帳號
        [HttpPost("AddAcc")]
        public string AddAcc([FromBody] Domain.DTOs.Account.AccountDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //_accountService.AddAccount 新增帳號
            int ResultCode = _accountService.AddAccount(value);


            return "[{\"st\": " + ResultCode + "}]";

        }

        //編輯帳號_權限
        [HttpPut("EditAcc")]
        public string EditAcc([FromQuery] int id, [FromBody] Domain.DTOs.Account.AccountDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _accountService.EditAcc(id, value);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //更新帳號_密碼
        [HttpPut("EditPwd")]
        public string EditPwd([FromBody] Domain.DTOs.Account.PutPwdDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _accountService.EditPwd(value);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //刪除帳號
        [HttpDelete("DelAcc")]
        public string DelAcc([FromQuery] int id)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _accountService.DelAcc(id);

            return "[{\"st\": " + ResultCode + "}]";
        }

        /*-----------後臺帳號權限相關-----------*/
        //權限資料List
        [HttpGet("GetAccLvList")]
        public string GetAccLvList()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

            DataTable dt = _accountService.GetAccLvList();

            AccountLevelDto[] accountLevelList = dt.Rows.Cast<DataRow>()   
                .Select(row => AccountLevelDto.GetAccLvList(row))
                .Where(accTuple => accTuple.Item1 == true)      
                .Select(accTuple => accTuple.Item2)              
                .ToArray();        
            
            string Result = JsonSerializer.Serialize(accountLevelList);//序列化回傳  回傳型態 string
            return Result;
        }

        //依照ID查詢權限資料
        [HttpGet("GetAccLVById")]
        public string GetAccLVById([FromQuery] int id)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }
            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            DataTable dt = _accountService.GetAccLvById(id);


            AccountLevelDto[] AccLvById = dt.Rows.Cast<DataRow>()
                .Select(row => AccountLevelDto.GetAccLvById(row))
                .Where(accTuple => accTuple.Item1 == true)
                .Select(accTuple => accTuple.Item2)
                .ToArray();

            string Result = JsonSerializer.Serialize(AccLvById);//序列化回傳  回傳型態 string
            return Result;
        }

        //增加權限
        [HttpPost("AddAccLv")]
        public string AddAccLv([FromBody] Domain.DTOs.Account.AccountLevelDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _accountService.AddAccLv(value);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //更新權限
        [HttpPut("EditAccLv")]
        public string EditAccLv([FromQuery] int id, [FromBody] Domain.DTOs.Account.AccountLevelDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _accountService.EditAccLv(id, value);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //刪除權限
        [HttpDelete("DelAccLv")]
        public string DelAccLv([FromQuery] int id)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }



            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }



            int ResultCode = _accountService.DelAccLv(id);

            return "[{\"st\": " + ResultCode + "}]";
        }



        //登入&權限檢查
        private bool loginValidate()
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||                        //判斷Session[Account]是否為空
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].SId != HttpContext.Session.Id ||//判斷DB SessionId與瀏覽器 SessionId是否一樣
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].ValidTime < DateTime.Now)       //判斷是否過期
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool RolesValidate()
        {
            if (HttpContext.Session.GetString("Roles").Contains("canUseAccount"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }


}

