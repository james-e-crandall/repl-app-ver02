using FamilyTreeLib.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyTreeLib.Data;

public partial class FamilyTreeContext : DbContext
{
    public FamilyTreeContext(DbContextOptions<FamilyTreeContext> options) : base(options)
    {
    }

    public DbSet<Halfling> Halflings { get; set; }
}