using DentalSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DentalSaaS.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    private readonly Guid _companyId;

    public AppDbContext(DbContextOptions<AppDbContext> options, Guid companyId) : base(options)
    {
        _companyId = companyId;
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<TreatmentPlan> TreatmentPlans => Set<TreatmentPlan>();
    public DbSet<PlanItem> PlanItems => Set<PlanItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted && p.CompanyId == _companyId);
        modelBuilder.Entity<TreatmentPlan>().HasQueryFilter(p => !p.IsDeleted && p.CompanyId == _companyId);
        modelBuilder.Entity<PlanItem>().HasQueryFilter(p => !p.IsDeleted && p.CompanyId == _companyId);
    }
}
