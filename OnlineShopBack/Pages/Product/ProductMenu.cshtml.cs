#region �����c�vʷ�޸�����
/*
    ����:��̨��Ʒ������
    ����:2022-05-05
*/
#endregion
using static OnlineShopBack.Pages.Product.BasePage;

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
