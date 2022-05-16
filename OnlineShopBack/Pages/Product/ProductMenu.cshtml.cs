using static OnlineShopBack.Pages.Account.BasePage;

namespace OnlineShopBack.Pages.Product
{
    public class ProductMenuModel : BasePageModel
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
