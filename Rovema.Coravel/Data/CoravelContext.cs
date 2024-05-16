using Coravel.Pro.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Rovema.Coravel.Data;

public class CoravelContext(DbContextOptions<CoravelContext> options) : DbContext(options), ICoravelProDbContext
{
    public DbSet<CoravelJobHistory> Coravel_JobHistory { get; set; }
    public DbSet<CoravelScheduledJob> Coravel_ScheduledJobs { get; set; }
    public DbSet<CoravelScheduledJobHistory> Coravel_ScheduledJobHistory { get; set; }
}
