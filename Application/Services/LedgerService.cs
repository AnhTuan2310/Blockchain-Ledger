using Application.DTOs;
using Application.DTOs.Ledger;
using Application.IRepositories;
using Application.IServices;
using Application.Mappings;
using Application.State;
using Domain.Entities;

namespace Application.Services {
    public class LedgerService : ILedgerService, ILedgerQueryService {
        private readonly BlockchainState _state;
        private readonly ILedgerRepository _repo;

        public LedgerService(BlockchainState state, ILedgerRepository repo) {
            _state = state;
            _repo = repo;
        }

        // ================================
        // COMMANDS (ILedgerService)
        // ================================

        public async Task AddTransactionAsync(Transaction tran) {
            await Task.Yield(); // giữ async signature theo convention
            _state.AddPendingTransaction(tran);
        }

        public async Task<Block> MineBlockAsync(string minerNote) {
            var pending = _state.DrainPendingTransactions();
            if (pending.Count == 0)
                throw new InvalidOperationException("No pending transactions to mine.");

            var currentChain = _state.Blocks;

            Block previous;
            if (currentChain.Count == 0) {
                previous = new Block {
                    Hash = "0",
                    Timestamp = DateTime.Now,
                    MinerNote = "GENESIS"
                };
                _state.AddBlock(previous);
                await _repo.AddBlockAsync(previous);
            } else {
                previous = currentChain.Last();
            }

            var block = new Block {
                PreviousHash = previous.Hash,
                Timestamp = DateTime.Now,
                MinerNote = minerNote,
                Transactions = pending
            };

            block.Hash = BlockchainState.ComputeHashForBlock(block);

            _state.AddBlock(block);
            await _repo.AddBlockAsync(block);

            return block;
        }

        public async Task<SnapshotDTO> CreateSnapshotAsync(string description) {
            var chain = _state.Blocks.ToList();
            var mempool = _state.PendingTransactions.ToList();

            var dto = await _repo.CreateSnapshotAsync(description, chain, mempool);
            return dto;
        }

        public async Task RollbackToSnapshotAsync(Guid snapId) {
            var snapshot = await _repo.GetSnapshotAsync(snapId);
            if (snapshot == null)
                throw new KeyNotFoundException("Snapshot not found.");

            var (chain, mempool) = snapshot.ToDomain();

            _state.ClearChainAndMempool();
            _state.ReplaceChain(chain);

            foreach (var tx in mempool)
                _state.AddPendingTransaction(tx);

            await _repo.RollbackToSnapshotAsync(snapshot);
        }

        // (Fork sẽ implement sau)
        public Task<Guid> ForkFromSnapshotAsync(Guid snapId, string newChainName) {
            throw new NotImplementedException();
        }

        // ================================
        // QUERIES (ILedgerQueryService)
        // ================================

        public async Task<List<Block>> GetCurrentChainAsync() {
            await Task.Yield();
            return _state.Blocks.ToList();
        }

        public async Task<bool> ValidateChainAsync() {
            await Task.Yield();

            var chain = _state.Blocks;
            return chain.Zip(chain.Skip(1), (prev, next) =>
                next.PreviousHash == prev.Hash).All(x => x);
        }

        public async Task<IEnumerable<BlockSearchResultDTO>> GetBlockSummariesAsync() {
            await Task.Yield();

            return _state.Blocks
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

        public async Task<IEnumerable<BlockSearchResultDTO>> SearchBlocksAsync(string keyword) {
            await Task.Yield();
            keyword = keyword.ToLower();

            var results = _state.Blocks
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
    }
}
