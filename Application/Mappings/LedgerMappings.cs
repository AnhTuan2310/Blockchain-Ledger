using System.Linq;
using Application.DTOs.Blocks;
using Application.DTOs.Transactions;
using Domain.Entities;

namespace Application.Mappings {
    public static class LedgerMappings {
        public static TransactionResponseDTO ToDTO(this Transaction tx) =>
            new() {
                Id = tx.Id,
                Content = tx.Content,
                Author = tx.Author,
                Timestamp = tx.Timestamp
            };

        public static BlockDetailDTO ToDTO(this Block block) =>
            new() {
                Hash = block.Hash,
                PreviousHash = block.PreviousHash,
                MinerNote = block.MinerNote,
                Timestamp = block.Timestamp,
                Transactions = block.Transactions
                    .Select(t => t.ToDTO()).ToList()
            };

        public static BlockSearchResultDTO ToSearchResult(this Block block) =>
            new() {
                Hash = block.Hash,
                PreviousHash = block.PreviousHash,
                MinerNote = block.MinerNote,
                Timestamp = block.Timestamp,
                MatchedTransactions = block.Transactions
                    .Select(t => t.ToDTO()).ToList()
            };
    }
}
