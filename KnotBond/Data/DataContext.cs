using KnotBond.Entities;
using Microsoft.EntityFrameworkCore;

namespace KnotBond.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; }
}
