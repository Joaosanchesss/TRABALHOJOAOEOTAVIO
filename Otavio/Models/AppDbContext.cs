using Microsoft.EntityFrameworkCore;

namespace Otavio.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Folha> Folhas { get; set; }
}
