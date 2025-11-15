using Application.DTOs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Blocks {
    public class BlockDetailDTO {
        public string Hash { get; set; } = "";
        public string PreviousHash { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string MinerNote { get; set; } = "";
        public List<TransactionResponseDTO> Transactions { get; set; } = new();
    }

}
