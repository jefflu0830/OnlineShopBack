using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace OnlineShopBack.Tool
{
    public class MyTool
    {
        
        //MD5 加密
        public static string PswToMD5(string pwd)
        {
            var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.ASCII.GetBytes(pwd));
            var strResult = BitConverter.ToString(result);
            var md5Pwd = strResult.Replace("-", "");
            return md5Pwd;
        }

        //DataTable轉JSON
        public static string DataTableJson(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sb.Append("\"");
                    sb.Append(dt.Columns[j].ColumnName);
                    sb.Append("\":\"");
                    sb.Append(dt.Rows[i][j].ToString());
                    sb.Append("\",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }

        //判斷字串是否只有英數
        public static bool IsENAndNumber(string str )
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");

            return reg1.IsMatch(str);
        }

        //判斷字串是否只有中,英及數字
        public static bool IsCNAndENAndNumber(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9_\u4e00-\u9fa5]+$");

            return reg1.IsMatch(str);
        }



    }
}
