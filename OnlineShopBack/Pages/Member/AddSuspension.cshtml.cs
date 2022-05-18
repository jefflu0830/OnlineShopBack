using static OnlineShopBack.Pages.Member.BasePage;

namespace OnlineShopBack.Pages.Account
{
    public class AddSuspensionModel : BasePageModel
    {
        public void OnGet()
        {
            //òž×CµÇÈë
            if (!LoginValidate())
            {
                Response.Redirect("/Login");
                return;
            }
            //òž×CÄ_É«
            if (!RolesValidate())
            {
                Response.Redirect("/index");
                return;
            }

        }
    }
}
