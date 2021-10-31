using CETAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CETAPI.Enums;
using System.Text.Json.Serialization;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderActionController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public OrderActionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/<OrderActionController>
        /// <summary>
        /// API GET method retrieving all of the order actions saved in the database
        /// </summary>
        /// <returns>Json object with the list of order actions</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<OrderActionModel> result = new List<OrderActionModel>();
                string connString = ConfigurationExtensions.GetConnectionString(_configuration, "DefaultConnection");
                if (string.IsNullOrEmpty(connString))
                    return StatusCode(500, $"Database connection string missing");

                using (SqlConnection conn = new SqlConnection(connString))
                {

                    {
                        SqlCommand cmd = new SqlCommand("SELECT * from OrderAction", conn);
                        cmd.CommandType = CommandType.Text;
                        conn.Open();

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
                if (result != null && result.Count != 0)
                    return Ok(result);
                else
                    return StatusCode(500, $"No action orders found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error ocurred while getting the list of all order actions {ex.Message}");
            }

        }




        // POST api/<OrderActionController>
        /// <summary>
        /// API POST method for adding an order action to the database
        /// The following operations are performed: 
        /// - Validate input json file 
        /// - Insert new order action into the database
        /// </summary>
        /// <param name="orderAction">Request parameter containing json file with the values to be inserted: OrderId, Timestamp, Price, Quantity, Side, Action</param>
        /// <returns>Success message or Failed message, including error</returns>
        [HttpPost]
        public IActionResult CreateOrderAction([FromBody] OrderActionModel orderAction)
        {


            try
            {
                //Verifying the validity of the input data as specified in the model attributes
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }



                string connString = ConfigurationExtensions.GetConnectionString(_configuration, "DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    //Inserting OrderAction data into the database
                    string query = "insert into OrderAction values (@OrderId, @Timestamp, @Price,@Quantity,@Side,@Action)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Connection = conn;
                        cmd.Parameters.AddWithValue("@OrderId", orderAction.OrderId);
                        cmd.Parameters.AddWithValue("@Timestamp", orderAction.Timestamp);
                        cmd.Parameters.AddWithValue("@Price", orderAction.Price);
                        cmd.Parameters.AddWithValue("@Quantity", orderAction.Quantity);
                        cmd.Parameters.AddWithValue("@Side", Enum.GetName(typeof(Enums.Side), orderAction.Side));
                        cmd.Parameters.AddWithValue("@Action", Enum.GetName(typeof(Enums.ActionType), orderAction.Action));
                        conn.Open();
                        int i = cmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            conn.Close();
                            return Ok();
                        }
                        else
                        {
                            conn.Close();
                            return StatusCode(500, $"Error ocurred while inserting order action in database");
                        }


                    }

                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error ocurred while inserting order action {ex.Message}");
            }
        }
  
    }
}
