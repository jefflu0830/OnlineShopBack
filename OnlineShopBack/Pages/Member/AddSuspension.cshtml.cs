using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static OnlineShopBack.Pages.Member.BasePage;

namespace OnlineShopBack.Pages.Account
{
    public class AddSuspensionModel : BasePageModel
    {
        public void OnGet()
        {
            MemberValidate();
        }
    }
}
