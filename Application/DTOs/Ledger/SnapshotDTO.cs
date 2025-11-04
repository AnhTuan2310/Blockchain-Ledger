using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Ledger {
    public class SnapshotDTO {
        public Guid SnapshotId { get; set; }
        public string Description { get; set; } = default!;
        public DateTime CreatedAt { get; set; }

        public List<BlockDTO> Chain { get; set; } = new();
        public List<TransactionDTO> Mempool { get; set; } = new();
    }
}
