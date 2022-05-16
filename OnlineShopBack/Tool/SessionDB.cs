﻿
#region 功能與歷史修改描述
/*
    描述:判斷帳號重複登入用的Dictionary
    日期:2022-05-09
*/
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OnlineShopBack.Tool
{
    public class SessionDB
    {

        //public static Dictionary<string, string> sessionDB = new Dictionary<string, string>();        

        //public static Dictionary<string, SessionInfo> sessionDB01 = new Dictionary<string, SessionInfo>();  //Modi@ String -> SessionInfo


        public static ConcurrentDictionary<string, SessionInfo> sessionDB = new ConcurrentDictionary<string, SessionInfo>();//Modi@ Dictionary -> ConcurrentDictionary

        public class SessionInfo
        {
            public string SId { get; set; }  = string.Empty;

            public DateTime ValidTime{ get; set; } = DateTime.Now.AddMinutes(30);
        }



        
    }
}
