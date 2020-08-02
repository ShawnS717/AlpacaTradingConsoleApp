using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AlpacaTradingApp.config
{
    static class Config
    {
        public static readonly string APIKey = "PKL8Q4FIICRVFDTV6LGB";
        public static readonly string APISecretKey = "GIGnlGQIx8eRfV2zkC5TKj69wZe6M/ybWGXv4el0";
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
    }
}
