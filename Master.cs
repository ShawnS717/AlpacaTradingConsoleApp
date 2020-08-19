using Alpaca.Markets;
using AlpacaTradingApp.workers;
using AlpacaTradingApp.config;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace AlpacaTradingApp
{
    class Master
    {
        static void Main(string[] args)
        {
            //make the needed shared variables
            APIInterface aPIInterface = new APIInterface();
            if (!Directory.Exists(Globals.SaveFolder))
            {
                Directory.CreateDirectory(Globals.SaveFolder);
            }

            //due to the importance of this thread, make sure it gets priority
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            //check if the market is open
            while (true)
            {
                lock (aPIInterface)
                {
                    Globals.LastMarketAvaliability = Globals.MarketAvaliability;
                    Globals.MarketAvaliability = aPIInterface.IsMarketOpen().Result;
                }

                //if the market has opened then start the workers
                if (Globals.MarketAvaliability && !Globals.LastMarketAvaliability)
                {
                    Console.WriteLine("Market has opened");

                    //make a new list of histories for todays workers
                    List<SymbolHistory> histories = CreateHistories();

                    //make and link the workers
                    MarketPriceUpdater priceUpdateWorker = new MarketPriceUpdater(aPIInterface, histories);
                    Auditor auditor = new Auditor(aPIInterface);
                    ShortTermBroker dayTrader = new ShortTermBroker(aPIInterface, histories, auditor);

                    //put pointers to any events here:

                    Thread callTimer = new Thread(new ApiCallTimer(aPIInterface).StartEventLoop);
                    Thread priceUpdater = new Thread(priceUpdateWorker.UpdatePrices);
                    Thread runAuditor = new Thread(auditor.StartEventLoop);
                    Thread shortTermBroker = new Thread(dayTrader.StartEventLoop);

                    //name each of the threads
                    Thread.CurrentThread.Name = "Main";
                    callTimer.Name = "Timer";
                    priceUpdater.Name = "Updater";
                    runAuditor.Name = "Auditor";
                    shortTermBroker.Name = "S_Broker";

                    //start the workers
                    callTimer.Start();
                    priceUpdater.Start();
                    runAuditor.Start();
                    shortTermBroker.Start();
                    //threads terminate themselves

                    while (Globals.MarketAvaliability)
                    {
                        lock (aPIInterface)
                        {
                            Globals.LastMarketAvaliability = Globals.MarketAvaliability;
                            Globals.MarketAvaliability = aPIInterface.IsMarketOpen().Result;
                        }
                        //anything you want done while the market is open/threads are alive do here

                        Thread.Sleep(60000);
                    }
                }
                else if (!Globals.MarketAvaliability)
                {
                    Console.WriteLine("Market is closed");
                }
                //check for market avalibility every minute until it's open
                Thread.Sleep(60000);
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
