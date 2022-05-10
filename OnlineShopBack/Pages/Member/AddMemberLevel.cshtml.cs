using static OnlineShopBack.Pages.Member.BasePage;

namespace OnlineShopBack.Pages.Account
{
    public class AddMemberLevelModel : BasePageModel
    {
        public void OnGet()
        {
            MemberValidate();
        }
    }
}
