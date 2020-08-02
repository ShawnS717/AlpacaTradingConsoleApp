using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Text;
using AlpacaTradingApp.config;
using System.Threading;
using System.IO;
using System.Security.Cryptography.X509Certificates;

//this worker closes itself. no need to interupt it

namespace AlpacaTradingApp.workers
{
    public class MarketPriceUpdater
    {
        private readonly AlpacaDataClient client;
        private readonly List<SymbolHistory> symbolHistories;
        public DateTime StartTime;
        public DateTime EndTime;

        public MarketPriceUpdater() { }
        public MarketPriceUpdater(AlpacaDataClient givenClient, List<SymbolHistory> givenSymbolHistories)
        {
            client = givenClient;
            symbolHistories = givenSymbolHistories;
        }

        public void UpdatePrices()
        {
            StartTime = DateTime.Now;
            while (true)
            {
                if (Config.MarketAvaliability)
                {
                    foreach (SymbolHistory item in symbolHistories)
                    {
                        if (Config.MarketAvaliability)
                        {
                            decimal newprice;
                            lock (client)
                            {
                                lock (symbolHistories)
                                {
                                    newprice = APIPortal.PriceCheck(client, item.Symbol).Result;
                                }
                            }
                            if (item.UpdateCurrentPrice(newprice))
                                Console.WriteLine("Price updated for: " + item.Symbol);
                            //^^^if put into a winform app change the output location

                            Thread.Sleep(500);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    EndTime = DateTime.Now;
                    foreach(SymbolHistory history in symbolHistories)
                    {
                        string currentFileLocation = Path.Combine(Config.SaveFolder, history.Symbol, " ", DateTime.Today.ToShortDateString());
                        File.AppendAllText(currentFileLocation,
                            history.Symbol+"\n"+
                            DateTime.Today.ToShortDateString()+"\n"+
                            "Started: "+StartTime.ToShortTimeString()+"\n"+
                            "Ended: "+EndTime.ToShortTimeString()+"\n\n"
                            );
                        foreach(float price in history.PriceHistory)
                        {
                            File.AppendAllText(currentFileLocation, price + "\n");
                        }
                    }

                    break;
                }
            }
        }
    }
}
