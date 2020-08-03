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
                
            }
        }
    }
}
