using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Ledger {
    public class BlockSearchResultDTO {
        public string Hash { get; set; } = default!;
        public string PreviousHash { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public string MinerNote { get; set; } = default!;
        public List<TransactionDTO> MatchedTransactions { get; set; } = new();
    }
}
