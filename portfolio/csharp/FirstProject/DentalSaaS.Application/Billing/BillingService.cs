using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Billing;

public sealed class BillingService : IBillingService
{
    private readonly ICompanyCrudRepository<Invoice> _invoices;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;
    private readonly IRoleAuthorizationService _authorization;

    public BillingService(
        ICompanyCrudRepository<Invoice> invoices,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user,
        IRoleAuthorizationService authorization)
    {
        _invoices = invoices;
        _tenant = tenant;
        _user = user;
        _authorization = authorization;
    }

    public async Task<IReadOnlyCollection<InvoiceDto>> ListAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanViewOperationalData().IsFailure)
        {
            return [];
        }

        var invoices = await _invoices.ListAsync(_tenant.Current.CompanyId, ct);
        return invoices
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InvoiceDto(i.Id, i.PatientId, i.Amount, i.IsPaid))
            .ToArray();
    }

    public async Task<Result<Guid>> CreateAsync(CreateInvoiceRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanManageFinancialData();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }

        var invoice = new Invoice
        {
            CompanyId = _tenant.Current.CompanyId,
            PatientId = request.PatientId,
            Amount = request.Amount,
            IsPaid = false,
            CreatedBy = _user.Current.UserId
        };

        await _invoices.AddAsync(invoice, ct);
        return Result<Guid>.Success(invoice.Id);
    }

    public async Task<Result> MarkPaidAsync(Guid invoiceId, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanManageFinancialData();
        if (permission.IsFailure)
        {
            return permission;
        }

        var invoice = await _invoices.GetAsync(_tenant.Current.CompanyId, invoiceId, ct);
        if (invoice is null)
        {
            return Result.Failure("Invoice not found.");
        }

        invoice.IsPaid = true;
        invoice.UpdatedAt = DateTimeOffset.UtcNow;
        invoice.UpdatedBy = _user.Current.UserId;
        await _invoices.UpdateAsync(invoice, ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid invoiceId, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanManageFinancialData();
        if (permission.IsFailure)
        {
            return permission;
        }

        var invoice = await _invoices.GetAsync(_tenant.Current.CompanyId, invoiceId, ct);
        if (invoice is null)
        {
            return Result.Failure("Invoice not found.");
        }

        await _invoices.SoftDeleteAsync(invoice, _user.Current.UserId, ct);
        return Result.Success();
    }

    private void EnsureTenant()
    {
        if (!_tenant.Current.IsResolved)
        {
            throw new InvalidOperationException("Tenant is not resolved.");
        }
    }
}
