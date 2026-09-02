using System.ComponentModel.DataAnnotations;

namespace FyaCreditApi.DTOs;

public class CreateCreditRequest
{
    [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
    [MaxLength(120, ErrorMessage = "El nombre no puede superar 120 caracteres.")]
    [RegularExpression(
        @"^[\p{L}.' -]+$",
        ErrorMessage = "El nombre solo puede contener letras, espacios, guiones y apóstrofes."
    )]
    public string ClientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cédula o ID es obligatoria.")]
    [MaxLength(30, ErrorMessage = "El documento no puede superar 30 caracteres.")]
    [RegularExpression(
        @"^\d+$",
        ErrorMessage = "La cédula o ID debe contener únicamente números."
    )]
    public string ClientDocument { get; set; } = string.Empty;

    [Range(
        0.01,
        double.MaxValue,
        ErrorMessage = "El valor del crédito debe ser mayor a 0."
    )]
    public decimal Amount { get; set; }

    [Range(
        0,
        100,
        ErrorMessage = "La tasa de interés debe estar entre 0 y 100."
    )]
    public decimal InterestRate { get; set; }

    [Range(
        1,
        600,
        ErrorMessage = "El plazo debe estar entre 1 y 600 meses."
    )]
    public int TermMonths { get; set; }

    [Required(ErrorMessage = "El nombre del comercial es obligatorio.")]
    [MaxLength(
        120,
        ErrorMessage = "El nombre del comercial no puede superar 120 caracteres."
    )]
    [RegularExpression(
        @"^[\p{L}.' -]+$",
        ErrorMessage = "El nombre del comercial solo puede contener letras, espacios, guiones y apóstrofes."
    )]
    public string Salesperson { get; set; } = string.Empty;
}