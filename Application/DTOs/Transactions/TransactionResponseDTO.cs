namespace Application.DTOs.Transactions;

public class TransactionResponseDTO {
    public Guid Id { get; set; }
    public string Content { get; set; } = "";
    public string Author { get; set; } = "System";
    public DateTime Timestamp { get; set; }
}
