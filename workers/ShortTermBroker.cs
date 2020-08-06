using Alpaca.Markets;
using AlpacaTradingApp.config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AlpacaTradingApp.workers
{
    class ShortTermBroker
    {
        private AlpacaTradingClient client;
        private List<SymbolHistory> symbolHistories;
        private Auditor linkedAuditor;

        public ShortTermBroker() { }
        public ShortTermBroker(AlpacaTradingClient givenClient, List<SymbolHistory> givenHistories)
        {
            client = givenClient;
            symbolHistories = givenHistories;
        }
        public ShortTermBroker(AlpacaTradingClient givenClient, List<SymbolHistory> givenHistories, Auditor auditor)
        {
            client = givenClient;
            symbolHistories = givenHistories;
            linkedAuditor = auditor;
        }
        public void LinkAuditor(Auditor auditor)
        {
            linkedAuditor = auditor;
        }

        public void StartEventLoop()
        {
            while (Globals.MarketAvaliability)
            {
                //keep in mind that you want to settle old buisness before starting new buisness
                //in other words sell before you buy

                //if there is stuff to sell then do it first
                if (linkedAuditor.assets != null && linkedAuditor.assets.Count > 0)
                {
                    foreach (Asset asset in linkedAuditor.assets)
                    {
                        //if an asset is good to sell or is past a loss threshold,    ==>                                                   and doesn't have an active sell order, sell it
                        if (asset.CurrentPrice >= asset.PurchasedAt+(decimal).02 || asset.CurrentPrice <= asset.PurchasedAt - 1 || linkedAuditor.orders.Where(x => x.Symbol == asset.Symbol).Count() > 0)
                        {
                            APIPortal.PlaceSellOrder(client, asset.Symbol, asset.Quantity);
                            //wait a second to sell
                            Thread.Sleep(2000);

                        }
                    }
                }
                Thread.Sleep(60000);
            }
        }
    }
}
