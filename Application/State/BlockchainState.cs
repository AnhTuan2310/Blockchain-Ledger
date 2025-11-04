using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.State {
    /// <summary>
    /// BlockchainState: in-memory state container for the ledger.
    /// - Singleton lifetime (register as AddSingleton<BlockchainState>())
    /// - Basic thread-safety with lock (TS1)
    /// - Supports: blocks (active chain) and pending transactions (mempool)
    /// - Provides helper methods to mutate state in a safe way.
    /// 
    /// NOTE:
    /// - This class does NOT persist to DB. Persistence responsibilities stay in Repository.
    /// - Call InitializeAsync(...) at app startup to load from DB or create Genesis (INIT1).
    /// </summary>
    public class BlockchainState {
        private readonly List<Block> _blocks = new();
        private readonly List<Transaction> _mempool = new();

        private readonly object _lock = new();

        // Public read-only views (thread-safe snapshots)
        public IReadOnlyList<Block> Blocks {
            get {
                lock (_lock) {
                    return _blocks.Select(CloneBlockShallow).ToList();
                    //Chỉ trả về bản Clone
                }
            }
        }

        public IReadOnlyList<Transaction> PendingTransactions {
            get {
                lock (_lock) {
                    return _mempool.Select(t => CloneTransactionShallow(t)).ToList();
                    //Tương tự Block, Transaction cũng trả về clone
                }
            }
        }

        public async Task InitializeAsync(IEnumerable<Block>? loadedBlocks) {
            await Task.Yield();

            lock (_lock) {
                _blocks.Clear();
                _mempool.Clear();

                if (loadedBlocks != null && loadedBlocks.Any()) {
                    foreach (var b in loadedBlocks) //Load chain từ db
                        _blocks.Add(CloneBlockDeep(b));
                } else {
                    var genesis = CreateGenesisBlock();//Chưa có thì tạo mới 1 genesis
                    _blocks.Add(genesis);
                }
            }
        }

        //Thêm Transaction vào mem pool
        public void AddPendingTransaction(Transaction tx) {
            if (tx == null) throw new ArgumentNullException(nameof(tx));
            lock (_lock) {
                if (_mempool.Any(t => t.Id == tx.Id)) return;
                _mempool.Add(CloneTransactionShallow(tx));
            }
        }
        public List<Transaction> DrainPendingTransactions() {
            lock (_lock) {
                var drained = _mempool.Select(CloneTransactionShallow).ToList();
                _mempool.Clear();
                return drained;
            }
        }

        public void AddBlock(Block block) {
            if (block == null) throw new ArgumentNullException(nameof(block));
            lock (_lock) {
                _blocks.Add(CloneBlockDeep(block));
            }
        }

        //Thay thế Chain (rollback)
        public void ReplaceChain(IEnumerable<Block> newChain) {
            if (newChain == null) throw new ArgumentNullException(nameof(newChain));
            lock (_lock) {
                _blocks.Clear();
                foreach (var b in newChain)
                    _blocks.Add(CloneBlockDeep(b));
            }
        }

        //Clear pool
        public void ClearChainAndMempool() {
            lock (_lock) {
                _blocks.Clear();
                _mempool.Clear();
            }
        }

        private Block CreateGenesisBlock() {
            var genesisTransactions = new List<Transaction>();

            genesisTransactions.Add(MakeSystemTx("SYSTEM", "MEMORY", "System memory core initialized", "INIT"));
            genesisTransactions.Add(MakeSystemTx("SYSTEM", "MEMORY", "Bootstrap sequence completed", "BOOT"));
            genesisTransactions.Add(MakeSystemTx("SYSTEM", "MEMORY", "Consensus subsystem online", "CONSENSUS"));

            var genesis = new Block {
                PreviousHash = "000",
                Timestamp = DateTime.UtcNow,
                Transactions = genesisTransactions,
                MinerNote = "Genesis block (system initialization)"
            };

            genesis.Hash = ComputeHashForBlock(genesis);

            return genesis;
        }

        private static Transaction MakeSystemTx(string from, string to, string note, string tag) {
            //Dùng Transac.Content để giữ lại cấu trúc cho JSON content
            var payload = new {
                from,
                to,
                note,
                tag,
                createdAt = DateTime.UtcNow
            };

            return new Transaction {
                Id = Guid.NewGuid(),
                Author = "SYSTEM",
                Timestamp = DateTime.UtcNow,
                Content = JsonSerializer.Serialize(payload)
            };
        }

        //Hash block
        public static string ComputeHashForBlock(Block block) {
            if (block == null) throw new ArgumentNullException(nameof(block));

            var sb = new StringBuilder();
            sb.Append(block.PreviousHash ?? "");
            sb.Append("|");
            sb.Append(block.Timestamp.ToString("o"));
            sb.Append("|");
            if (block.Transactions != null && block.Transactions.Count > 0) {
                foreach (var tx in block.Transactions) {
                    sb.Append(tx.Id);
                    sb.Append(":");
                    sb.Append(tx.Content?.GetHashCode());
                    sb.Append(";");
                }
            }
            sb.Append("|");
            sb.Append(block.MinerNote ?? "");

            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        private static Block CloneBlockDeep(Block src) { //Clone
            return new Block {
                Hash = src.Hash,
                PreviousHash = src.PreviousHash,
                Timestamp = src.Timestamp,
                MinerNote = src.MinerNote,
                Transactions = src.Transactions?.Select(CloneTransactionShallow).ToList() ?? new List<Transaction>()
            };
        }
        private static Block CloneBlockShallow(Block src) {
            return CloneBlockDeep(src);
        }
        private static Transaction CloneTransactionShallow(Transaction t) {
            return new Transaction {
                Id = t.Id,
                Content = t.Content,
                Timestamp = t.Timestamp,
                Author = t.Author
            };
        }
    }
}
