using FyaCreditApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace FyaCreditApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Credit> Credits { get; set; }
}