using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Ledger {
    public class TransactionDTO {
        public Guid Id { get; set; }
        public string Content { get; set; } = default!;
        public string Author { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        
    }
}
