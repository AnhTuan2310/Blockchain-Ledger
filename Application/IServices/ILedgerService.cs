using Application.DTOs.Ledger;
using Domain.Entities;

namespace Application.IServices {
    public interface ILedgerService {
        Task AddTransactionAsync(Transaction tran);
        Task<Block> MineBlockAsync(string minerNote);
        Task<SnapshotDTO> CreateSnapshotAsync(string description);
        Task RollbackToSnapshotAsync(Guid snapId);
        Task<List<Block>> GetCurrentChainAsync();

    }
}
