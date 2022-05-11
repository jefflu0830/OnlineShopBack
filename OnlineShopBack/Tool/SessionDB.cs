using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OnlineShopBack.Tool
{
    public class SessionDB
    {

        //public static Dictionary<string, string> sessionDB = new Dictionary<string, string>();        

        //public static Dictionary<string, SessionInfo> sessionDB = new Dictionary<string, SessionInfo>();  //Modi@ String -> SessionInfo

        public static ConcurrentDictionary<string, SessionInfo> sessionDB = new ConcurrentDictionary<string, SessionInfo>();//Modi@ Dictionary -> ConcurrentDictionary

        public class SessionInfo
        {
            public string SId { get; set; }  = string.Empty;

            public DateTime ValidTime{ get; set; } = DateTime.Now.AddMinutes(30);
        }
    }
}
