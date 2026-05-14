using System.Linq.Expressions;
using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Common;
using DentalSaaS.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DentalSaaS.Infrastructure.Persistence;

public sealed class AppDbContext : IdentityDbContext<AppUser, AppUserRole, Guid>
{
    private readonly ICurrentTenantAccessor _tenant;
    private Guid? CurrentCompanyId => _tenant.Current.IsResolved ? _tenant.Current.CompanyId : null;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenantAccessor tenant)
        : base(options)
    {
        _tenant = tenant;
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanySettings> CompanySettings => Set<CompanySettings>();
    public DbSet<CompanyMembership> CompanyMemberships => Set<CompanyMembership>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<TreatmentPlan> TreatmentPlans => Set<TreatmentPlan>();
    public DbSet<PlanItem> PlanItems => Set<PlanItem>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<TreatmentType> TreatmentTypes => Set<TreatmentType>();
    public DbSet<TreatmentRoom> TreatmentRooms => Set<TreatmentRoom>();
    public DbSet<Dentist> Dentists => Set<Dentist>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ToothRecord> ToothRecords => Set<ToothRecord>();
    public DbSet<Xray> Xrays => Set<Xray>();
    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<InsurancePlan> InsurancePlans => Set<InsurancePlan>();
    public DbSet<CostEstimate> CostEstimates => Set<CostEstimate>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PaymentPlan> PaymentPlans => Set<PaymentPlan>();
    public DbSet<ImpersonationSession> ImpersonationSessions => Set<ImpersonationSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>().Property(u => u.DisplayName).HasMaxLength(120);

        modelBuilder.Entity<Company>()
            .HasIndex(c => c.Slug)
            .IsUnique();

        modelBuilder.Entity<CompanyMembership>()
            .HasIndex(m => new { m.CompanyId, m.UserId })
            .IsUnique();

        modelBuilder.Entity<TreatmentPlan>()
            .HasMany(tp => tp.Items)
            .WithOne()
            .HasForeignKey(i => i.TreatmentPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ToothRecord>()
            .HasIndex(t => new { t.CompanyId, t.PatientId, t.ToothNumber })
            .IsUnique();

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => a.PlanItemId);

        modelBuilder.Entity<Appointment>()
            .HasOne<PlanItem>()
            .WithMany()
            .HasForeignKey(a => a.PlanItemId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CostEstimate>()
            .HasIndex(e => e.SubmissionState);

        ApplyCompanyFilters(modelBuilder);
    }

    private void ApplyCompanyFilters(ModelBuilder modelBuilder)
    {
        ApplyCompanyFilter<CompanySettings>(modelBuilder);
        ApplyCompanyFilter<Patient>(modelBuilder);
        ApplyCompanyFilter<TreatmentPlan>(modelBuilder);
        ApplyCompanyFilter<PlanItem>(modelBuilder);
        ApplyCompanyFilter<AuditLog>(modelBuilder);
        ApplyCompanyFilter<Subscription>(modelBuilder);
        ApplyCompanyFilter<TreatmentType>(modelBuilder);
        ApplyCompanyFilter<TreatmentRoom>(modelBuilder);
        ApplyCompanyFilter<Dentist>(modelBuilder);
        ApplyCompanyFilter<Appointment>(modelBuilder);
        ApplyCompanyFilter<ToothRecord>(modelBuilder);
        ApplyCompanyFilter<Xray>(modelBuilder);
        ApplyCompanyFilter<Treatment>(modelBuilder);
        ApplyCompanyFilter<InsurancePlan>(modelBuilder);
        ApplyCompanyFilter<CostEstimate>(modelBuilder);
        ApplyCompanyFilter<Invoice>(modelBuilder);
        ApplyCompanyFilter<PaymentPlan>(modelBuilder);
    }

    private void ApplyCompanyFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : CompanyEntityBase
    {
        Expression<Func<TEntity, bool>> filter = entity =>
            !entity.IsDeleted &&
            (CurrentCompanyId == null || (Guid?)entity.CompanyId == CurrentCompanyId);

        modelBuilder.Entity<TEntity>().HasQueryFilter(filter);
    }
}
