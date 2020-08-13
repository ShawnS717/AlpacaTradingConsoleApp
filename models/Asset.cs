using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlpacaTradingApp
{
    public class Asset
    {
        //TODO maby:
        //develop a long term storage solution to keeping track of what assets you have purchased
        public Guid AssetID { get; set; }
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public decimal ChangePercentage { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal PurchasedAt
        {
            get
            {
                return CurrentPrice * ChangePercentage;
            }
        }

        public Asset() { }
        public Asset(IPosition position)
        {
            AssetID = position.AssetId;
            Symbol = position.Symbol;
            Quantity = position.Quantity;
            ChangePercentage = position.AssetChangePercent;
            CurrentPrice = position.AssetCurrentPrice;
        }

        public override string ToString()
        {
            return
                $"AssetID: {AssetID}\n" +
                $"Symbol: {Symbol}\n" +
                $"Quantity: {Quantity}\n" +
                $"Change Percentage: {ChangePercentage}\n" +
                $"Current Price: {CurrentPrice}\n" +
                $"Purchased at: {PurchasedAt}\n";
        }
    }
}
