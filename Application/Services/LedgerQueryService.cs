using Application.DTOs;
using Application.DTOs.Ledger;
using Application.IRepositories;
using Application.IServices;
using Domain.Entities;

namespace Application.Services {
    public class LedgerQueryService : ILedgerQueryService {
        private readonly ILedgerRepository _ledgerRepository;

        public LedgerQueryService(ILedgerRepository ledgerRepository) {
            _ledgerRepository = ledgerRepository;
        }

        public async Task<IEnumerable<BlockSearchResultDTO>> SearchBlocksAsync(string keyword) {
            keyword = keyword.ToLower();

            var chain = await _ledgerRepository.GetCurrentChainAsync();

            var results = chain
                .Select(block => {
                    var matchedTx = block.Transactions
                        .Where(t =>
                            t.Content.ToLower().Contains(keyword) ||
                            t.Author.ToLower().Contains(keyword)
                        )
                        .Select(t => new TransactionDTO {
                            Id = t.Id,
                            Content = t.Content,
                            Author = t.Author,
                            Timestamp = t.Timestamp
                        })
                        .ToList();

                    bool blockMatched =
                        block.Hash.ToLower().Contains(keyword) ||
                        block.MinerNote.ToLower().Contains(keyword);

                    if (!blockMatched && matchedTx.Count == 0)
                        return null;

                    return new BlockSearchResultDTO {
                        Hash = block.Hash,
                        PreviousHash = block.PreviousHash,
                        Timestamp = block.Timestamp,
                        MinerNote = block.MinerNote,
                        MatchedTransactions = matchedTx
                    };
                })
                .Where(r => r != null)
                .Select(r => r!)
                .OrderByDescending(r =>
                    r.MatchedTransactions
                        .OrderByDescending(t => t.Timestamp)
                        .FirstOrDefault()?.Timestamp
                )
                .ToList();

            return results;
        }

        public async Task<IEnumerable<BlockSearchResultDTO>> GetBlockSummariesAsync() {
            var chain = await _ledgerRepository.GetCurrentChainAsync();

            return chain
                .Select(b => new BlockSearchResultDTO {
                    Hash = b.Hash,
                    PreviousHash = b.PreviousHash,
                    Timestamp = b.Timestamp,
                    MinerNote = b.MinerNote,
                    MatchedTransactions = b.Transactions
                        .Select(t => new TransactionDTO {
                            Id = t.Id,
                            Content = t.Content,
                            Author = t.Author,
                            Timestamp = t.Timestamp
                        })
                        .ToList()
                })
                .OrderByDescending(r => r.Timestamp);
        }

        public async Task<bool> ValidateChainAsync() {
            var chain = await _ledgerRepository.GetCurrentChainAsync();

            return chain.Zip(chain.Skip(1), (prev, next) =>
                next.PreviousHash == prev.Hash).All(x => x);
        }

        public Task<List<Block>> GetCurrentChainAsync()
            => _ledgerRepository.GetCurrentChainAsync();
    }
}
