using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using AlpacaTradingApp.config;

namespace AlpacaTradingApp.workers
{
    class ApiCallTimer
    {
        public ApiCallTimer() { }

        public void StartEventLoop()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (Globals.LastMarketAvaliability||Globals.MarketAvaliability)
            {
                if (stopwatch.ElapsedMilliseconds >= 60000)
                {
                    Console.WriteLine("\ntimer has elapsed 1 minute. resetting");
                    Console.WriteLine("calls made: " + Globals.ApiCalls);
                    Globals.ApiCalls = 0;
                    Console.WriteLine("milliseconds on the clock: "+stopwatch.ElapsedMilliseconds);
                    stopwatch.Restart();
                }
            }
            Globals.ApiCalls = 0;
        }
    }
}
