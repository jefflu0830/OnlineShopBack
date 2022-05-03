using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineShopBack.Pages.Account
{
    [Authorize(Roles = "canUseAccount")]
    public class PutPwdModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
