using static OnlineShopBack.Pages.Product.BasePage;

namespace OnlineShopBack.Pages.Product
{
    public class AddCategoryModel : BasePageModel
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
