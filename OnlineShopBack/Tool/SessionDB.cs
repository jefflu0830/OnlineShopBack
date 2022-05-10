using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopBack.Tool
{
    public class SessionDB
    {
        public static Dictionary<string, SessionInfo> sessionDB = new Dictionary<string, SessionInfo>();//CUNCURRENT

       
        // public static string SessionId;

        public class SessionInfo
        {
            public string SId { get; set; } = string.Empty;

            public DateTime ValidTime { get; set; } = DateTime.Now.AddMinutes(30);
        }
    }
}
