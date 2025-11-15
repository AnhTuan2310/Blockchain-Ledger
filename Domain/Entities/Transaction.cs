using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities {
    public class Transaction {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Author { get; set; } = "System"; //Tác giả của chính giao dịch trong Block

        //FK
        public string? BlockHash { get; set; }
        public Block? Block { get; set; } // Navigation
    }
}
