﻿using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlpacaTradingApp
{
    public class SymbolHistory
    {      
        private int indexer = 0;

        public string Symbol;
        public float[] PriceHistory = new float[1440000];
        //4 hours worth of 1 update/sec ^^^
        //currently set to update 1 history every half second
        //to last 8 hours do 1 update every 2 seconds (watch 4 symbols at 1 check/half sec)
        public float Average
        {
            get
            {
                return (PriceHistory[0]==0)?0:PriceHistory.Where(x => x != 0).Average();
            }
        }
        public float UpperAverage
        {
            get
            {
                float average = Average;
                float upperAverage = PriceHistory.Where(x => x >= average).Average();
                return upperAverage;
            } 
        }
        public float LowerAverage
        {
            get
            {
                float average = Average;
                float lowerAverage = PriceHistory.Where(x => x != 0).Where(x => x <= average).Average();
                return lowerAverage;
            }
        }

        public SymbolHistory()
        {
            for (int i = 0; i < PriceHistory.Length; i++)
            {
                PriceHistory[i] = 0;
            }
        }

        public SymbolHistory(string symbol)
        {
            for (int i = 0; i < PriceHistory.Length; i++)
            {
                PriceHistory[i] = 0;
            }
            this.Symbol = symbol;
        }

        public SymbolHistory(IPosition position)
        {
            for (int i = 0; i < PriceHistory.Length; i++)
            {
                PriceHistory[i] = 0;
            }
            this.Symbol = position.Symbol;
        }

        /// <summary>
        /// updates the current price with whatever is passed to it
        /// </summary>
        /// <param name="newPrice">the price to update the internal array with</param>
        /// <returns>either true for successfull or false for array is full</returns>
        public bool UpdateCurrentPrice(decimal newPrice)
        {
            if (PriceHistory[^1] == 0)
            {
                PriceHistory[indexer] = (float)newPrice;
                indexer++;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return
                $"Symbol: {Symbol}\n" +
                $"Current Price: {PriceHistory[indexer-1]}\n" +
                $"Average: {Average}\n" +
                $"Upper bound average: {UpperAverage}\n" +
                $"Lower bound average: {LowerAverage}\n";
        }
    }
}
