using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CryptoView.Views.Home
{
    public class LargeDealsModel : PageModel
    {
        public List<Transaction> LargeTransactions { get; set; }

        private readonly IHttpClientFactory _clientFactory;

        public LargeDealsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGet()
        {
            try
            {
                var transactions = await GetLargeTransactions();
                LargeTransactions = transactions ?? new List<Transaction>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching transactions: {ex.Message}");
                LargeTransactions = new List<Transaction>(); // Initialize with empty list in case of error
            }
        }

        private async Task<List<Transaction>> GetLargeTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();

            try
            {
                var client = _clientFactory.CreateClient();
                string apiKey = "YOUR_API_KEY_HERE";
                string apiUrl = $"https://api.etherscan.io/api?module=account&action=txlist&address=0x1e0447b19bb6ecfdae1e4ae1694b0c3659614e4e&sort=desc&apikey={apiKey}";

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<EtherscanResponse>(jsonString);

                    if (result != null && result.Status == "1" && result.Result != null)
                    {
                        foreach (var item in result.Result)
                        {
                            transactions.Add(new Transaction
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
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error: {ex.Message}");
            }

            return transactions;
        }

        private DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            double timeStamp = double.Parse(unixTimeStamp);
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(timeStamp).ToLocalTime();
            return dateTime;
        }
    }

    public class Transaction
    {
        public string Hash { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Value { get; set; }
        public string GasPrice { get; set; }
        public string GasUsed { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class EtherscanResponse
    {
        public string Status { get; set; }
        public List<TransactionItem> Result { get; set; }
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