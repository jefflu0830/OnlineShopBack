using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineShopBack.Pages.Member
{
    [Authorize(Roles = "canUseMember")]
    public class SelectMemberModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
