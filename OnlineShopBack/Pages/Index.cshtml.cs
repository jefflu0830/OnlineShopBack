using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineShopBack.Pages
{
    [Authorize]
    public class BackPageModel : PageModel
    {
        public string Session01;
        public void OnGet()
        {
            Session01 = HttpContext.Session.GetString("SessionID");

        }
    }
}
