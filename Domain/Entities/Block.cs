using System;

namespace Domain.Entities {
    public class Block {
        public string Data { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public string PreviousHash { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public List<Transaction> Transactions { get; set; } = new();
        public string MinerNote { get; set; } = string.Empty; //Tương tự Author bên Transaction
    }
}


