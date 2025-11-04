using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs {
    public class BlockSummaryDTO {
        public string Hash { get; set; } = default!;
        public string PreviousHash { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public int TransactionCount { get; set; }
        public string MinerNote { get; set; } = default!;
    }
}
