namespace FyaCreditApi.Services;

public interface IEmailService
{
    Task SendCreditCreatedEmailAsync(int creditId);
}