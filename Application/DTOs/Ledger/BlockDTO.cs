using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Ledger {
    public class BlockDTO {
        public string Hash { get; set; } = default!;
        public string PreviousHash { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public string? MinerNote { get; set; }

        public List<TransactionDTO> Transactions { get; set; } = new();
    }

}
