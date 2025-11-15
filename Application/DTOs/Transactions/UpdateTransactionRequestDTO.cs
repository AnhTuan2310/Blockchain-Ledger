using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Transactions;

public class UpdateTransactionRequestDTO : CreateTransactionRequestDTO, IValidatableObject {
    [Required]
    public Guid Id { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext _) {
        if (Id == Guid.Empty)
            yield return new ValidationResult("Id must be a non-empty.", new[] { nameof(Id) });
    }
}
