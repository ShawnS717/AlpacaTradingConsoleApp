using System;
using System.Collections.Generic;
using System.IO;

namespace AlpacaTradingApp.config
{
    static class Globals
    {
        //limit of 200 api calls a minute
        //120 per minute for updating prices (0.5 sec)
        //80 calls remaining
        //rest of the calls are to be handled by thread priority

        //things to consider:
        //consider making some events upon making some buy/sell orders
        //consider seeing when a buy/sell order gets filled
        public static readonly string APIKey = "PKL8Q4FIICRVFDTV6LGB";
        public static readonly string APISecretKey = "GIGnlGQIx8eRfV2zkC5TKj69wZe6M/ybWGXv4el0";
        //moved into the ApiInterface
        //public static byte ApiCalls = 0;
        public static readonly string SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AlpacaTradingConsoleApp");

        public static bool MarketAvaliability = false;
        public static bool LastMarketAvaliability = false;

        public static List<string> WatchedSymbols = new List<string>()
        {
            "TRT",
            "GTT",
            "QTT",
            "WTT",
            "MYT",
            "AAU",
            "GAU",
            "JFU",
            "BLU",
            "CMU",
            "GRU",
            "CSU",
            "BTU",
            "UUU",
            "CVU",
            "AXU",
            "LXU",
            "NCV",
            "IMV",
            "GSV",
            "PSV",
            "CVV",
            "BBW",
            "GNW",
            "DPW",
            "FAX",
            "JAX",
            "UBX",
            "OCX",
            "SCX",
            "EEX",
            "NEX",
            "DHX",
            "PHX",
            "HLX",
            "PLX",
            "EMX",
            "MMX",
            "TRX",
            "ASX",
            "DSX",
            "MUX",
            "AWX",
            "NBY",
            "ACY",
            "AEY",
            "EGY",
            "DHY",
            "ANY",
            "BRY",
            "SSY",
            "WYY",
            "NCZ",
            "VGZ",
            "ENZ",
            "HTZ",
            "NTZ",
            "DZZ",
            "CUBA",
            "MDCA",
            "SLCA",
            "GNCA",
            "SNCA",
            "BCDA",
            "RKDA",
            "GMDA",
            "ESEA",
            "DTEA",
            "IZEA",
            "LMFA",
            "SRGA",
            "APHA",
            "CALA",
            "XELA",
            "POLA",
            "ADMA",
            "SGMA",
            "ALNA",
            "HEPA",
            "XSPA",
            "MARA",
            "IDRA",
            "NDRA",
            "CHRA",
            "CTRA",
            "YTRA",
            "SVRA",
            "MESA",
            "WISA",
            "TLSA",
            "HUSA",
            "CPTA",
            "SAVA",
            "BBVA",
            "NEWA",
            "ALYA",
            "PLYA",
            "UFAB",
            "SPCB",
            "ITCB",
            "SLDB",
            "SIEB",
            "TRIB",
            "CTIB",
            "SELB",
            "XELB",
            "SGLB",
            "XTLB",
            "ICMB",
            "MTNB",
            "ENOB",
            "PBPB",
            "VERB",
            "CLRB",
            "PDSB",
            "RVSB",
            "LITB",
            "CLUB",
            "ITUB",
            "NAVB",
            "SEAC"
        };
        public static decimal InvestingMaxAmount = 10;
        public static decimal CurrentlyInvested = 0;
        public static decimal IndividualInvestMaxAmount = 1;
    }
}
