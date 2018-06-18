using MC_Checker.Enums;
using System;
using System.Collections.Generic;

namespace MC_Checker
{
    class Data
    {
        public static List<string> AccountsList = new List<string>();
        public static List<string> ProxyList = new List<string>();

        public static int CurrentAccount = 0;

        public static AuthType AuthType = AuthType.SITE;
        public static ProxyType ProxyType = ProxyType.NO_PROXY;

        public static int Goods = 0;

        public static int Premium = 0;
        public static int SecretQuestion = 0;
        public static int Gifts = 0;

        public static int Errors = 0;

        public static bool SaveSQ = false;
        public static bool SaveWithoutPremium = false;

        public static string GetCurrentAccount()
        {
            return AccountsList[CurrentAccount];
        }

        public static string GetRandomProxy()
        {
            var rnd = new Random();
            var Index = rnd.Next(ProxyList.Count);

            return ProxyList[Index];
        }
    }
}
