using System.ComponentModel.DataAnnotations;

namespace FyaCreditApi.DTOs;

public class CreateCreditRequest
{
    [Required]
    [MaxLength(120)]
    public string ClientName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string ClientDocument { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Range(0, 100)]
    public decimal InterestRate { get; set; }

    [Range(1, 600)]
    public int TermMonths { get; set; }

    [Required]
    [MaxLength(120)]
    public string Salesperson { get; set; } = string.Empty;
}