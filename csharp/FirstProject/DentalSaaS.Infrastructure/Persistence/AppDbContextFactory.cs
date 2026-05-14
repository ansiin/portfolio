using DentalSaaS.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DentalSaaS.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var webProjectPath = Path.GetFullPath(Path.Combine(basePath, "..", "DentalSaaS.Web"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.Exists(webProjectPath) ? webProjectPath : basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? "Host=localhost;Port=5432;Database=dentalsaas;Username=dentalsaas;Password=devpassword";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        ICurrentTenantAccessor tenantAccessor = new DesignTimeTenantAccessor();
        return new AppDbContext(optionsBuilder.Options, tenantAccessor);
    }
}
