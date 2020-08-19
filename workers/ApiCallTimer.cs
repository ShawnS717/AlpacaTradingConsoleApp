using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using AlpacaTradingApp.config;

namespace AlpacaTradingApp.workers
{
    class ApiCallTimer
    {
        private APIInterface Interface;
        public ApiCallTimer() { }
        public ApiCallTimer(APIInterface aPIInterface)
        {
            Interface = aPIInterface;
        }

        public void StartEventLoop()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (Globals.LastMarketAvaliability || Globals.MarketAvaliability)
            {
                if (stopwatch.ElapsedMilliseconds >= 60000)
                {
                    Console.WriteLine("non updater calls made: " + Interface.apiCalls);
                    lock (Interface)
                    {
                        Console.WriteLine("callers were:");
                        foreach (string item in Interface.callers)
                        {
                            Console.WriteLine("\t" + item);
                        }
                        Interface.apiCalls = 0;
                        Interface.callers.Clear();
                    }
                    stopwatch.Restart();
                }
            }
            lock (Interface)
            {
                Interface.apiCalls = 0;
            }
        }
    }
}
