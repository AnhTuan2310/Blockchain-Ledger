using Application.DTOs;
using Application.DTOs.Blocks;
using Application.DTOs.Transactions;
using Application.DTOs.Snapshot;
using Application.IRepositories;
using Application.IServices;
using Application.Mappings;
using Application.State;
using Domain.Entities;

namespace Application.Services {
    public class LedgerService : ILedgerService {
        private readonly BlockchainState _state;
        private readonly ILedgerRepository _repo;
        public LedgerService(BlockchainState state, ILedgerRepository repo) {
            _state = state;
            _repo = repo;
        }

        //Blocks
        public async Task<Block> MineBlockAsync(string minerNote) {
            var pending = _state.DrainPendingTransactions();
            if (pending.Count == 0)
                throw new InvalidOperationException("No pending transactions to mine.");

            Block previous;

            if (_state.Blocks.Count == 0) {
                // Genesis block
                previous = new Block {
                    Hash = "0",
                    MinerNote = "GENESIS",
                    Timestamp = DateTime.UtcNow
                };

                _state.AddBlock(previous);
                await _repo.AddBlockAsync(previous);
            } else {
                previous = _state.Blocks.Last();
            }

            var block = new Block {
                PreviousHash = previous.Hash,
                Timestamp = DateTime.UtcNow,
                MinerNote = minerNote,
                Transactions = pending
            };

            block.Hash = BlockchainState.ComputeHashForBlock(block);

            _state.AddBlock(block);
            await _repo.AddBlockAsync(block);

            return block;
        }


        public async Task<IEnumerable<BlockSearchResultDTO>> GetBlockSummariesAsync() {
            await Task.Yield();

            return _state.Blocks
                .Select(b => new BlockSearchResultDTO {
                    Hash = b.Hash,
                    PreviousHash = b.PreviousHash,
                    Timestamp = b.Timestamp,
                    MinerNote = b.MinerNote,
                    MatchedTransactions = b.Transactions.Select(t => t.ToDTO()).ToList()
                })
                .OrderByDescending(r => r.Timestamp);
        }

        public async Task<IEnumerable<BlockSearchResultDTO>> SearchBlocksAsync(string keyword) {
            await Task.Yield();
            keyword = keyword.ToLower();

            return _state.Blocks
                .Select(block => {
                    var matchedTx = block.Transactions
                        .Where(t =>
                            t.Content.ToLower().Contains(keyword) ||
                            t.Author.ToLower().Contains(keyword))
                        .Select(t => t.ToDTO()) // convert đúng kiểu TransactionDTO
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
                        .FirstOrDefault()?.Timestamp)
                .ToList();
        }


        //Snapshot
        public async Task<SnapshotDTO> CreateSnapshotAsync(string description) {
            var chain = _state.Blocks.ToList();
            var mempool = _state.PendingTransactions.ToList();

            return await _repo.CreateSnapshotAsync(description, chain, mempool);
        }

        public async Task RollbackToSnapshotAsync(Guid snapId) {
            var snapshot = await _repo.GetSnapshotAsync(snapId);
            if (snapshot == null)
                throw new KeyNotFoundException("Snapshot not found.");

            var (chain, mempool) = snapshot.ToDomain();

            //Xóa trạng thái hiện tại và thay thế
            _state.ClearChainAndMempool();
            _state.ReplaceChain(chain);

            foreach (var tx in mempool)
                _state.AddPendingTransaction(tx);

            await _repo.RollbackToSnapshotAsync(snapshot);
        }

            //Fork timeline (chưa hoàn thiện)
        public Task<Guid> ForkFromSnapshotAsync(Guid snapId, string newChainName) {
            throw new NotImplementedException();
        }

        //Transaction
        public async Task AddTransactionAsync(Transaction transaction) {
            await Task.Yield();
            _state.AddPendingTransaction(transaction);
        }

        public async Task<Transaction> UpdateTransactionAsync(UpdateTransactionRequestDTO request) {
            await Task.Yield();

            // 1) Tìm trong mempool
            var tx = _state.PendingTransactions.FirstOrDefault(t => t.Id == request.Id);
            if (tx is not null) {
                tx.Content = request.Content.Trim();
                tx.Author = request.Author.Trim();
                tx.Timestamp = DateTime.UtcNow; // nếu muốn stamp thời điểm chỉnh sửa
                return tx;
            }

            // 2) Không ở mempool → kiểm tra chain
            var existsInBlock = _state.Blocks
                .SelectMany(b => b.Transactions)
                .Any(t => t.Id == request.Id);

            if (existsInBlock)
                throw new InvalidOperationException("Transaction has been mined. Update is not allowed.");

            throw new KeyNotFoundException("Transaction not found.");
        }
        public async Task DeleteTransactionAsync(Guid id) {
            await Task.Yield();

            // 1) Thử xoá khỏi mempool
            var removed = _state.PendingTransactions.RemoveAll(t => t.Id == id) > 0;
            if (removed) return;

            // 2) Không ở mempool → kiểm tra chain
            var existsInBlock = _state.Blocks
                .SelectMany(b => b.Transactions)
                .Any(t => t.Id == id);

            if (existsInBlock)
                throw new InvalidOperationException("Transaction has been mined. Delete is not allowed.");

            throw new KeyNotFoundException("Transaction not found.");
        }


        //Chain
        public async Task<List<Block>> GetCurrentChainAsync() {
            var dbChain = await _repo.GetCurrentChainAsync();
            _state.ReplaceChain(dbChain);
            return dbChain.ToList();
        }

        public async Task<bool> ValidateChainAsync() {
            await Task.Yield();

            var chain = _state.Blocks;
            return chain.Zip(chain.Skip(1),
                (prev, next) => next.PreviousHash == prev.Hash)
                .All(x => x);
        }

        
    }
}
