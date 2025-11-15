using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Blocks;

public class BlockQueryRequestDTO : IValidatableObject {
    [Range(1, 1000)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [MaxLength(100)] public string? Keyword { get; set; }
    [MaxLength(100)] public string? Author { get; set; }
    [MaxLength(200)] public string? MinerNote { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    [Range(0, int.MaxValue)] public int? MinTransactionCount { get; set; }
    [Range(0, int.MaxValue)] public int? MaxTransactionCount { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext _) {
        if (FromDate.HasValue && ToDate.HasValue && FromDate > ToDate)
            yield return new ValidationResult("FromDate must be ≤ ToDate",
                new[] { nameof(FromDate), nameof(ToDate) });

        if (MinTransactionCount.HasValue && MaxTransactionCount.HasValue &&
            MinTransactionCount > MaxTransactionCount)
            yield return new ValidationResult("MinTransactionCount must be ≤ MaxTransactionCount",
                new[] { nameof(MinTransactionCount), nameof(MaxTransactionCount) });
    }
}
