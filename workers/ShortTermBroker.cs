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

        public ShortTermBroker() { }
        public ShortTermBroker(AlpacaTradingClient givenClient, List<SymbolHistory> givenHistories)
        {
            client = givenClient;
            symbolHistories = givenHistories;
        }

        public void EventLoop()
        {
            while (Config.MarketAvaliability)
            {
                //keep in mind that you want to settle old buisness before starting new buisness
                //in other words sell before you buy

            }
        }
    }
}
