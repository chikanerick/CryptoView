using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CryptoView.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CryptoView.Controllers
{
    public class LargeDealsController : Controller
    {
        private readonly ILogger<LargeDealsController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public LargeDealsController(ILogger<LargeDealsController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var largeDealsModel = new LargeDealsModel();
                var client = _httpClientFactory.CreateClient();
                string apiKey = "YOUR_API_KEY_HERE";
                string apiUrl = $"https://api.etherscan.io/api?module=account&action=txlist&address=0x1e0447b19bb6ecfdae1e4ae1694b0c3659614e4e&sort=desc&apikey={apiKey}";

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<EtherscanResponse>(jsonString);

                    if (result != null && result.Status == "1" && result.Result != null)
                    {
                        largeDealsModel.LargeTransactions = new List<Transaction>();
                        foreach (var item in result.Result)
                        {
                            largeDealsModel.LargeTransactions.Add(new Transaction
                            {
                                Hash = item.TransactionHash,
                                From = item.From,
                                To = item.To,
                                Value = item.Value,
                                GasPrice = item.GasPrice,
                                GasUsed = item.GasUsed,
                                Timestamp = UnixTimeStampToDateTime(item.TimeStamp)
                            });
                        }
                    }
                }

                // Подождите, пока данные будут загружены, прежде чем передавать модель представлению
                return View(largeDealsModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching large transactions: {ex.Message}");
                return View(new LargeDealsModel());
            }
        }

        private DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            double timeStamp = double.Parse(unixTimeStamp);
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(timeStamp).ToLocalTime();
            return dateTime;
        }
    }

    public class EtherscanResponse
    {
        public string Status { get; set; }
        public TransactionItem[] Result { get; set; }
    }

    public class TransactionItem
    {
        public string TransactionHash { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Value { get; set; }
        public string GasPrice { get; set; }
        public string GasUsed { get; set; }
        public string TimeStamp { get; set; }
    }
}