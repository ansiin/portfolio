using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Application.Onboarding;
using DentalSaaS.Application.Appointments;
using DentalSaaS.Application.Billing;
using DentalSaaS.Application.CompanySettings;
using DentalSaaS.Application.Insurance;
using DentalSaaS.Application.Patients;
using DentalSaaS.Application.PaymentPlans;
using DentalSaaS.Application.PracticeSetup;
using DentalSaaS.Application.Reports;
using DentalSaaS.Application.ToothRecords;
using DentalSaaS.Application.Treatments;
using DentalSaaS.Application.TreatmentPlans;
using DentalSaaS.Application.Xrays;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Infrastructure.Persistence;
using DentalSaaS.Infrastructure.Repositories;
using DentalSaaS.Infrastructure.Security;
using DentalSaaS.Infrastructure.Services;
using DentalSaaS.Infrastructure.Tenancy;
using DentalSaaS.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DentalSaaS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? "Host=localhost;Port=5432;Database=dentalsaas;Username=dentalsaas;Password=devpassword";

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null)));

        services
            .AddIdentity<AppUser, AppUserRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ICurrentTenantAccessor, CurrentTenantAccessor>();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddScoped<IIdentityAccountService, IdentityAccountService>();

        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyMembershipRepository, CompanyMembershipRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<ITreatmentPlanRepository, TreatmentPlanRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped(typeof(ICompanyCrudRepository<>), typeof(CompanyCrudRepository<>));

        services.AddScoped<IFeatureGateService, FeatureGateService>();
        services.AddScoped<ISystemAdministrationService, SystemAdministrationService>();
        services.AddScoped<IInsuranceSubmissionGateway, MockInsuranceSubmissionGateway>();
        services.AddScoped<IReportsService, ReportsService>();
        services.AddScoped<IRoleAuthorizationService, RoleAuthorizationService>();

        services.AddScoped<IOnboardingService, OnboardingService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<ITreatmentPlanService, TreatmentPlanService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IInsuranceService, InsuranceService>();
        services.AddScoped<IBillingService, BillingService>();
        services.AddScoped<IPaymentPlanService, PaymentPlanService>();
        services.AddScoped<IPracticeSetupService, PracticeSetupService>();
        services.AddScoped<ICompanySettingsService, CompanySettingsService>();
        services.AddScoped<IToothRecordService, ToothRecordService>();
        services.AddScoped<IXrayService, XrayService>();
        services.AddScoped<ITreatmentService, TreatmentService>();

        services.AddScoped<IAuthorizationHandler, TenantResolvedAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, TenantRoleAuthorizationHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.TenantResolved, policy => policy.Requirements.Add(new TenantResolvedRequirement()));
            options.AddPolicy(Policies.TenantOwner, policy => policy.Requirements.Add(new TenantRoleRequirement(Roles.CompanyOwner)));
            options.AddPolicy(Policies.TenantOwnerOrAdmin, policy =>
                policy.Requirements.Add(new TenantRoleRequirement(Roles.CompanyOwner, Roles.CompanyAdmin)));
            options.AddPolicy(Policies.TenantLeadership, policy =>
                policy.Requirements.Add(new TenantRoleRequirement(Roles.CompanyOwner, Roles.CompanyAdmin, Roles.CompanyManager)));
            options.AddPolicy(Policies.TenantStaff, policy =>
                policy.Requirements.Add(new TenantRoleRequirement(
                    Roles.CompanyOwner,
                    Roles.CompanyAdmin,
                    Roles.CompanyManager,
                    Roles.CompanyEmployee)));
            options.AddPolicy(Policies.SystemOperator, policy =>
                policy.RequireRole(Roles.SystemAdmin, Roles.SystemSupport, Roles.SystemBilling));
            options.AddPolicy(Policies.SystemAdminOnly, policy => policy.RequireRole(Roles.SystemAdmin));
            options.AddPolicy(Policies.Impersonation, policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.IsInRole(Roles.SystemAdmin) ||
                    string.Equals(ctx.User.FindFirst("impersonating")?.Value, "true", StringComparison.OrdinalIgnoreCase)));
        });

        return services;
    }
}
