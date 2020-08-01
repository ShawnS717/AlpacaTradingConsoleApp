using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Text;
using AlpacaTradingApp.config;
using System.Threading;

namespace AlpacaTradingApp.workers
{
    public class MarketPriceUpdater
    {
        private readonly AlpacaDataClient client;
        private readonly List<SymbolHistory> symbolHistories;

        public MarketPriceUpdater() { }
        public MarketPriceUpdater(AlpacaDataClient givenClient, List<SymbolHistory> givenSymbolHistories)
        {
            client = givenClient;
            symbolHistories = givenSymbolHistories;
        }

        public void UpdatePrices()
        {
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
                    //put after action report saving here
                    
                    break;
                }
            }
        }
    }
}
