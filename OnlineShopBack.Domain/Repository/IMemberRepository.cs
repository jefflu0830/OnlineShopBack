using OnlineShopBack.Domain.DTOs.Member;
using System.Data;

namespace OnlineShopBack.Domain.Repository
{
    public interface IMemberRepository
    {
        public DataTable GetAccountAndLevelList();//取得前台會員資料

        public DataTable GetMemberByAcc( string Acc ); //取得指定會員資料

        public int DelMember(int id);//刪除會員

        public int EditMember(int id, MemberDto value);//編輯會員(等級&狀態)

        public int EditShopGold(PutShopGlodDto value);//調整購物金
    }
}
