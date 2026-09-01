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
    public async Task<ActionResult<IEnumerable<CreditResponse>>> GetCredits()
    {
        var credits = await _context.Credits
            .OrderByDescending(c => c.CreatedAt)
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