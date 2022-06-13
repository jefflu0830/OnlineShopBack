#region 功能與歷史修改描述
/*
    描述:後台登入頁面
    日期:2022-05-05
*/
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OnlineShopBack.Tool;

namespace OnlineShopBack.Pages
{
    public class IndexModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) &&
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].SId == HttpContext.Session.Id)
            {
                Response.Redirect("/index");
            }

        }
    }
}
