#region 功能與歷史修改描述
/*
    描述:前台會員系統相關
    日期:2022-05-05
*/
#endregion
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Domain.DTOs.Member;
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
    public class MemberController : ControllerBase
    {

        private readonly IMemberRepository _MemberService = null;
        public MemberController(IMemberRepository MemberService)
        {
            _MemberService = MemberService;
        }
        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        /*----------前台會員相關----------*/

        //取得前台會員資料
        [HttpGet("GetMember")]
        public string GetMember()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            DataTable dt = new DataTable();
            dt = _MemberService.GetAccountAndLevelList();

            MemberDto[] MemberList = dt.Rows.Cast<DataRow>()
                .Select(row => MemberDto.GetMemberList(row))
                .Where(accTuple => accTuple.Item1 == true)
                .Select(accTuple => accTuple.Item2)
                .ToArray();

            string Result = JsonSerializer.Serialize(MemberList);//序列化回傳  回傳型態 string
            return Result;
        }

        //取得指定會員資料
        [HttpGet("GetMemberByAcc")]
        public string GetMemberByAcc([FromQuery] string Acc)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            if (RolesValidate())
            {
                return "無使用權限";
            }

            DataTable dt = new DataTable();
            dt = _MemberService.GetMemberByAcc(Acc);


            MemberDto[] MemberListByAcc = dt.Rows.Cast<DataRow>()
               .Select(row => MemberDto.GetMemberListByAcc(row))
               .Where(accTuple => accTuple.Item1 == true)
               .Select(accTuple => accTuple.Item2)
               .ToArray();

            string Result = JsonSerializer.Serialize(MemberListByAcc);//序列化回傳  回傳型態 string
            return Result;
        }

        //刪除會員
        [HttpDelete("DelMember")]
        public string DelMember([FromQuery] int id)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }
            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _MemberService.DelMember(id);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //編輯會員(等級&狀態)
        [HttpPut("EditMember")]
        public string EditMember([FromQuery] int id, [FromBody] MemberDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _MemberService.EditMember(id, value);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //調整購物金
        [HttpPut("EditShopGold")]
        public string EditShopGold([FromBody] PutShopGlodDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _MemberService.EditShopGold(value);

            return "[{\"st\": " + ResultCode + "}]";
        }

        /*----------前台會員等級相關----------*/

        //取得會員等級資料List
        [HttpGet("GetMemLvList")]
        public string GetMemLvList()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            DataTable dt = _MemberService.GetMemLvList();

            MemLvDto[] MemLvList = dt.Rows.Cast<DataRow>()
              .Select(row => MemLvDto.GetMemLvList(row))
              .Where(accTuple => accTuple.Item1 == true)
              .Select(accTuple => accTuple.Item2)
              .ToArray();

            string Result = JsonSerializer.Serialize(MemLvList);//序列化回傳  回傳型態 string
            return Result;
        }

        //添加會員等級 
        [HttpPost("AddMemLv")]
        public string AddMemLv([FromBody] MemLvDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _MemberService.AddMemLv(value);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //更新權限
        [HttpPut("EditMemLv")]
        public string EditMemLv([FromQuery] int MemLv, [FromBody] MemLvDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _MemberService.EditMemLv(MemLv, value);

            return "[{\"st\": " + ResultCode + "}]";

        }

        //刪除會員等級
        [HttpDelete("DelMemLv")]
        public string DelMemLv([FromQuery] int memLv)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _MemberService.DelMemLv(memLv);

            return "[{\"st\": " + ResultCode + "}]";
        }


        /*----------前台會員狀態相關----------*/

        //取得狀態資料List
        [HttpGet("GetSuspensionList")]
        public string GetSuspensionList()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }
            DataTable dt = _MemberService.GetSuspensionList();

            suspensionDto[] SuspensionList = dt.Rows.Cast<DataRow>()
                .Select(row => suspensionDto.GetSuspensionList(row))
                .Where(Tuple => Tuple.Item1 == true)
                .Select(Tuple => Tuple.Item2)
                .ToArray();

            string Result = JsonSerializer.Serialize(SuspensionList);//序列化回傳  回傳型態 string
            return Result;
        }

        //增加狀態 
        [HttpPost("AddSuspension")]
        public string AddSuspension([FromBody] suspensionDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }



            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _MemberService.AddSuspension(value);

            return "[{\"st\": " + ResultCode + "}]";

        }

        //更新狀態
        [HttpPut("EditSuspension")]
        public string EditSuspension([FromQuery] int id, [FromBody] suspensionDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _MemberService.EditSuspension(id, value);

            return "[{\"st\": " + ResultCode + "}]";

        }

        //刪除等級
        [HttpDelete("DelSuspension")]
        public string DelSuspension([FromQuery] int id)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _MemberService.DelSuspension(id);

            return "[{\"st\": " + ResultCode + "}]";
        }

        /*-------------------------------------------*/
        //登入檢查
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
        //權限檢查
        private bool RolesValidate()
        {

            if (HttpContext.Session.GetString("Roles").Contains("canUseMember"))
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
