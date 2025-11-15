using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Transactions;

public class CreateTransactionRequestDTO {
    [Required]
    [StringLength(500, MinimumLength = 3, ErrorMessage ="Content must in 3 and 500")]
    public string Content { get; set; } = "";

    [Required]
    [StringLength(100, MinimumLength = 2, ErrorMessage ="Author too long")]
    public string Author { get; set; } = "";
}
