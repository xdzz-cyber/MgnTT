using MagniseTestTask.Models;
using Microsoft.EntityFrameworkCore;

namespace MagniseTestTask.Database;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _config;

    public ApplicationDbContext(IConfiguration config)
    {
        _config = config;
    }
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection")!);
    }

    public DbSet<CryptoCurrency> CryptoCurrencies { get; set; } = null!;
}