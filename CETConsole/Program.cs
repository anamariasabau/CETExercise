using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CETConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //ImportData().Wait();
            DisplayOrderBook().Wait();
            System.Console.ReadLine();
        }


        static async Task DisplayOrderBook()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44352/");

                Console.WriteLine("Get order book");
                HttpResponseMessage response = await client.GetAsync("/api/OrderBook?asOf=2019-12-18T01:35:10.477Z");
                if (response.IsSuccessStatusCode)
                {
                    string output = await response.Content.ReadAsStringAsync();
                    var orderBookObj = JsonConvert.DeserializeObject<OrderBook>(output);
                   
                    System.Console.WriteLine(orderBookObj.ToString());

                }

            }
        }

        static Task ImportData()
        {
            bool hasHeader = true;
            char delimiter = ';';
            List<string> headerValues = null;
            List<object> dataToBeInserted = new List<object>();
            using (var reader = new StreamReader(@"C:\Users\anama\Downloads\testdata.csv"))
            {
                if (hasHeader)
                    headerValues = new List<string>(reader.ReadLine().Split(delimiter));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(delimiter);

                    ulong orderIdTemp;
                    double priceTemp;
                    int quantityTemp;
                    DateTimeOffset ts;

                    var orderParseSucces = ulong.TryParse(values[5], out orderIdTemp);
                    var priceParseSucces = double.TryParse(values[1],NumberStyles.Number,CultureInfo.InvariantCulture, out priceTemp);
                    var quantityParseSucces = int.TryParse(values[2], out quantityTemp);
                    var timestampParseSuccess = DateTimeOffset.TryParseExact(values[0], "yyyy-MM-dd HH:mm:ss.fff",CultureInfo.InvariantCulture,DateTimeStyles.None,out ts);


                    if (orderParseSucces && priceParseSucces && quantityParseSucces && timestampParseSuccess)
                    {
                        ulong? orderId = orderIdTemp;
                        double? price = priceTemp;
                        int? quantity = quantityTemp;


                        var obj = new
                        {
                            OrderId = orderId.Value,
                            Timestamp = ts.ToString("yyyy-MM-ddTHH:mm:ss.fffZ",CultureInfo.InvariantCulture),
                            Price = price.Value,
                            Quantity = quantity.Value,
                            Side = values[3],
                            Action = values[4]
                        };
                        dataToBeInserted.Add(obj);

                    }
                }
            }

            if(dataToBeInserted!=null && dataToBeInserted.Count != 0)
            {
                HttpClient client = new HttpClient();
                foreach(var entry in dataToBeInserted)
                {
                    string datatobeSent = JsonConvert.SerializeObject(entry);
                    InsertItem(datatobeSent,client).Wait();

                }
            }

            return Task.CompletedTask;
        }

        static async Task<bool> InsertItem(string datatobeSent, HttpClient client)
        {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var content = new StringContent(datatobeSent, Encoding.UTF8, "application/json");

                    Console.WriteLine("Add order actions to the database");
                    HttpResponseMessage response = await client.PostAsync("https://localhost:44352/api/OrderAction", content);

                    if (response.IsSuccessStatusCode)
                    {
                        System.Console.WriteLine(datatobeSent + " inserted succesfully");
                        return true;
                    }
                    else
                    { 
                        System.Console.WriteLine(response.StatusCode);
                    return false;
                    }

        }
    }
}
