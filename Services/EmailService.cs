using FyaCreditApi.Configuration;
using FyaCreditApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FyaCreditApi.Services;

public class EmailService : IEmailService
{
    private readonly AppDbContext _context;
    private readonly SendGridSettings _settings;

    public EmailService(
        AppDbContext context,
        IOptions<SendGridSettings> settings)
    {
        _context = context;
        _settings = settings.Value;
    }

    public async Task SendCreditCreatedEmailAsync(int creditId)
    {
        var credit = await _context.Credits
            .FirstOrDefaultAsync(c => c.Id == creditId);

        if (credit is null)
        {
            throw new InvalidOperationException(
                $"Credit with ID {creditId} was not found.");
        }

        var client = new SendGridClient(_settings.ApiKey);

        var from = new EmailAddress(
            _settings.FromEmail,
            _settings.FromName);

        var to = new EmailAddress("fyasocialcapital@gmail.com");

        const string subject = "Nuevo crédito registrado";

        var body = $"""
                    Se ha registrado un nuevo crédito.

                    Cliente: {credit.ClientName}
                    Valor del crédito: ${credit.Amount:N0}
                    Comercial: {credit.Salesperson}
                    Fecha de registro: {credit.CreatedAt:dd/MM/yyyy HH:mm}
                    """;

        var message = MailHelper.CreateSingleEmail(
            from,
            to,
            subject,
            body,
            htmlContent: null);

        var response = await client.SendEmailAsync(message);

        var statusCode = (int)response.StatusCode;

        if (statusCode < 200 || statusCode >= 300)
        {
            throw new InvalidOperationException(
                $"SendGrid failed to send the email. Status code: {statusCode}");
        }
    }
}