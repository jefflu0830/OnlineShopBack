using static OnlineShopBack.Pages.Member.BasePage;

namespace OnlineShopBack.Pages.Member
{
    public class AddShopGoldModel : BasePageModel
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
