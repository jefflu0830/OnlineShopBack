using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopBack.Enum
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
            //失敗
            //</summary >
            Fail = 1,
            //<summary >
            //預設值
            //</summary >
            Default = 2
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
