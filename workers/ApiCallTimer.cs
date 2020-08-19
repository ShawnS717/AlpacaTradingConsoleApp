﻿using System;
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
                    Console.WriteLine("\ntimer has elapsed 1 minute. resetting");
                    Console.WriteLine("non updater calls made: " + Interface.apiCalls);
                    lock (Interface)
                    {
                        Interface.apiCalls = 0;
                    }
                    Console.WriteLine("milliseconds on the clock: " + stopwatch.ElapsedMilliseconds);
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
