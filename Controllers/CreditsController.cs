using FyaCreditApi.Data;
using FyaCreditApi.DTOs;
using FyaCreditApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FyaCreditApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CreditsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Credit>> CreateCredit(
        CreateCreditRequest request)
    {
        var credit = new Credit
        {
            ClientName = request.ClientName,
            ClientDocument = request.ClientDocument,
            Amount = request.Amount,
            InterestRate = request.InterestRate,
            TermMonths = request.TermMonths,
            Salesperson = request.Salesperson,
            CreatedAt = DateTime.UtcNow
        };

        _context.Credits.Add(credit);

        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, credit);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CreditResponse>>> GetCredits(
    [FromQuery] string? clientName,
    [FromQuery] string? clientDocument,
    [FromQuery] string? salesperson,
    [FromQuery] string? sortBy,
    [FromQuery] string? sortOrder = "desc")
    {
        var query = _context.Credits.AsQueryable();

        if (!string.IsNullOrWhiteSpace(clientName))
        {
            query = query.Where(c =>
                EF.Functions.ILike(c.ClientName, $"%{clientName}%"));
        }

        if (!string.IsNullOrWhiteSpace(clientDocument))
        {
            query = query.Where(c =>
                c.ClientDocument.Contains(clientDocument));
        }

        if (!string.IsNullOrWhiteSpace(salesperson))
        {
            query = query.Where(c =>
                EF.Functions.ILike(c.Salesperson, $"%{salesperson}%"));
        }

        var descending = sortOrder?.ToLower() != "asc";

        query = sortBy?.ToLower() switch
        {
            "amount" => descending
                ? query.OrderByDescending(c => c.Amount)
                : query.OrderBy(c => c.Amount),

            "createdat" => descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt),

            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        var credits = await query
            .Select(c => new CreditResponse
            {
                Id = c.Id,
                ClientName = c.ClientName,
                ClientDocument = c.ClientDocument,
                Amount = c.Amount,
                InterestRate = c.InterestRate,
                TermMonths = c.TermMonths,
                Salesperson = c.Salesperson,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(credits);
    }
}