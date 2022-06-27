using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using OnlineShopBack.Domain.DTOs.Product;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;
using OnlineShopBack.Domain.Enum;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace OnlineShopBack.Persistent
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _SQLConnectionString = null;//SQL連線字串

        private readonly IHostingEnvironment _env;
        public ProductRepository(IHostingEnvironment env, IConfigHelperRepository configHelperRepository)
        {
            _env = env;
            _SQLConnectionString = configHelperRepository.SQLConnectionStrings();
        }
        /*---------------商品相關---------------*/
        //取得商品List
        public DataTable GetProduct()
        {

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
                cmd.CommandText = @" EXEC pro_onlineShopBack_getProductAndCategory ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
            }
            finally
            {
                //關閉連線
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            return dt;
        }

        //新增商品 
        public int AddProduct(ProductDto dto, IFormFileCollection files)
        {
            int ResultCode = (int)ProductEnum.ProductReturnCode.Defult;
            string ErrorStr = "";//記錄錯誤訊息
            //資料驗證-----------------------------------------------           

            //商品代號
            if (string.IsNullOrWhiteSpace(dto.Num))
            {
                ErrorStr += "[商品代號不可為空]\n";
            }
            else
            {
                if (!MyTool.IsENAndNumber(dto.Num))
                {
                    ErrorStr += "[商品代號只能為英數]\n";
                }
                if (dto.Num.Length > 20 || dto.Num.Length < 3)
                {
                    ErrorStr += "[商品代號長度應介於3～20個數字之間]\n";
                }
            }
            //商品主類別編號
            int[] CategoryArr = { 10, 20, 30 };//10=3C ,20=電腦周邊 ,30=軟體

            if (Array.IndexOf(CategoryArr, dto.Category) < 0)
            {
                ErrorStr += "[主類別]不存在請重新輸入\n";
            }
            //商品子類型
            if (dto.SubCategory > 999 || dto.SubCategory < 0)
            {
                ErrorStr += "[商品類型&商品子類型範圍應在0-999之間]\n";
            }
            //商品名稱
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                ErrorStr += "[商品名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(dto.Name))
                {
                    ErrorStr += "[商品名稱應為中文,英文及數字]\n";
                }
                if (dto.Name.Length > 20 || dto.Name.Length < 1)
                {
                    ErrorStr += "[商品名稱應介於1～20個字之間]\n";
                }
            }
            //價格
            if (dto.Price < 0 || dto.Price > 999999999)
            {
                ErrorStr += "[價格不得為負或大於999999999]\n";
            }
            //是否開放商品
            if ((dto.Status != 0 && dto.Status != 100))//|| (value.Status != 0))
            {
                ErrorStr += "[狀態碼應為0(開放)或100(不開放)]\n";
            }
            //商品描述
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {//不為空才要做字數判斷
                if (dto.Content.Length > 500 || dto.Content.Length < 0)
                {
                    ErrorStr += "[商品描述應在500字內]\n";
                }
            }
            //庫存量
            if (dto.Stock < 1 || dto.Stock > 99999)
            {
                ErrorStr += "[庫存量應介於1-99999]\n";
            }
            //熱門度
            if (dto.Popularity < 0 || dto.Popularity > 100)
            {
                ErrorStr += "[熱門度應介於1-100]\n";
            }


            //ErrorStr不為空 則回傳錯誤訊息
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)ProductEnum.ProductReturnCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {

                    //取得存放路徑
                    string ImgPath = _env.ContentRootPath + @"\wwwroot\Img\";
                    //新圖片名稱
                    string NewProductName = "";
                    //儲存路徑
                    string SaveImgPath = "";

                    if (files.Count != 1) //圖片數量不為1
                    {
                        ResultCode = (int)ProductEnum.ProductReturnCode.ImgFail;
                    }
                    else//圖片為一張
                    {
                        var Imgfile = files[0];
                        string imgType = Path.GetExtension(Imgfile.FileName); //取得圖片格式
                                                                              //限制圖片格式
                        if (imgType.Contains(".jpg") || imgType.Contains(".png") || imgType.Contains(".bmp"))//bmp?
                        {
                            if (!Directory.Exists(ImgPath))//資料夾不存在新增資料夾                    
                            {
                                Directory.CreateDirectory(ImgPath);
                            }
                            else
                            {
                                NewProductName = dto.Num + imgType; //新圖片名稱
                                SaveImgPath = ImgPath + NewProductName;//儲存路徑 
                            }
                        }
                        else//不支援此格式
                        {
                            ResultCode = (int)ProductEnum.ProductReturnCode.ImgFail;
                        }
                    }

                    if (ResultCode == (int)ProductEnum.ProductReturnCode.Defult)
                    {
                        // 資料庫連線
                        cmd = new SqlCommand();
                        cmd.Connection = new SqlConnection(_SQLConnectionString);

                        cmd.CommandText = @"EXEC pro_onlineShopBack_addProduct @num, @category, @subCategory, @name, @ImgPath, @price, @status, @contnet, @stock, @popularity";

                        cmd.Parameters.AddWithValue("@num", dto.Num);                //商品代號
                        cmd.Parameters.AddWithValue("@category", dto.Category);      //商品類型
                        cmd.Parameters.AddWithValue("@subCategory", dto.SubCategory);//商品子類型
                        cmd.Parameters.AddWithValue("@name", dto.Name);              //商品名稱
                        cmd.Parameters.AddWithValue("@ImgPath", NewProductName);     //圖片
                        cmd.Parameters.AddWithValue("@price", dto.Price);            //價格
                        cmd.Parameters.AddWithValue("@status", dto.Status);          //開放狀態
                        cmd.Parameters.AddWithValue("@contnet", dto.Content);        //商品描述     
                        cmd.Parameters.AddWithValue("@stock", dto.Stock);            //庫存量
                        cmd.Parameters.AddWithValue("@popularity", dto.Popularity);  //熱門度

                        //開啟連線
                        cmd.Connection.Open();
                        ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                        //資料庫輸入成功後才會做儲存圖片動作
                        if (ResultCode == (int)ProductEnum.ProductReturnCode.Success && !string.IsNullOrWhiteSpace(NewProductName))
                        {
                            var file = files[0];
                            var stream = new FileStream(SaveImgPath, FileMode.Create);
                            file.CopyToAsync(stream);
                            stream.Close();//關閉
                        }
                    }
                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)ProductEnum.ProductReturnCode.ExceptionError;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
                }
            }

            return ResultCode;
        }

        //更新商品
        public int UpdateProduct(ProductDto dto, IFormFileCollection files)
        {
            int ResultCode = (int)ProductEnum.ProductReturnCode.Defult;
            string ErrorStr = "";//記錄錯誤訊息

            //商品代號
            if (string.IsNullOrWhiteSpace(dto.Num))
            {
                ErrorStr += "[商品代號不可為空]\n";
            }
            else
            {
                if (!MyTool.IsENAndNumber(dto.Num))
                {
                    ErrorStr += "[商品代號只能為英數]\n";
                }
                if (dto.Num.Length > 20 || dto.Num.Length < 3)
                {
                    ErrorStr += "[商品代號長度應介於3～20個數字之間]\n";
                }
            }
            //商品主類別編號
            int[] CategoryArr = { 10, 20, 30 };//10=3C ,20=電腦周邊 ,30=軟體

            if (Array.IndexOf(CategoryArr, dto.Category) < 0)
            {
                ErrorStr += "[主類別]不存在請重新輸入\n";
            }
            //商品子類型
            if (dto.SubCategory > 999 || dto.SubCategory < 0)
            {
                ErrorStr += "[商品類型&商品子類型範圍應在0-999之間]\n";
            }
            //商品名稱
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                ErrorStr += "[商品名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(dto.Name))
                {
                    ErrorStr += "[商品名稱應為中文,英文及數字]\n";
                }
                if (dto.Name.Length > 20 || dto.Name.Length < 1)
                {
                    ErrorStr += "[商品名稱應介於1～20個字之間]\n";
                }
            }
            //價格
            if (dto.Price < 0 || dto.Price > 999999999)
            {
                ErrorStr += "[價格不得為負或大於999999999]\n";
            }
            //是否開放商品
            if ((dto.Status != 0 && dto.Status != 100))//|| (value.Status != 0))
            {
                ErrorStr += "[狀態碼應為0(開放)或100(不開放)]\n";
            }
            //商品描述
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {//不為空才要做字數判斷
                if (dto.Content.Length > 500 || dto.Content.Length < 0)
                {
                    ErrorStr += "[商品描述應在500字內]\n";
                }
            }
            //庫存量
            if (dto.Stock < 1 || dto.Stock > 99999)
            {
                ErrorStr += "[庫存量應介於1-99999]\n";
            }
            //熱門度
            if (dto.Popularity < 0 || dto.Popularity > 100)
            {
                ErrorStr += "[熱門度應介於1-100]\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)ProductEnum.ProductReturnCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {

                    //取得存放路徑
                    string ImgPath = _env.ContentRootPath + @"\wwwroot\Img\";
                    //新圖片名稱
                    string NewProductName = "";
                    //儲存路徑
                    string SaveImgPath = "";

                    if (files.Count == 1) //有要更新的圖片
                    {
                        var Imgfile = files[0];
                        string imgType = Path.GetExtension(Imgfile.FileName); //取得圖片格式
                                                                              //限制圖片格式
                        if (imgType.Contains(".jpg") || imgType.Contains(".png") || imgType.Contains(".bmp"))//bmp?
                        {
                            if (!Directory.Exists(ImgPath))//資料夾不存在新增資料夾                    
                            {
                                Directory.CreateDirectory(ImgPath);
                            }
                            else
                            {
                                NewProductName = dto.Num + imgType; //新圖片名稱
                                SaveImgPath = ImgPath + NewProductName;//儲存路徑 
                            }
                        }
                        else//不支援此格式
                        {
                            ResultCode = (int)ProductEnum.ProductReturnCode.ImgFail;
                        }
                    }

                    if (ResultCode == (int)ProductEnum.ProductReturnCode.Defult)
                    {
                        // 資料庫連線
                        cmd = new SqlCommand();
                        cmd.Connection = new SqlConnection(_SQLConnectionString);

                        cmd.CommandText = @"EXEC pro_onlineShopBack_putProduct @productNum, @category, @subCategory, @name, @price, @status, @contnet, @stock, @popularity";


                        cmd.Parameters.AddWithValue("@productNum", dto.Num);          //商品代號
                        cmd.Parameters.AddWithValue("@category", dto.Category);      //商品類型
                        cmd.Parameters.AddWithValue("@subCategory", dto.SubCategory);//商品子類型
                        cmd.Parameters.AddWithValue("@name", dto.Name);              //商品名稱
                        cmd.Parameters.AddWithValue("@ImgPath", NewProductName);   //圖片
                        cmd.Parameters.AddWithValue("@price", dto.Price);            //價格
                        cmd.Parameters.AddWithValue("@status", dto.Status);          //開放狀態
                        cmd.Parameters.AddWithValue("@contnet", dto.Content);        //商品描述     
                        cmd.Parameters.AddWithValue("@stock", dto.Stock);            //庫存量
                        cmd.Parameters.AddWithValue("@popularity", dto.Popularity);  //熱門度

                        //開啟連線
                        cmd.Connection.Open();
                        ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL                

                        //資料庫輸入成功後才會做儲存圖片動作&有新圖片
                        if (ResultCode == (int)ProductEnum.ProductReturnCode.Success && !string.IsNullOrWhiteSpace(NewProductName) && files.Count == 1)
                        {
                            var file = files[0];
                            var stream = new FileStream(SaveImgPath, FileMode.Create);
                            file.CopyToAsync(stream);
                            stream.Close(); //關閉

                        }
                    }
                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)ProductEnum.ProductReturnCode.ExceptionError;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
                }
            }
            return ResultCode;
        }

        //刪除商品
        public int DelProduct(int ProductId, string ProductNum, string ImgName)
        {
            string ErrorStr = "";//記錄錯誤訊息
            int ResultCode = (int)ProductEnum.ProductReturnCode.Defult;
            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                ResultCode = (int)ProductEnum.ProductReturnCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);
                    cmd.CommandText = @"EXEC pro_onlineShopBack_delProduct @productId, @productNum";

                    cmd.Parameters.AddWithValue("@productId", ProductId);
                    cmd.Parameters.AddWithValue("@productNum", ProductNum);
                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL


                    //資料庫刪除成功後才會做刪除圖片動作
                    if (ResultCode == (int)ProductEnum.ProductReturnCode.Success)
                    {
                        //取得存放路徑
                        string ImgPath = _env.ContentRootPath + @"\wwwroot\Img\";
                        string DelImgPath = ImgPath + ImgName;//儲存路徑 
                        System.IO.FileInfo file = new System.IO.FileInfo(DelImgPath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                }
                catch (Exception e)
                {
                    ResultCode = (int)ProductEnum.ProductReturnCode.ExceptionError;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
                }
            }
            return ResultCode;
        }

        //取得類別
        public DataTable GetCategory()
        {

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
                cmd.CommandText = @" EXEC pro_onlineShopBack_getProductCategory ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
            }
            finally
            {
                //關閉連線
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }

            return dt;
        }

        //新增類別 
        public int AddCategory(ProductCategoryDto value)
        {
            string ErrorStr = "";//記錄錯誤訊息
            int ResultCode = (int)ProductEnum.CategoryReturnCode.Defult;

            int[] CategoryArr = { 10, 20, 30 };//10=3C ,20=電腦周邊 ,30=軟體
            //主類別
            if (Array.IndexOf(CategoryArr, value.CategoryNum) < 0)
            {
                ErrorStr += "[主類別]不存在請重新輸入\n";
            }

            //子類別
            if (value.SubCategoryNum > 999 || value.SubCategoryNum < 0)
            {
                ErrorStr += "[子類別編號應介於0～999之間]\n";
            }
            //子類別名稱
            if (string.IsNullOrEmpty(value.SubCategoryName))
            {
                ErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.SubCategoryName))
                {
                    ErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (value.SubCategoryName.Length > 20 || value.SubCategoryName.Length < 0)
                {
                    ErrorStr += "[名稱應介於0～20個字之間]\n";
                }
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)ProductEnum.CategoryReturnCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_addProductCategory @categoryNum, @subCategoryNum, @subCategoryName";

                    cmd.Parameters.AddWithValue("@categoryNum", value.CategoryNum);
                    cmd.Parameters.AddWithValue("@subCategoryNum", value.SubCategoryNum);
                    cmd.Parameters.AddWithValue("@subCategoryName", value.SubCategoryName);
                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)ProductEnum.CategoryReturnCode.ExceptionError;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
                }
            }
            return ResultCode;
        }

        //更新類別
        public int UpdateCategory(int Num, int SubNum, ProductCategoryDto value)
        {
            int ResultCode = (int)ProductEnum.CategoryReturnCode.Defult;

            string ErrorStr = "";//記錄錯誤訊息
            //主類別編號
            int[] CategoryArr = { 10, 20, 30 };//10=3C ,20=電腦周邊 ,30=軟體
            if (Array.IndexOf(CategoryArr, Num) < 0)
            {
                ErrorStr += "[主類別]不存在請重新輸入\n";
            }

            //子類別編號 
            if (SubNum > 999 || SubNum < 0)
            {
                ErrorStr += "[子類別編號應介於0～999個之間]\n";
            }

            //權限名稱
            if (string.IsNullOrEmpty(value.SubCategoryName))
            {
                ErrorStr += "[子類別名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.SubCategoryName))
                {
                    ErrorStr += "[子類別名稱應為中文,英文及數字]\n";
                }
                if (value.SubCategoryName.Length > 20 || value.SubCategoryName.Length < 0)
                {
                    ErrorStr += "[子類別名稱應介於0～20個字之間]\n";
                }
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)ProductEnum.CategoryReturnCode.ValidaFail;
            }
            else
            {

                SqlCommand cmd = null;

                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_putCategory @num, @subNum, @subCategoryName";

                    cmd.Parameters.AddWithValue("@num", Num);
                    cmd.Parameters.AddWithValue("@subNum", SubNum);
                    cmd.Parameters.AddWithValue("@subCategoryName", value.SubCategoryName);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL                
                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)ProductEnum.CategoryReturnCode.ExceptionError;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
                }
            }
            return ResultCode;
        }
        //刪除類別
        public int DelCategory(int Num, int SubNum)
        {
            int ResultCode = (int)ProductEnum.CategoryReturnCode.Defult;
            string ErrorStr = "";//記錄錯誤訊息


            //主類別編號
            int[] CategoryArr = { 10, 20, 30 };//10=3C ,20=電腦周邊 ,30=軟體
            if (Array.IndexOf(CategoryArr, Num) < 0)
            {
                ErrorStr += "[主類別]不存在請重新輸入\n";
            }

            //子類別編號 
            if (SubNum > 999 || SubNum < 0)
            {
                ErrorStr += "[子類別編號 應介於0～999之間]\n";
            }
            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)ProductEnum.CategoryReturnCode.ValidaFail;
            }
            else
            {

                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);
                    cmd.CommandText = @"EXEC pro_onlineShopBack_delCategory @categoryNum, @subCategoryNum";

                    cmd.Parameters.AddWithValue("@categoryNum", Num);
                    cmd.Parameters.AddWithValue("@subCategoryNum", SubNum);
                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL
                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)ProductEnum.CategoryReturnCode.ExceptionError;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
                }
            }
            return ResultCode;
        }
    }
}
