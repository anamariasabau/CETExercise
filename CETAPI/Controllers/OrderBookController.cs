using CETAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Newtonsoft.Json;

namespace CETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderBookController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OrderBookController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // GET api/OrderBookController?asOf=2020-08-13T23:56:57.000Z
        /// <summary>
        /// API GET method retrieving the order book as of a specific date 
        /// The following operations are performed: 
        ///- Extract all order actions before a given date; assumning date is included
        ///- Remove all cancelled order actions from the extracted list of order actions
        ///- Keep only the last update of an order action in the list
        ///- Extract the bids and the asks and order accordingly 
        ///Bids: the higher the price, the better position in the book. If two bids have the same price, the one that was changed earliest wins
        ///Asks: the lower the price, the better position in the book. If two asks have the same price, the one that was changed earliest wins
        /// </summary>
        /// <param name="asOf">DateTime representing the date cutoff for extracting a snapshot of the order book</param>
        /// <returns>Json object containing a list of Bids and a list of Asks/returns>
        [HttpGet]
        public IActionResult Get(DateTime? asOf)
        {
            try
            {
                List<OrderActionModel> result = new List<OrderActionModel>();
                if (!asOf.HasValue)
                    return StatusCode(500, $"No date time selector has been passed to the API call");

                string connString = ConfigurationExtensions.GetConnectionString(_configuration, "DefaultConnection");

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT * from OrderAction where DATEDIFF(millisecond,Timestamp,@asOf)>=0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Connection = conn;
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@asOf";
                        param.SqlDbType = SqlDbType.DateTimeOffset;
                        param.Precision = 3;
                        param.Value = asOf.Value.ToString("yyyy-MM-ddTHH:mm:ss.FFFZ", CultureInfo.InvariantCulture);
                        cmd.Parameters.Add(param);


                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            long orderId = Convert.ToInt64(reader["OrderId"]);
                            DateTimeOffset dtoff = (DateTimeOffset)reader["Timestamp"];
                            DateTime timestamp = new DateTime(dtoff.Ticks);
                            Double price = Convert.ToDouble(reader["Price"]);
                            int quantity = Convert.ToInt32(reader["Quantity"]);
                            Enums.Side side = (Enums.Side)Enum.Parse(typeof(Enums.Side), Convert.ToString(reader["Side"]));
                            Enums.ActionType action = (Enums.ActionType)Enum.Parse(typeof(Enums.ActionType), Convert.ToString(reader["Action"]));
                            OrderActionModel orderAction = new OrderActionModel(orderId, timestamp, price, quantity, side, action);
                            result.Add(orderAction);

                        }

                        conn.Close();
                    }

                }

                var comparer = new OrderActionModelComparer();
                List<OrderActionModel> filteredOrderActions = new List<OrderActionModel>();
                var groupedOrders = result.GroupBy(o => new { o.OrderId, o.Side });
                foreach (var g in groupedOrders)
                {
                    //Delete order actions by OrderId, Price, Quantity and depending on the timestamp of the operation
                    List<OrderActionModel> groupedFilteredOrderActions = new List<OrderActionModel>();
                    var deleteActions = g.Where(o => o.Action == Enums.ActionType.Delete).Select(o => o).ToList();
                    var remainingActions = g.Where(o => o.Action != Enums.ActionType.Delete).Select(o => o).OrderByDescending(o => o.Timestamp).ToList();

                    if(deleteActions.Count != 0)
                    { 
                    foreach (var delAction in deleteActions)
                    {
                        foreach (var entry in remainingActions.Select((value, index) => (value, index)))
                        {
                            if (comparer.Equals(entry.value, delAction))
                            {

                                groupedFilteredOrderActions.AddRange(remainingActions.TakeLast(remainingActions.Count-(entry.index+1)));
                                break;
                            }
                            else
                                groupedFilteredOrderActions.Add(entry.value);
                        }
                    }
                        if (groupedFilteredOrderActions != null && groupedFilteredOrderActions.Count != 0)
                            filteredOrderActions.AddRange(groupedFilteredOrderActions);
                    }
                    else
                        filteredOrderActions.AddRange(remainingActions);

                }

                var bids = filteredOrderActions.Where(o => o.Side == Enums.Side.Bid);
                bids = bids.GroupBy(o => o.Price).Select(o => o.OrderBy(v => v.Timestamp).FirstOrDefault()).ToList().OrderByDescending(o => o.Price);
                var asks = filteredOrderActions.Where(o => o.Side == Enums.Side.Ask);
                asks = asks.GroupBy(o => o.Price).Select(o => o.OrderBy(v => v.Timestamp).FirstOrDefault()).OrderBy(o => o.Price);

                OrderBookModel orderBook = new OrderBookModel(bids, asks);
                return Ok(orderBook);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error ocurred while extracting the order book {ex.Message}");
            }

        }
    }

}
