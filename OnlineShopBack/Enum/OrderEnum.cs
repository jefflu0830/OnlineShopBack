#region 功能與歷史修改描述
/*
    描述:後台訂單系統相關列舉
    日期:2022-05-05
*/
#endregion
namespace OnlineShopBack.Enum
{
    public class OrderEnum
    {
        //訂單相關列舉(Enum)        
        public enum OrderReturnCode //新增商品
        {
            //<summary >
            //成功
            //</summary >
            Success = 0,
            //<summary >
            //失敗
            //</summary >
            Fail = 1,
            //<summary >
            //預設值
            //</summary >
            Default = 2
        }
    }
}
