using System;
using System.Collections.Generic;
using Application.DTOs.Transactions;

namespace Application.DTOs.Blocks {
    public class BlockSearchResultDTO {
        public string Hash { get; set; } = string.Empty;
        public string PreviousHash { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string MinerNote { get; set; } = string.Empty;

        // Danh sách transaction match với keyword
        public List<TransactionDTO> MatchedTransactions { get; set; } = new();
    }
}
