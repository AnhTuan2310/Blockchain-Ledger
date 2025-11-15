using System.ComponentModel.DataAnnotations;

namespace Domain.Entities {
    public class MemoryRecord {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Mô tả snapshot
        [Required]
        [StringLength(300)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // JSON serialized state
        public string ChainStateJson { get; set; } = string.Empty;
        public string PendingTransactionsJson { get; set; } = string.Empty;
    }
}
