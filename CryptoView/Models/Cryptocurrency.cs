namespace CryptoView.Models
{
    public class Cryptocurrency
    {
        public string LogoUrl { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal PriceChangePercentage { get; set; }
    }
}
