#region 功能與歷史修改描述
/*
    描述:後台帳號系統相關
    日期:2022-05-05
*/
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Services;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //取得SQL連線字串
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");


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

            //DataTable轉Json;
            string result = MyTool.DataTableJson(dt);

            return result;
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
            //DataTable轉Json;
            string result = MyTool.DataTableJson(dt);

            return result;
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

            //DataTable轉Json;
            string result = MyTool.DataTableJson(dt);

            return result;
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
          
            int ResultCode = _accountService.EditAccLv(id,value);

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

