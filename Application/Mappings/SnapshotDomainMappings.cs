using System.Text.Json;
using Domain.Entities;

namespace Application.Mappings {
    public static class SnapshotDomainMappings {

        public static (List<Block> Chain, List<Transaction> Mempool) ToDomain(this MemoryRecord record) {

            var chain = string.IsNullOrWhiteSpace(record.ChainStateJson)
                ? new List<Block>()
                : JsonSerializer.Deserialize<List<Block>>(record.ChainStateJson)
                  ?? new List<Block>();

            var mempool = string.IsNullOrWhiteSpace(record.PendingTransactionsJson)
                ? new List<Transaction>()
                : JsonSerializer.Deserialize<List<Transaction>>(record.PendingTransactionsJson)
                  ?? new List<Transaction>();

            return (chain, mempool);
        }
    }
}
