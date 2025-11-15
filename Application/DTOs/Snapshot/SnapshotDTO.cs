using Application.DTOs.Blocks;
using Application.DTOs.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Snapshot {
    public class SnapshotDTO {

        public Guid Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Description { get; set; } = default!;


        public DateTime CreatedAt { get; set; }

        public List<BlockDTO> Chain { get; set; } = new();
        public List<TransactionResponseDTO> Mempool { get; set; } = new();
    }
}
