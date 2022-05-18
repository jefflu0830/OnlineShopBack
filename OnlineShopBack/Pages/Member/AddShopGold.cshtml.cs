using static OnlineShopBack.Pages.Member.BasePage;

namespace OnlineShopBack.Pages.Member
{
    public class AddShopGoldModel : BasePageModel
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
