using Microsoft.EntityFrameworkCore;
using OperationsKnowledge.Models;

namespace OperationsKnowledge.Data;

public class OperationalSystemContext : DbContext
{
    public DbSet<OperationalSystem> OperationalSystems { get; set; } = null!;

    public OperationalSystemContext(
        DbContextOptions<OperationalSystemContext> options)
        : base(options)
    {
    }
}

/* q's
 * What calls the default constructor?
 * How does it know what to pass to it (DbContextOptions).
 * */
