using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Text;
using AlpacaTradingApp.config;
using System.Threading;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

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
                if (Globals.MarketAvaliability)
                {
                    foreach (SymbolHistory item in symbolHistories)
                    {
                        if (Globals.MarketAvaliability)
                        {
                            decimal newprice;
                            lock (client)
                            {
                                lock (symbolHistories)
                                {
                                    newprice = APIPortal.PriceCheck(client, item.Symbol).Result;
                                    Globals.ApiCalls--;
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
                    //TODO:
                    /////////////////////////////////////////////////
                    //Saving temporaraly commented out until a long term broker is created
                    //unneeded until that day
                    /////////////////////////////////////////////////
                    EndTime = DateTime.Now;
                    //foreach (SymbolHistory history in symbolHistories)
                    //{
                    //    string currentFileLocation = Path.Combine(Config.SaveFolder, history.Symbol + " " + DateTime.Today.Day + "-" + DateTime.Today.Month + "-" + DateTime.Today.Year + ".txt");
                    //    File.AppendAllText(currentFileLocation,
                    //        history.Symbol + "\n" +
                    //        DateTime.Today.ToShortDateString() + "\n" +
                    //        "Started: " + StartTime.ToShortTimeString() + "\n" +
                    //        "Ended: " + EndTime.ToShortTimeString() + "\n\n"
                    //        );
                    //    StringBuilder builder = new StringBuilder();
                    //    foreach (float price in history.PriceHistory.Where(x => x != 0))
                    //    {
                    //        builder.Append(price.ToString()).AppendLine();
                    //    }
                    //    File.AppendAllText(currentFileLocation, builder.ToString());
                    //}
                    break;
                }
            }
        }
    }
}
