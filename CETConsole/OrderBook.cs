using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CETConsole
{
    public class OrderBookEntry
    {
        public long OrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public Double Price { get; set; }
        public int Quantity { get; set; }
        public string Side { get; set; }
        public string Action { get; set; }
    }
    class OrderBook
    {

        public List<OrderBookEntry> Bids { get; set; }

        public List<OrderBookEntry> Asks { get; set; }
  
        public override String ToString()
        {
           String bidTemplate =  "{3}-{2}: Someone bids {0} EUR/MWh for {1} MWh";
           String askTemplate = "{3}-{2}: Someone asks {0} EUR/MWh for {1} MWh";
           StringBuilder bidString = new StringBuilder("Bids (someone wanting to buy)").Append(Environment.NewLine);
           StringBuilder askString = new StringBuilder("Asks (someone wanting to sell)").Append(Environment.NewLine);
            foreach (var bid in this.Bids)
            {
               
                bidString.Append(String.Format(bidTemplate,bid.Price, bid.Quantity, bid.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),bid.OrderId)).Append(Environment.NewLine);
            }

            foreach (var ask in this.Asks.OrderByDescending(o=>o.Price))
            {
                askString.Append(string.Format(askTemplate, ask.Price, ask.Quantity, ask.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),ask.OrderId)).Append(Environment.NewLine);
            }

            return bidString.Append(askString).ToString();
        }

    }
}
