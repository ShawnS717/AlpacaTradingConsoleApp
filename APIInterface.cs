using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//this is to replace the APIPortal when completed
//this object will be in charge of talking to the api instead of a static class
//instead of having to lock the clients you just lock this interface
namespace AlpacaTradingApp
{
    public class APIInterface
    {
        public AlpacaTradingClient tradeClient;
        public AlpacaDataClient dataClient;
        public byte apiCalls = 0;

        //constructors
        public APIInterface()
        {
            tradeClient = MakeTradingClient();
            dataClient = MakeDataClient();
        }
        //methods
        private void WaitUntilAvaliable()
        {
                                      //the updater thread is exempt from waiting
            while (apiCalls >= 79 && Thread.CurrentThread.Name != "Updater")
            {
                Console.WriteLine(Thread.CurrentThread.Name + "is waiting for the next round of calls");
                Thread.Sleep(500);
            }
            //if it's not the updater then increment
            if (Thread.CurrentThread.Name != "Updater")
            {
                apiCalls++; 
            }
        }
        public AlpacaTradingClient MakeTradingClient()
        {
            return Alpaca.Markets.Environments.Paper.GetAlpacaTradingClient(new SecretKey(config.Globals.APIKey, config.Globals.APISecretKey));
        }
        public AlpacaDataClient MakeDataClient()
        {
            return Alpaca.Markets.Environments.Paper.GetAlpacaDataClient(new SecretKey(config.Globals.APIKey, config.Globals.APISecretKey));
        }

        /// <summary>
        /// Gets a price check for a given symbol (rolls low)
        /// </summary>
        /// <param name="symbol">Symbol for the asset to check</param>
        /// <returns>A lowballed estimate for what the current price is</returns>
        public async Task<decimal> PriceCheck(string symbol)
        {
            WaitUntilAvaliable();
            try
            {
                var result = await dataClient.GetBarSetAsync(new BarSetRequest(symbol.ToUpper(), TimeFrame.Minute));
                return result[symbol].Last().Low;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
                return 0;
            }
        }

        /// <summary>
        /// This will Get all the positions for the account linked
        /// </summary>
        /// <returns>An array of Asset Objects</returns>
        public async Task<Asset[]> GetAllAssetInfo()
        {
            WaitUntilAvaliable();
            IReadOnlyList<IPosition> assets = null;
            try
            {
                assets = await tradeClient.ListPositionsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            Asset[] myAssets = new Asset[assets.Count];
            for (int i = 0; i < assets.Count; i++)
            {
                myAssets[i] = new Asset(assets[i]);
            }
            return myAssets;
        }

        /// <summary>
        /// this method gets all the buy and sell orders for the linked account
        /// </summary>
        /// <returns>a list of orders</returns>
        public async Task<IReadOnlyList<IOrder>> GetOrders()
        {
            WaitUntilAvaliable();
            IReadOnlyList<IOrder> orders = null;
            try
            {
                orders = await tradeClient.ListAllOrdersAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            return orders;
        }

        /// <summary>
        /// Returns a bool telling you if the market is open or not
        /// </summary>
        public async Task<bool> IsMarketOpen()
        {
            WaitUntilAvaliable();
            IClock clock = null;
            try
            {
                clock = await tradeClient.GetClockAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            if (clock.IsOpen)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Places an order for an asset and in what quantity
        /// </summary>
        /// <param name="symbol">the symbol for the asset you wish to get</param>
        /// <param name="qty">the quantity you wish to get</param>
        public async void PlaceBuyOrder(string symbol, int qty)
        {
            WaitUntilAvaliable();
            IOrder order = null;
            try
            {
                order = await tradeClient.PostOrderAsync(new NewOrderRequest(symbol.ToUpper(), qty, OrderSide.Buy, OrderType.Market, TimeInForce.Day));
                Console.WriteLine($"Buy order made for: {symbol.ToUpper()}, Qty: {qty}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Places an order to sell an asset and in what quantity
        /// </summary>
        /// <param name="symbol">the symbol for the assset to sell</param>
        /// <param name="qty">the quantity to sell</param>
        public  async void PlaceSellOrder(string symbol, int qty)
        {
            WaitUntilAvaliable();
            IOrder order = null;
            try
            {
                order = await tradeClient.PostOrderAsync(new NewOrderRequest(symbol.ToUpper(), qty, OrderSide.Sell, OrderType.Market, TimeInForce.Day));
                Console.WriteLine($"Sell order made for: {symbol.ToUpper()}, Qty: {qty}");
            }
            catch (Exception e)
            {
                Console.WriteLine("API Interface couldn't create the sell order because...");
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// if there is a need to get rid of an asset then use this to ditch it
        /// </summary>
        /// <param name="symbol">the symbol for what to remove</param>
        public async void EmergencyLiquidate(string symbol)
        {
            await tradeClient.DeletePositionAsync(new DeletePositionRequest(symbol));
        }




        //unused but may be usefull methods
        public async void GetAccountDetails()
        {
            WaitUntilAvaliable();;
            IAccount account = null;
            try
            {
                account = await tradeClient.GetAccountAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            if (account.IsTradingBlocked)
            {
                Console.WriteLine("Account is currently restricted from trading.");
                Environment.Exit(1);
            }
            Console.WriteLine("Some Account Details: ");
            Console.WriteLine(account.AccountId);
            Console.WriteLine(account.AccountNumber);
            Console.WriteLine(account.BuyingPower + " in buying power");
            Console.WriteLine(account.Equity + "in equity");
            Console.WriteLine(account.TradableCash + " in transferable cash");
        }

        public async void GetProfit()
        {
            WaitUntilAvaliable();
            try
            {
                var account = await tradeClient.GetAccountAsync();
                decimal ballanceChange = account.Equity - account.LastEquity;

                Console.WriteLine("Profit made: " + ballanceChange);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Use this to find out is a symbol for a asset is tradable
        /// </summary>
        /// <param name="symbol">Symbol to check if tradable</param>
        /// <returns>Boolean that says if it is tradable or not</returns>
        public async Task<bool> IsAssetTradable(string symbol)
        {
            WaitUntilAvaliable();
            try
            {
                var asset = await tradeClient.GetAssetAsync(symbol.ToUpper());
                if (asset.IsTradable)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
