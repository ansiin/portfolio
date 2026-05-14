using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Reports;
using DentalSaaS.Infrastructure.Persistence;
using DentalSaaS.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace DentalSaaS.Infrastructure.Services;

public sealed class ReportsService : IReportsService
{
    private readonly AppDbContext _db;
    private readonly ICurrentTenantAccessor _tenant;

    public ReportsService(AppDbContext db, ICurrentTenantAccessor tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<ReportsDashboardDto> GetDashboardAsync(DateOnly? dateFrom = null, DateOnly? dateTo = null, CancellationToken ct = default)
    {
        EnsureTenant();

        var to = dateTo ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var from = dateFrom ?? to.AddDays(-29);
        if (from > to)
        {
            (from, to) = (to, from);
        }

        var fromAt = new DateTimeOffset(from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
        var toAt = new DateTimeOffset(to.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc));

        var appointments = (await _db.Appointments
                .AsNoTracking()
                .ToListAsync(ct))
            .Where(a => a.StartAt >= fromAt && a.StartAt <= toAt)
            .ToList();

        var treatments = (await _db.Treatments
                .AsNoTracking()
                .ToListAsync(ct))
            .Where(t => t.PerformedAt >= fromAt && t.PerformedAt <= toAt)
            .ToList();

        var plans = (await _db.TreatmentPlans
                .AsNoTracking()
                .ToListAsync(ct))
            .Where(p => p.CreatedAt >= fromAt && p.CreatedAt <= toAt)
            .ToList();

        var planItems = await _db.PlanItems.AsNoTracking().ToListAsync(ct);
        var estimates = await _db.CostEstimates.AsNoTracking().ToListAsync(ct);
        var invoices = await _db.Invoices.AsNoTracking().ToListAsync(ct);
        var paymentPlans = await _db.PaymentPlans.AsNoTracking().ToListAsync(ct);
        var rooms = await _db.TreatmentRooms.AsNoTracking().ToListAsync(ct);
        var xrays = await _db.Xrays.AsNoTracking().ToListAsync(ct);
        var patients = await _db.Patients.AsNoTracking().ToListAsync(ct);
        var settings = (await _db.CompanySettings
                .AsNoTracking()
                .ToListAsync(ct))
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefault();

        var activePatients = appointments.Select(a => a.PatientId)
            .Union(treatments.Select(t => t.PatientId))
            .Union(plans.Select(p => p.PatientId))
            .Distinct()
            .Count();

        var appointmentMinutes = appointments.Sum(a => Math.Max(0, (a.EndAt - a.StartAt).TotalMinutes));
        var periodDays = Math.Max(1, to.DayNumber - from.DayNumber + 1);
        var roomCount = Math.Max(1, rooms.Count);
        var availableMinutes = roomCount * periodDays * 8 * 60;
        var utilization = availableMinutes <= 0
            ? 0m
            : decimal.Round((decimal)(appointmentMinutes / availableMinutes) * 100m, 2);

        var acceptedItems = planItems.Count(i => i.DecisionStatus == PlanItemDecisionStatus.Accepted);
        var deferredItems = planItems.Count(i => i.DecisionStatus == PlanItemDecisionStatus.Deferred);
        var urgentItems = planItems.Where(i => i.Urgency >= 4).ToArray();
        var urgentAccepted = urgentItems.Count(i => i.DecisionStatus == PlanItemDecisionStatus.Accepted);
        var urgentRate = urgentItems.Length == 0
            ? 0m
            : decimal.Round((decimal)urgentAccepted / urgentItems.Length * 100m, 2);

        var submittedInRange = estimates
            .Where(e => e.SubmittedAt.HasValue && e.SubmittedAt.Value >= fromAt && e.SubmittedAt.Value <= toAt)
            .ToArray();

        var approved = submittedInRange.Count(e => e.SubmissionState == InsuranceSubmissionState.Accepted);
        var rejected = submittedInRange.Count(e => e.SubmissionState == InsuranceSubmissionState.Rejected);
        var approvalRate = (approved + rejected) == 0
            ? 0m
            : decimal.Round((decimal)approved / (approved + rejected) * 100m, 2);

        var outstanding = invoices
            .Where(i => !i.IsPaid)
            .Sum(i => i.Amount);

        var intervalDays = settings?.XrayIntervalDays > 0 ? settings.XrayIntervalDays : 180;
        var now = DateTimeOffset.UtcNow;
        var latestByPatient = xrays
            .GroupBy(x => x.PatientId)
            .ToDictionary(g => g.Key, g => g.MaxBy(x => x.TakenAt)!);

        var overdueXrayPatients = patients.Count(p =>
            latestByPatient.TryGetValue(p.Id, out var latest)
            && latest.TakenAt.AddDays(intervalDays) < now);

        var paymentPlanExposure = paymentPlans.Sum(p => p.MonthlyAmount * p.Months);

        return new ReportsDashboardDto(
            from,
            to,
            activePatients,
            appointments.Count,
            utilization,
            treatments.Count,
            acceptedItems,
            deferredItems,
            urgentRate,
            submittedInRange.Length,
            approvalRate,
            outstanding,
            overdueXrayPatients,
            paymentPlanExposure);
    }

    private void EnsureTenant()
    {
        if (!_tenant.Current.IsResolved)
        {
            throw new InvalidOperationException("Tenant is not resolved.");
        }
    }
}
