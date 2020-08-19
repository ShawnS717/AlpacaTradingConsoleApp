using Alpaca.Markets;
using AlpacaTradingApp.config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlpacaTradingApp.workers
{
    //the job of the auditor is to keep track of what we have and to be questioned by
    //brokers to make decisions
    class Auditor
    {
        private APIInterface Interface;
        public List<Asset> assets;
        public IReadOnlyList<IOrder> orders;

        public Auditor() { }
        public Auditor(APIInterface aPIInterface)
        {
            Interface = aPIInterface;
        }

        public async void StartEventLoop()
        {
            while (Globals.MarketAvaliability)
            {
                Task minuteTimer = Task.Run(() => Thread.Sleep(60000));
                //get all the account assets
                lock (Interface)
                {
                    assets = Interface.GetAllAssetInfo().Result.ToList();
                    Console.WriteLine("\nAuditer updated assets list");
                    orders = Interface.GetOrders().Result;
                    Console.WriteLine("Auditer updated orders list");
                }


                Globals.CurrentlyInvested = assets.Select(x => x.PurchasedAt).Sum();

                Console.WriteLine("Assets owned:");
                foreach (Asset item in assets)
                {
                    Console.WriteLine("\t" + item.Symbol + " Qty: " + item.Quantity + " Change perentage: " + item.ChangePercentage);
                }
                Console.WriteLine("Active orders:");
                foreach (IOrder item in orders)
                {
                    Console.WriteLine("\t" + item.Symbol + " Side: " + item.OrderSide);
                }

                //check if any of the positions owned are in the negatives and remove them
                foreach(Asset item in assets)
                {
                    if (item.Quantity < 0)
                    {
                        lock (Interface)
                        {
                            Interface.EmergencyLiquidate(item.Symbol);
                        }
                        Console.WriteLine("Asset found to have a quantity less than 0, has been deleted");
                    }
                }
                Console.WriteLine();
                //wait 1 minute and update them again to stay up to date
                await minuteTimer;
            }
        }
    }
}
