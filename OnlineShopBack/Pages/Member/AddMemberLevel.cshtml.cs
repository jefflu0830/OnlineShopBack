#region �����c�vʷ�޸�����
/*
    ����:ǰ̨���T�ȼ��������
    ����:2022-05-05
*/
#endregion
using static OnlineShopBack.Pages.Member.BasePage;

namespace OnlineShopBack.Pages.Account
{
    public class AddMemberLevelModel : BasePageModel
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
