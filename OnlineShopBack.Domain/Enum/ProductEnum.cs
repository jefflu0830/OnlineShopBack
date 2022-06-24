#region 功能與歷史修改描述
/*
    描述:後台商品系統相關列舉
    日期:2022-05-05
*/
#endregion
namespace OnlineShopBack.Domain.Enum
{
    public class ProductEnum
    {
        //商品相關列舉(Enum)        
        public enum ProductReturnCode //新增商品
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
            //圖片上傳失敗,請檢查格式
            //</summary >
            ImgFail = 202,
            //<summary >
            //Defult
            //</summary >
            Defult = 999
        }

        public enum CategoryReturnCode//[類別]相關代號
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
