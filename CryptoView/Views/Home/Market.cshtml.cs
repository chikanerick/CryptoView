using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace CryptoView.Views.Home
{
    public class MarketModel : PageModel
    {
        public List<CryptoInfo> CryptoInfos { get; set; }

        public async Task OnGet()
        {
            CryptoInfos = await GetCryptoData();
        }

        private async Task<List<CryptoInfo>> GetCryptoData()
        {
            List<CryptoInfo> cryptoInfos = new List<CryptoInfo>();

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync("https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=3&page=1&sparkline=false");
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    cryptoInfos = JsonConvert.DeserializeObject<List<CryptoInfo>>(content);
                }
                catch (Exception ex)
                {
                    // Обработка ошибок при запросе данных
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return cryptoInfos;
        }
    }

    public class CryptoInfo
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal MarketCap { get; set; }
    }
}