using OnlineShopBack.Domain.DTOs.Member;
using System.Data;

namespace OnlineShopBack.Domain.Repository
{
    public interface IMemberRepository
    {
        /*----------前台會員相關----------*/
        public DataTable GetAccountAndLevelList();//取得前台會員資料

        public DataTable GetMemberByAcc( string Acc ); //取得指定會員資料

        public int DelMember(int id);//刪除會員

        public int EditMember(int id, MemberDto value);//編輯會員(等級&狀態)

        public int EditShopGold(PutShopGlodDto value);//調整購物金

        /*----------前台會員等級相關----------*/
        public DataTable GetMemLvList(); //取得會員等級資料List

        public int AddMemLv(MemLvDto value);//添加會員等級 

        public int EditMemLv(int MemLv, MemLvDto value);//更新權限

        public int DelMemLv(int memLv);//刪除會員等級

        /*----------前台會員狀態相關----------*/
        public DataTable GetSuspensionList();//取得狀態資料List

        public int AddSuspension(suspensionDto value);//增加狀態 

        public int EditSuspension(int id, suspensionDto value); //更新狀態

        public int DelSuspension( int id);//刪除等級
    }
}
