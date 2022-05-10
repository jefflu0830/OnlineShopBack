using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static OnlineShopBack.Pages.Member.BasePage;

namespace OnlineShopBack.Pages.Member
{
    public class AddShopGoldModel : BasePageModel
    {
        public void OnGet()
        {
            MemberValidate();
        }
    }
}
