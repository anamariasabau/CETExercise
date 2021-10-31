using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CETAPI.Model
{
    public class OrderBookModel
    {

        public OrderBookModel(IEnumerable<OrderActionModel> bids, IEnumerable<OrderActionModel> asks)
        {
            this.Bids = bids;
            this.Asks = asks;
        }

        public IEnumerable<OrderActionModel> Bids { get; set; }
        public IEnumerable<OrderActionModel> Asks { get; set; }
    }
}
