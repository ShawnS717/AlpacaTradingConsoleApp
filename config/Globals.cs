using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AlpacaTradingApp.config
{
    static class Globals
    {
        //limit of 200 api calls a minute
        //120 per minute for updating prices (0.5 sec)
        //80 calls remaining
        //rest of the calls are to be handled by thread priority
        public static readonly string APIKey = "PKL8Q4FIICRVFDTV6LGB";
        public static readonly string APISecretKey = "GIGnlGQIx8eRfV2zkC5TKj69wZe6M/ybWGXv4el0";
        public static byte ApiCalls = 0;
        public static readonly string SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AlpacaTradingConsoleApp");

        public static bool MarketAvaliability = false;
        public static bool LastMarketAvaliability = false;

        public static List<string> WatchedSymbols = new List<string>()
        {
            "M",
            "CIF",
            "AACG",
            "AAME",
            "EEA"
        };
        public static decimal InvestingMaxAmount = 10;
        public static decimal IndividualInvestMaxAmount = 1;
    }
}
