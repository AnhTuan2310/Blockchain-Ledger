namespace Application.DTOs.Blocks;

public record BlockSummaryDTO(
    string Hash,
    string PreviousHash,
    DateTime Timestamp,
    int TransactionCount,
    string MinerNote
);
