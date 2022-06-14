#region 功能與歷史修改描述
/*
    描述:新增產品頁面
    日期:2022-05-05
*/
#endregion
using static OnlineShopBack.Pages.Product.BasePage;

namespace OnlineShopBack.Pages.Product
{
    public class AddProductModel : BasePageModel
    {
        public void OnGet()
        {
            //驗證登入
            if (!LoginValidate())
            {
                Response.Redirect("/Login");
                return;
            }
            //驗證腳色
            if (!RolesValidate())
            {
                Response.Redirect("/index");
                return;
            }
        }
    }
}
