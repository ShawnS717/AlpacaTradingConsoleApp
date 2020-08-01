using Alpaca.Markets;
using AlpacaTradingApp.workers;
using AlpacaTradingApp.config;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AlpacaTradingApp
{
    class Master
    {
        static void Main(string[] args)
        {
            var tradeClient = APIPortal.MakeTradingClient();
            var dataClient = APIPortal.MakeDataClient();
            var histories = new List<SymbolHistory>() { new SymbolHistory() { Symbol = "M" } };

            Thread marketAvaliabilityChecker = new Thread(new MarketOpenChecker(tradeClient).CheckMarket);
            marketAvaliabilityChecker.Start();

            Thread.Sleep(2000);
            if (Config.MarketAvaliability)
            {
                Thread priceUpdaterThread = new Thread(new MarketPriceUpdater(dataClient, histories).UpdatePrices);
                priceUpdaterThread.Start();
            }

        }
    }
}
