using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static OnlineShopBack.Pages.Account.BasePage;

namespace OnlineShopBack.Pages.Account
{
    public class AddAccountLevelModel : BasePageModel
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
