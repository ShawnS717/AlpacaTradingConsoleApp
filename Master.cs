using Alpaca.Markets;
using AlpacaTradingApp.workers;
using AlpacaTradingApp.config;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace AlpacaTradingApp
{
    class Master
    {
        static void Main(string[] args)
        {
            //make the needed shared variables
            AlpacaTradingClient tradeClient = APIPortal.MakeTradingClient();
            AlpacaDataClient dataClient = APIPortal.MakeDataClient();
            if (!Directory.Exists(Config.SaveFolder))
            {
                Directory.CreateDirectory(Config.SaveFolder);
            }

            //check if the market is open
            while (true)
            {
                lock (tradeClient)
                {
                    Config.LastMarketAvaliability = Config.MarketAvaliability;
                    Config.MarketAvaliability = APIPortal.IsMarketOpen(tradeClient).Result;
                }

                //if the market has opened then start the workers
                if (Config.MarketAvaliability && !Config.LastMarketAvaliability)
                {
                    Console.WriteLine("Market is open");
                    //load the previous days data here

                    //make a new list of histories for todays workers
                    List<SymbolHistory> histories = CreateHistories();

                    //make the workers
                    Thread priceUpdater = new Thread(new MarketPriceUpdater(dataClient, histories).UpdatePrices);
                    Thread shortTermBroker = new Thread(new ShortTermBroker(tradeClient, histories).EventLoop);

                    //start the workers
                    priceUpdater.Start();
                    shortTermBroker.Start();

                    //(added for testing purposes)
                    Console.WriteLine("press enter to stop the program");
                    Console.ReadLine();
                    Config.MarketAvaliability = false;
                    break;
                    //(added for testing purposes)
                }
                else
                {
                    Console.WriteLine("Market is closed");
                }
                //wait 10min and see if it's still open
                //temp:moved to 1min for testing purposes
                Thread.Sleep(60000);//0
            }
        }

        public static List<SymbolHistory> CreateHistories()
        {
            List<SymbolHistory> symbolHistories = new List<SymbolHistory>();
            foreach (string symbol in Config.WatchedSymbols)
            {
                symbolHistories.Add(new SymbolHistory(symbol));
            }
            return symbolHistories;
        }
    }
}
