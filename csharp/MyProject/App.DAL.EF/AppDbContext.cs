using System.Text.Json;
using App.Domain;
using App.Domain.Identity;
using Base.Domain;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>, IDataProtectionKeyContext
{
    private static readonly ValueComparer<LangStr> LangStrValueComparer = new(
        (left, right) => SerializeLangStr(left) == SerializeLangStr(right),
        value => SerializeLangStr(value).GetHashCode(),
        value => DeserializeLangStr(SerializeLangStr(value)));

    public DbSet<ListItem> ListItems { get; set; }

    public DbSet<Portfolio> Portfolios { get; set; } = default!;
    public DbSet<Asset> Assets { get; set; } = default!;
    public DbSet<Transaction> Transactions { get; set; } = default!;
    public DbSet<TransactionFee> TransactionFees { get; set; } = default!;
    public DbSet<PriceSnapshot> PriceSnapshots { get; set; } = default!;
    public DbSet<PositionSnapshot> PositionSnapshots { get; set; } = default!;
    public DbSet<Watchlist> Watchlists { get; set; } = default!;
    public DbSet<WatchlistItem> WatchlistItems { get; set; } = default!;
    public DbSet<Tag> Tags { get; set; } = default!;
    public DbSet<AssetTag> AssetTags { get; set; } = default!;
    public DbSet<Note> Notes { get; set; } = default!;
    public DbSet<Currency> Currencies { get; set; } = default!;
    public DbSet<Exchange> Exchanges { get; set; } = default!;
    public DbSet<AssetType> AssetTypes { get; set; } = default!;
    public DbSet<MarketDataProvider> MarketDataProviders { get; set; } = default!;

    public DbSet<AppRefreshToken> RefreshTokens { get; set; } = default!;

    // This maps to the table that stores data protection keys.
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = default!;


    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // disable cascade delete
        foreach (var relationship in builder.Model
                     .GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        ConfigureLangStr(builder.Entity<ListItem>().Property(e => e.Summary));
        ConfigureLangStr(builder.Entity<Currency>().Property(e => e.DisplayName));
        ConfigureLangStr(builder.Entity<Exchange>().Property(e => e.DisplayName));
        ConfigureLangStr(builder.Entity<AssetType>().Property(e => e.DisplayName));
        ConfigureLangStr(builder.Entity<MarketDataProvider>().Property(e => e.DisplayName));

        builder.Entity<Transaction>()
            .Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Entity<Portfolio>()
            .HasIndex(e => new { e.AppUserId, e.Name });

        builder.Entity<Asset>()
            .HasIndex(e => new { e.PortfolioId, e.Name });

        builder.Entity<PriceSnapshot>()
            .HasIndex(e => new { e.AssetId, e.RecordedAt });

        builder.Entity<Transaction>()
            .HasIndex(e => new { e.PortfolioId, e.ExecutedAt });
    }

    private static void ConfigureLangStr(Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<LangStr> propertyBuilder)
    {
        propertyBuilder
            .HasConversion(
                v => SerializeLangStr(v),
                v => DeserializeLangStr(v)
            )
            .HasColumnType("jsonb");

        propertyBuilder.Metadata.SetValueComparer(LangStrValueComparer);
    }

    private static string SerializeLangStr(LangStr? value)
    {
        return JsonSerializer.Serialize(value ?? new LangStr(), (JsonSerializerOptions?)null);
    }

    private static LangStr DeserializeLangStr(string value)
    {
        return JsonSerializer.Deserialize<LangStr>(value, (JsonSerializerOptions?)null) ?? new LangStr();
    }
}
