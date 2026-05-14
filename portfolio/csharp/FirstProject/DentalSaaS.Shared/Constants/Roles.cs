namespace DentalSaaS.Shared.Constants;

public static class Roles
{
    public const string SystemAdmin = "SystemAdmin";
    public const string SystemSupport = "SystemSupport";
    public const string SystemBilling = "SystemBilling";

    public const string CompanyOwner = "CompanyOwner";
    public const string CompanyAdmin = "CompanyAdmin";
    public const string CompanyManager = "CompanyManager";
    public const string CompanyEmployee = "CompanyEmployee";

    public static readonly string[] SystemRoles =
    [
        SystemAdmin,
        SystemSupport,
        SystemBilling
    ];

    public static readonly string[] CompanyRoles =
    [
        CompanyOwner,
        CompanyAdmin,
        CompanyManager,
        CompanyEmployee
    ];
}
