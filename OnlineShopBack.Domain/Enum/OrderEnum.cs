#region 功能與歷史修改描述
/*
    描述:後台訂單系統相關列舉
    日期:2022-05-05
*/
#endregion
namespace OnlineShopBack.Domain.Enum
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
            //後端驗證失敗
            //</summary >
            ValidaFail = 200,
            //<summary >
            //例外錯誤
            //</summary >
            ExceptionError = 201,
            //<summary >
            //Defult
            //</summary >
            Defult = 999
        }
    }
}
