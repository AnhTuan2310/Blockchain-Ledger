using Application.DTOs.Snapshot;
using Domain.Entities;

namespace Application.IServices {
    public interface ILedgerService {
        Task AddTransactionAsync(Transaction transaction);
        Task<Block> MineBlockAsync(string minerNote);         

        // Snapshots (đang có sẵn)
        Task<SnapshotDTO> CreateSnapshotAsync(string description);
        Task RollbackToSnapshotAsync(Guid snapshotId);
        Task<Guid> ForkFromSnapshotAsync(Guid snapshotId, string newChainName);
    }
}
