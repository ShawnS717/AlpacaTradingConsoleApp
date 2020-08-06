using Alpaca.Markets;
using AlpacaTradingApp.config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AlpacaTradingApp.workers
{
    //the job of the auditor is to keep track of what we have and to be questioned by
    //brokers to make decisions
    class Auditor
    {
        private AlpacaTradingClient client;
        public List<Asset> assets;
        public IReadOnlyList<IOrder> orders;

        public Auditor() { }
        public Auditor(AlpacaTradingClient givenClient)
        {
            client = givenClient;
        }

        public void StartEventLoop()
        {
            while (Globals.MarketAvaliability)
            {
                //get all the account assets
                lock (client)
                {
                    assets = APIPortal.GetAllAssetInfo(client).Result.ToList();
                    Console.WriteLine("\nAuditer updated assets list");
                }
                lock (client)
                {
                    orders = APIPortal.GetOrders(client).Result;
                    Console.WriteLine("Auditer updated orders list");
                }

                Console.WriteLine("Assets owned:");
                foreach (Asset item in assets)
                {
                    Console.WriteLine("\t" + item.Symbol + " Qty: " + item.Quantity);
                }
                Console.WriteLine("Active orders:");
                foreach (IOrder item in orders)
                {
                    Console.WriteLine("\t" + item.Symbol + " Side: " + item.OrderSide);
                }
                Console.WriteLine();
                //wait 1 minute and update them again to stay up to date
                Thread.Sleep(60000);
            }
        }
    }
}
