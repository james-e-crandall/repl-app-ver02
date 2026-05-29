using FamilyTreeLib.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyTreeLib.Data;

public partial class FamilyTreeContext : DbContext
{
    public FamilyTreeContext(DbContextOptions<FamilyTreeContext> options) : base(options)
    {
    }

    public DbSet<Halfling> Halflings { get; set; }
    public DbSet<Orc> Orcs { get; set; }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Halfling>().HasNoKey();
        // modelBuilder.Entity<Orc>().HasNoKey();

        // modelBuilder.Entity<Halfling>().HasKey(h => h.Id);
        // modelBuilder.Entity<Orc>().HasKey(h => h.Id);
    }
}