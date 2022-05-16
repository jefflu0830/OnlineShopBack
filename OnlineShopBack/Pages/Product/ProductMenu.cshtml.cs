using static OnlineShopBack.Pages.Account.BasePage;

namespace OnlineShopBack.Pages.Product
{
    public class ProductMenuModel : BasePageModel
    {
        public void OnGet()
        {
            //��C����
            if (!LoginValidate())
            {
                Response.Redirect("/Login");
                return;
            }
            //��C�_ɫ
            if (!RolesValidate())
            {
                Response.Redirect("/index");
                return;
            }
        }
    }
}
