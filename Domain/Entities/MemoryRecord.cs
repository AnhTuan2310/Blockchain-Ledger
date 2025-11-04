using System.Linq;
using System;
namespace Domain.Entities {
    public class MemoryRecord {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime SnapshotTime { get; set; } = DateTime.Now;
        public List<Block> Blocks { get; set; } = new();
        public string Description { get; set; } = string.Empty;
        public string ChainStateJson { get; set; } = string.Empty;

        public MemoryRecord Clone() {
            return new MemoryRecord {
                Id = Guid.NewGuid(),
                SnapshotTime = SnapshotTime,
                Description = Description,
                Blocks = this.Blocks.Select(b => new Block {
                    Hash = b.Hash,
                    PreviousHash = b.PreviousHash,
                    Timestamp = b.Timestamp,
                    MinerNote = b.MinerNote,
                    Transactions = b.Transactions.Select(t => new Transaction {
                        Id = t.Id,
                        Author = t.Author,
                        Content = t.Content,
                        Timestamp = t.Timestamp
                    }).ToList()
                }).ToList()
            };
        }
    }
}
