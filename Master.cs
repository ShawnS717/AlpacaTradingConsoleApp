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
            if (!Directory.Exists(Globals.SaveFolder))
            {
                Directory.CreateDirectory(Globals.SaveFolder);
            }

            //due to the importance of this thread, make sure it gets priority
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            //check if the market is open
            while (true)
            {
                lock (tradeClient)
                {
                    Globals.LastMarketAvaliability = Globals.MarketAvaliability;
                    Globals.MarketAvaliability = APIPortal.IsMarketOpen(tradeClient).Result;
                }

                //if the market has opened then start the workers
                if (Globals.MarketAvaliability && !Globals.LastMarketAvaliability)
                {
                    Console.WriteLine("Market has opened");
                    //TODO:
                    //load the previous days data here

                    //make a new list of histories for todays workers
                    List<SymbolHistory> histories = CreateHistories();

                    //make and link the workers
                    MarketPriceUpdater priceUpdateWorker = new MarketPriceUpdater(dataClient, histories);
                    Auditor auditor = new Auditor(tradeClient);
                    ShortTermBroker dayTrader = new ShortTermBroker(tradeClient, histories, auditor);


                    Thread callTimer = new Thread(new ApiCallTimer().StartEventLoop);
                    Thread priceUpdater = new Thread(priceUpdateWorker.UpdatePrices);
                    Thread runAuditor = new Thread(auditor.StartEventLoop);
                    Thread shortTermBroker = new Thread(dayTrader.StartEventLoop);

                    //start the workers
                    callTimer.Start();
                    priceUpdater.Start();
                    runAuditor.Start();
                    shortTermBroker.Start();

                    //(added for testing purposes)
                    Console.WriteLine("press enter to stop the program");
                    Console.ReadLine();
                    Globals.MarketAvaliability = false;
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
            foreach (string symbol in Globals.WatchedSymbols)
            {
                symbolHistories.Add(new SymbolHistory(symbol));
            }
            return symbolHistories;
        }
    }
}
