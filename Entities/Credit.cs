namespace FyaCreditApi.Entities;

public class Credit
{
    public int Id { get; set; }

    public string ClientName { get; set; } = string.Empty;

    public string ClientDocument { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public decimal InterestRate { get; set; }

    public int TermMonths { get; set; }

    public string Salesperson { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}