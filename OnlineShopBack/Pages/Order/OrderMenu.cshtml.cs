#region �����c�vʷ�޸�����
/*
    ����:ӆ�����
    ����:2022-05-05
*/
#endregion
using static OnlineShopBack.Pages.Order.BasePage;

namespace OnlineShopBack.Pages.Order
{
    public class OrderMenuModel : BasePageModel
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
