#region �����c�vʷ�޸�����
/*
    ����:�����aƷ���
    ����:2022-05-05
*/
#endregion
using static OnlineShopBack.Pages.Product.BasePage;

namespace OnlineShopBack.Pages.Product
{
    public class AddProductModel : BasePageModel
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
