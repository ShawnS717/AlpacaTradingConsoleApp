using Alpaca.Markets;
using AlpacaTradingApp.config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AlpacaTradingApp.workers
{
    public class MarketOpenChecker
    {
        public AlpacaTradingClient client;

        public MarketOpenChecker() { }
        public MarketOpenChecker(AlpacaTradingClient tradingClient)
        {
            client = tradingClient;
        }

        public void CheckMarket()
        {
            while (true)
            {
                lock (client)
                {
                    Config.MarketAvaliability = APIPortal.IsMarketOpen().Result; 
                }
                if (Config.MarketAvaliability)
                {
                    Console.WriteLine("Market is open");
                }
                else
                {
                    Console.WriteLine("Market is closed");
                }
                Thread.Sleep(600000);
            }
        }
    }
}
