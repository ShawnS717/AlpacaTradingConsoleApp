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
            while (Globals.LastMarketAvaliability)
            {
                if (stopwatch.ElapsedMilliseconds > 60000)
                {
                    Globals.ApiCalls = 0;
                    stopwatch.Restart();
                }
            }
            Globals.ApiCalls = 0;
        }
    }
}
