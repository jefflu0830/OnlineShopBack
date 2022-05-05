using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineShopBack.Pages.Account
{
    [Authorize(Roles = "canUseMember")]
    public class AddSuspensionModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
