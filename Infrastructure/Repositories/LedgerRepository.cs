using Application.DTOs;
using Application.DTOs.Ledger;
using Application.IRepositories;
using Application.IServices;
using Application.Mappings;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Repositories {
    public class LedgerRepository : ILedgerRepository {
        private readonly AppDbContext _db;

        public LedgerRepository(AppDbContext db) {
            _db = db;
        }

        //Lay chuoi Chain hien tai
        public async Task<List<Block>> GetCurrentChainAsync() {
            return await _db.Blocks
                .Include(b => b.Transactions)
                .OrderBy(b => b.Timestamp)
                .ToListAsync();
        }
        public async Task AddBlockAsync(Block block) {
            if (block == null) throw new ArgumentNullException(nameof(block));

            await _db.Blocks.AddAsync(block);
            await _db.SaveChangesAsync();
        }

        //Snapshot

        // Tao snapshot va return SnapshotDTO
        public async Task<SnapshotDTO> CreateSnapshotAsync(string description, List<Block> chain, List<Transaction> mempool) {
            var snapId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;

            var dto = new SnapshotDTO {
                SnapshotId = snapId,
                Description = description ?? string.Empty,
                CreatedAt = createdAt,
                Chain = chain.Select(b => b.ToDTO()).ToList(),
                Mempool = mempool.Select(t => t.ToDTO()).ToList()
            };

            var json = JsonSerializer.Serialize(dto);

            var record = new MemoryRecord {
                Id = snapId,
                Description = description,
                SnapshotTime = createdAt,
                ChainStateJson = json
            };

            await _db.MemoryRecords.AddAsync(record);
            await _db.SaveChangesAsync();

            return dto;
        }

        //Get snapshot theo Id
        public async Task<SnapshotDTO?> GetSnapshotAsync(Guid snapshotId) {
            var snap = await _db.MemoryRecords.FirstOrDefaultAsync(x => x.Id == snapshotId);
            if (snap == null) return null;

            var dto = JsonSerializer.Deserialize<SnapshotDTO>(snap.ChainStateJson);
            return dto;
        }

        //Thay the trang thai cua db bang snapshot
        public async Task RollbackToSnapshotAsync(SnapshotDTO snapshotDto) {
            if (snapshotDto == null) throw new ArgumentNullException(nameof(snapshotDto));

            // Convert DTO sang Domain
            var (chain, mempool) = snapshotDto.ToDomain();

            _db.Transactions.RemoveRange(_db.Transactions);
            _db.Blocks.RemoveRange(_db.Blocks);
            await _db.SaveChangesAsync();

            if (chain.Any()) {
                await _db.Blocks.AddRangeAsync(chain);
            }

            if (mempool.Any()) {
                await _db.Transactions.AddRangeAsync(mempool);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<List<SnapshotDTO>> GetSnapshotsAsync() {
            var snaps = await _db.MemoryRecords
                .OrderByDescending(m => m.SnapshotTime)
                .ToListAsync();

            return snaps
                .Select(s => JsonSerializer.Deserialize<SnapshotDTO>(s.ChainStateJson)!)
                .Where(x => x != null)
                .ToList();
        }
    }
}
