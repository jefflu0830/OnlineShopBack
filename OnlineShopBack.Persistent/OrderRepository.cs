using Microsoft.Data.SqlClient;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;
using OnlineShopBack.Domain.Enum;

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
                cmd.Connection = new SqlConnection("");
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

    }
}
