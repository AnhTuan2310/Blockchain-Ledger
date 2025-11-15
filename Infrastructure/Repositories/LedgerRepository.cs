using Application.DTOs.Snapshot;
using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Repositories {
    public class LedgerRepository : ILedgerRepository {
        private readonly AppDbContext _context;

        public LedgerRepository(AppDbContext context) {
            _context = context;
        }

        public async Task AddBlockAsync(Block block) {
            await _context.Blocks.AddAsync(block);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Block>> GetCurrentChainAsync() {
            return await _context.Blocks
                .Include(b => b.Transactions)
                .OrderBy(b => b.Timestamp)
                .ToListAsync();
        }

        // ================================
        // SNAPSHOT SUPPORT
        // ================================

        public async Task<SnapshotDTO> CreateSnapshotAsync(string description, List<Block> chain, List<Transaction> mempool) {
            var record = new MemoryRecord {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Description = description,
                ChainStateJson = JsonSerializer.Serialize(chain),
                PendingTransactionsJson = JsonSerializer.Serialize(mempool)
            };

            await _context.MemoryRecords.AddAsync(record);
            await _context.SaveChangesAsync();

            return new SnapshotDTO {
                Id = record.Id,
                Description = record.Description,
                CreatedAt = record.CreatedAt
            };
        }

        public async Task<MemoryRecord?> GetSnapshotAsync(Guid id) {
            return await _context.MemoryRecords.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task RollbackToSnapshotAsync(MemoryRecord record) {
            // Clear DB tables
            _context.Blocks.RemoveRange(_context.Blocks);
            _context.Transactions.RemoveRange(_context.Transactions);

            // Restore data
            var restoredChain = JsonSerializer.Deserialize<List<Block>>(record.ChainStateJson) ?? new();
            var restoredMempool = JsonSerializer.Deserialize<List<Transaction>>(record.PendingTransactionsJson) ?? new();

            await _context.Blocks.AddRangeAsync(restoredChain);
            await _context.Transactions.AddRangeAsync(restoredMempool);

            await _context.SaveChangesAsync();
        }
    }
}
