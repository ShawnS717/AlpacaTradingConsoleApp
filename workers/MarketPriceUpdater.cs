using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Text;
using AlpacaTradingApp.config;
using System.Threading;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Threading.Tasks;

//this worker closes itself. no need to interupt it

namespace AlpacaTradingApp.workers
{
    public class MarketPriceUpdater
    {
        private APIInterface Interface;
        private List<SymbolHistory> symbolHistories;
        public DateTime StartTime;
        public DateTime EndTime;

        public MarketPriceUpdater() { }
        public MarketPriceUpdater(APIInterface aPIInterface, List<SymbolHistory> givenSymbolHistories)
        {
            Interface = aPIInterface;
            symbolHistories = givenSymbolHistories;
        }

        public async void UpdatePrices()
        {
            StartTime = DateTime.Now;
            while (Globals.MarketAvaliability)
            {
                var minuteTimer = Task.Run(() => Thread.Sleep(60000));
                foreach (SymbolHistory item in symbolHistories)
                {
                    if (Globals.MarketAvaliability)
                    {
                        decimal newprice;
                        lock (Interface)
                        {
                            lock (symbolHistories)
                            {
                                newprice = Interface.PriceCheck(item.Symbol).Result;
                            }
                        }
                        if (item.UpdateCurrentPrice(newprice))
                            Console.WriteLine("Price updated for: " + item.Symbol);

                        Thread.Sleep(500);
                    }
                    else
                    {
                        continue;
                    }
                }
                await minuteTimer;
            }
            //after the market closes do the saving

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

        }
    }
}
