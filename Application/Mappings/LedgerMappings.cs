using Application.DTOs.Ledger;
using Domain.Entities;

namespace Application.Mappings {
    public static class LedgerMappings {
        //Mapping cho Block
        public static BlockDTO ToDTO(this Block block) {
            return new BlockDTO {
                Hash = block.Hash,
                PreviousHash = block.PreviousHash,
                Timestamp = block.Timestamp,
                MinerNote = block.MinerNote,
                Transactions = block.Transactions?
                .Select(t => t.ToDTO())
                .ToList() ?? new()
            };
        }
        public static Block ToDomain(this BlockDTO dto) {
            return new Block {
                Hash = dto.Hash,
                PreviousHash = dto.PreviousHash,
                Timestamp = dto.Timestamp,
                MinerNote = dto.MinerNote,
                Transactions = dto.Transactions?
                .Select(t => t.ToDomain())
                .ToList() ?? new()
            };
        }
        //Mapping cho Transaction
        public static TransactionDTO ToDTO(this Transaction transaction) {
            return new TransactionDTO {
                Id = transaction.Id,
                Content = transaction.Content,
                Timestamp = transaction.Timestamp,
                Author = transaction.Author
            };
        }
        public static Transaction ToDomain(this TransactionDTO dto) {
            return new Transaction {
                Id = dto.Id,
                Content = dto.Content,
                Timestamp = dto.Timestamp,
                Author = dto.Author
            };
        }
        //Mapping cho Snapshot
        public static SnapshotDTO ToSnapshotDto(
           Guid snapshotId,
           string description,
           DateTime createdAt,
           List<Block> chain,
           List<Transaction> mempool) {
            return new SnapshotDTO {
                SnapshotId = snapshotId,
                Description = description,
                CreatedAt = createdAt,
                Chain = chain.Select(b => b.ToDTO()).ToList(),
                Mempool = mempool.Select(t => t.ToDTO()).ToList()
            };
        }
        public static (List<Block> Chain, List<Transaction> Mempool) ToDomain( this SnapshotDTO dto) {
            var chain = dto.Chain.Select(b => b.ToDomain()).ToList();
            var mempool = dto.Mempool.Select(t => t.ToDomain()).ToList();
            return (chain, mempool);
        }
    }
}
