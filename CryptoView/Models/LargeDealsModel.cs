using System;
using System.Collections.Generic;

namespace CryptoView.Models
{
    public class LargeDealsModel
    {
        public List<Transaction> LargeTransactions { get; set; }
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
}