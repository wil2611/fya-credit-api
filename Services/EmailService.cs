using FyaCreditApi.Configuration;
using FyaCreditApi.Data;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FyaCreditApi.Services;

public class EmailService : IEmailService
{
    private readonly AppDbContext _context;
    private readonly EmailSettings _settings;

    public EmailService(
        AppDbContext context,
        IOptions<EmailSettings> settings)
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
            return;
        }

        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(_settings.FromName, _settings.FromEmail)
        );

        message.To.Add(
            MailboxAddress.Parse("williamperezdiaz26@gmail.com")
        );

        message.Subject = "Nuevo crédito registrado";

        message.Body = new TextPart("plain")
        {
            Text =
                $"Se ha registrado un nuevo crédito.\n\n" +
                $"Cliente: {credit.ClientName}\n" +
                $"Valor del crédito: ${credit.Amount:N0}\n" +
                $"Comercial: {credit.Salesperson}\n" +
                $"Fecha de registro: {credit.CreatedAt:dd/MM/yyyy HH:mm}"
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _settings.Host,
            _settings.Port,
            SecureSocketOptions.StartTls
        );

        await client.AuthenticateAsync(
            _settings.Username,
            _settings.Password
        );

        await client.SendAsync(message);

        await client.DisconnectAsync(true);
    }
}