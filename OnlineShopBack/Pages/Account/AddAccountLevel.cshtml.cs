using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static OnlineShopBack.Pages.Account.BasePage;

namespace OnlineShopBack.Pages.Account
{
    public class AddAccountLevelModel : BasePageModel
    {
        public void OnGet()
        {
            AccountValidate();
        }
    }
}
