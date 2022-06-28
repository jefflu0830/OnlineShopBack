using Microsoft.Data.SqlClient;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;
using OnlineShopBack.Domain.Enum;
using OnlineShopBack.Domain.DTOs.Order;

namespace OnlineShopBack.Persistent
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _SQLConnectionString = null;//SQL連線字串

        public OrderRepository(IConfigHelperRepository configHelperRepository)
        {
            _SQLConnectionString = configHelperRepository.SQLConnectionStrings();
        }

        /*-----------------訂單相關-----------------*/

        //取得訂單
        public DataSet GetOrder()
        {
            SqlCommand cmd = null;
            DataSet st = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
                cmd.CommandText = @" EXEC pro_onlineShopBack_getOrder ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(st);
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

            return st;
        }

        //更新訂單配送方式&狀態
        public int UpdateOrder(string OrderNum, int TransportNum, int TransportStatusNum)
        {
            int ResultCode = (int)OrderEnum.OrderReturnCode.Defult;

            string ErrorStr = "";//記錄錯誤訊息
            //訂單編號
            if (!MyTool.OnlyNumber(OrderNum))
            {
                ErrorStr += "[訂單號碼只能是數字]\n";
            }

            //配送方式&配送狀態
            if (TransportNum > 255 || TransportNum < 0 ||
                TransportStatusNum > 255 || TransportStatusNum < 0)
            {
                ErrorStr += "[方式代號&狀態代號，應介於0～255個之間]\n";
            }

            //錯誤訊息有值 return錯誤訊息
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)OrderEnum.OrderReturnCode.ValidaFail;
            }

            SqlCommand cmd = null;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_putOrder @orderNum, @transportNum, @transportStatusNum ";

                cmd.Parameters.AddWithValue("@orderNum", OrderNum);
                cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                cmd.Parameters.AddWithValue("@transportStatusNum", TransportStatusNum);

                //開啟連線
                cmd.Connection.Open();
                ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL                

            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
                ResultCode = (int)OrderEnum.OrderReturnCode.ExceptionError;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }

            return ResultCode;
        }

        //訂單退貨
        public int OrderReturn(string OrderNum)
        {
            int ResultCode = (int)OrderEnum.OrderReturnCode.Defult;
            string ErrorStr = "";//記錄錯誤訊息
            //訂單編號
            if (!MyTool.OnlyNumber(OrderNum))
            {
                ErrorStr += "[訂單號碼只能是數字]\n";
            }

            //錯誤訊息有值 return錯誤訊息
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)OrderEnum.OrderReturnCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @" EXEC pro_onlineShopBack_putOrderReturn @orderNum ";

                    cmd.Parameters.AddWithValue("@orderNum", OrderNum);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL                
                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)OrderEnum.OrderReturnCode.ExceptionError;
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

        //取消訂單
        public int OrderCancel(string OrderNum)
        {
            int ResultCode = (int)OrderEnum.OrderReturnCode.Defult;

            string ErrorStr = "";//記錄錯誤訊息
            //訂單編號
            if (!MyTool.OnlyNumber(OrderNum))
            {
                ErrorStr += "[訂單號碼只能是數字]\n";
            }

            //錯誤訊息有值 return錯誤訊息
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)OrderEnum.OrderReturnCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @" EXEC pro_onlineShopBack_putOrderCancel @orderNum ";

                    cmd.Parameters.AddWithValue("@orderNum", OrderNum);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL                

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)OrderEnum.OrderReturnCode.ExceptionError;
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

        /*配送方式相關-----------------*/

        //取得配送方式
        public DataTable GetTransport()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
                cmd.CommandText = @" SELECT f_transport, f_transportName FROM t_transport ";

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

        //新增配送方式
        public int AddTransport(TransportDto value)
        {
            int ResultCode = (int)OrderEnum.OrderReturnCode.Defult;
            string ErrorStr = "";//記錄錯誤訊息

            //編號 
            if (value.Transport > 255 || value.Transport < 0)
            {
                ErrorStr += "[代號應介於0～255個之間]\n";
            }

            //名稱
            if (string.IsNullOrEmpty(value.TransportName))
            {
                ErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.TransportName))
                {
                    ErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (value.TransportName.Length > 20 || value.TransportName.Length < 1)
                {
                    ErrorStr += "[名稱應介於1～20個字之間]\n";
                }
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)OrderEnum.OrderReturnCode.ValidaFail;
            }
            else
            {

                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_addTransport @transport, @transportName ";

                    cmd.Parameters.AddWithValue("@transport", value.Transport);
                    cmd.Parameters.AddWithValue("@transportName", value.TransportName);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)OrderEnum.OrderReturnCode.ExceptionError;
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

        //刪除配送方式
        public int DelTransport(int TransportNum)
        {
            int ResultCode = (int)OrderEnum.OrderReturnCode.Defult;

            string ErrorStr = "";//記錄錯誤訊息
            //編號 
            if (TransportNum > 255 || TransportNum < 0)
            {
                ErrorStr += "[代號應介於0～255個之間]\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)OrderEnum.OrderReturnCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);
                    cmd.CommandText = @"EXEC pro_onlineShopBack_delTransport @transportNum ";

                    cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL
                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)OrderEnum.OrderReturnCode.ExceptionError;
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

        //更新配送方式名稱
        public int UpdateTransport(int TransportNum, string TransportName)
        {
            int ResultCode = (int)OrderEnum.OrderReturnCode.Defult;

            string ErrorStr = "";//記錄錯誤訊息
            //編號 
            if (TransportNum > 255 || TransportNum < 0)
            {
                ErrorStr += "[代號應介於0～255個之間]\n";
            }

            //名稱
            if (string.IsNullOrEmpty(TransportName))
            {
                ErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(TransportName))
                {
                    ErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (TransportName.Length > 20 || TransportName.Length < 0)
                {
                    ErrorStr += "[名稱應介於0～20個字之間]\n";
                }
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)OrderEnum.OrderReturnCode.ExceptionError;
            }
            else
            {

                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_putTransport @TransportNum, @TransportName ";

                    cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                    cmd.Parameters.AddWithValue("@transportName", TransportName);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL                


                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)OrderEnum.OrderReturnCode.ExceptionError;
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

        /*-----------------配送狀態相關-----------------*/

        //取得配送狀態
        public DataSet GetTransportStatus()
        {
            SqlCommand cmd = null;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(_SQLConnectionString);
                cmd.CommandText = @" SELECT f_transport, f_transportName FROM t_transport  
                                     SELECT f_transport, f_transportStatus,f_transportStatusName FROM t_transportStatus ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
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
            return ds;
        }

        //新增配送狀態
        public int AddTransportStatus(TransportDto value)
        {
            int ResultCode = (int)OrderEnum.OrderReturnCode.Defult;
            string ErrorStr = "";//記錄錯誤訊息

            //編號 
            if (value.Transport > 255 || value.Transport < 0 ||
                value.TransportStatus > 255 || value.TransportStatus < 0)
            {
                ErrorStr += "[代號應介於0～255個之間]\n";
            }

            //名稱
            if (string.IsNullOrEmpty(value.TransportStatusName))
            {
                ErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.TransportStatusName))
                {
                    ErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (value.TransportStatusName.Length > 20 || value.TransportStatusName.Length < 1)
                {
                    ErrorStr += "[名稱應介於1～20個字之間]\n";
                }
            }


            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)OrderEnum.OrderReturnCode.ValidaFail;
            }
            else
            {
                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_addTransportStatus @transport, @transportStatus, @transportName ";

                    cmd.Parameters.AddWithValue("@transport", value.Transport);
                    cmd.Parameters.AddWithValue("@transportStatus", value.TransportStatus);
                    cmd.Parameters.AddWithValue("@transportName", value.TransportStatusName);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)OrderEnum.OrderReturnCode.ExceptionError;
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

        //更新配送方式名稱
        public int UpdateTransportStatus(int TransportNum, int TransportStatusNum, string TransportStatusName)
        {
            int ResultCode = (int)OrderEnum.OrderReturnCode.Defult;

            string ErrorStr = "";//記錄錯誤訊息
            //編號 
            if (TransportNum > 255 || TransportNum < 0 ||
                TransportStatusNum > 255 || TransportStatusNum < 0)
            {
                ErrorStr += "[代號應介於0～255個之間]\n";
            }

            //名稱
            if (string.IsNullOrEmpty(TransportStatusName))
            {
                ErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(TransportStatusName))
                {
                    ErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (TransportStatusName.Length > 20 || TransportStatusName.Length < 0)
                {
                    ErrorStr += "[名稱應介於0～20個字之間]\n";
                }
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)OrderEnum.OrderReturnCode.ValidaFail;
            }
            else
            {

                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShopBack_putTransportStatus @TransportNum, @transportStatusNum, @transportStatusName ";

                    cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                    cmd.Parameters.AddWithValue("@transportStatusNum", TransportStatusNum);
                    cmd.Parameters.AddWithValue("@transportStatusName", TransportStatusName);

                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL                

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)OrderEnum.OrderReturnCode.ExceptionError;
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
        //刪除配送狀態
        public int DelTransportStatus(int TransportNum, int TransportStatusNum)
        {
            int ResultCode = (int)OrderEnum.OrderReturnCode.Defult;

            string ErrorStr = "";//記錄錯誤訊息
            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                MyTool.WriteErroLog(ErrorStr);
                ResultCode = (int)OrderEnum.OrderReturnCode.ValidaFail;
            }
            else
            {

                SqlCommand cmd = null;
                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(_SQLConnectionString);
                    cmd.CommandText = @"EXEC pro_onlineShopBack_delTransportStatus @transportNum, @transportStatusNum ";

                    cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                    cmd.Parameters.AddWithValue("@transportStatusNum", TransportStatusNum);
                    //開啟連線
                    cmd.Connection.Open();
                    ResultCode = (int)cmd.ExecuteScalar();//執行Transact-SQL

                }
                catch (Exception e)
                {
                    MyTool.WriteErroLog(e.Message);
                    ResultCode = (int)OrderEnum.OrderReturnCode.ExceptionError;
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
