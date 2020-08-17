﻿using Alpaca.Markets;
using AlpacaTradingApp.workers;
using AlpacaTradingApp.config;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net.Http.Headers;

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

                    //make a new list of histories for todays workers
                    List<SymbolHistory> histories = CreateHistories();

                    //make and link the workers
                    MarketPriceUpdater priceUpdateWorker = new MarketPriceUpdater(dataClient, histories);
                    Auditor auditor = new Auditor(tradeClient);
                    ShortTermBroker dayTrader = new ShortTermBroker(tradeClient, histories, auditor);

                    //put pointers to any events here:

                    Thread callTimer = new Thread(new ApiCallTimer().StartEventLoop);
                    Thread priceUpdater = new Thread(priceUpdateWorker.UpdatePrices);
                    Thread runAuditor = new Thread(auditor.StartEventLoop);
                    Thread shortTermBroker = new Thread(dayTrader.StartEventLoop);

                    //start the workers
                    callTimer.Start();
                    priceUpdater.Start();
                    runAuditor.Start();
                    shortTermBroker.Start();

                    //while the market is open, check on the threads
                    while (Globals.MarketAvaliability)
                    {
                        //every 30 seconds check the threads
                        Thread.Sleep(30000);

                        Console.WriteLine(callTimer.ThreadState);
                        Console.WriteLine(priceUpdater.ThreadState);
                        Console.WriteLine(runAuditor.ThreadState);
                        Console.WriteLine(shortTermBroker.ThreadState);
                    }
                }
                else if(!Globals.MarketAvaliability)
                {
                    Console.WriteLine("Market is closed");
                }
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
