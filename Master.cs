using Alpaca.Markets;
using AlpacaTradingApp.workers;
using AlpacaTradingApp.config;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;
using System.Linq;

namespace AlpacaTradingApp
{
    class Master
    {
        static void Main(string[] args)
        {
            //remove when finished or keep stored away for later vvv
            Console.WriteLine($"symbols with a price under {Globals.InvestingMaxAmount}");
            OutputViableSymbols(new APIInterface());
            Environment.Exit(0);
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
        public static void OutputViableSymbols(APIInterface aPIInterface)
        {
            List<string> foundStrings = new List<string>();
            Dictionary<int, string> numberToCharacter = new Dictionary<int, string>
            {
                {0,null },
                {1,"A" },
                {2,"B" },
                {3,"C" },
                {4,"D" },
                {5,"E" },
                {6,"F" },
                {7,"G" },
                {8,"H" },
                {9,"I" },
                {10,"J" },
                {11,"K" },
                {12,"L" },
                {13,"M" },
                {14,"N" },
                {15,"O" },
                {16,"P" },
                {17,"Q" },
                {18,"R" },
                {19,"S" },
                {20,"T" },
                {21,"U" },
                {22,"V" },
                {23,"W" },
                {24,"X" },
                {25,"Y" },
                {26,"Z" }
            };

            int tank1 = 0, tank2 = 0, tank3 = 0, tank4 = 0, tank5 = 0, tank6 = 0;
            while (tank6 < 27)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(numberToCharacter[tank1]).Append(numberToCharacter[tank2]).Append(numberToCharacter[tank3]).Append(numberToCharacter[tank4]).Append(numberToCharacter[tank5]).Append(numberToCharacter[tank6]);
                tank1++;
                if (tank1 > 26)
                {
                    tank2++;
                    tank1 = 0;
                }
                if (tank2 > 26)
                {
                    tank3++;
                    tank2 = 0;
                }
                if (tank3 > 26)
                {
                    tank4++;
                    tank3 = 0;
                }
                if (tank4 > 26)
                {
                    tank5++;
                    tank4 = 0;
                }
                if (tank5 > 26)
                {
                    tank6++;
                    tank5 = 0;
                }
                //after the string has been built check it against the api
                if (!foundStrings.Contains(stringBuilder.ToString()))
                {
                    if (aPIInterface.IsAssetTradable(stringBuilder.ToString()).Result)
                    {
                        if (aPIInterface.TempPriceCheck(stringBuilder.ToString()).Result < Globals.InvestingMaxAmount)
                        {
                            //if it fits all the criteria then put it in the list
                            foundStrings.Add(stringBuilder.ToString());
                        }
                    } 
                }
                Thread.Sleep(333);
            }
            //report the findings
            foundStrings = foundStrings.Distinct().ToList();
            Console.WriteLine("{");
            foreach(var item in foundStrings)
            {
                Console.WriteLine(item+",");
            }
            Console.WriteLine("}");
        }
    }
}
