using DAL.Entities;
using DAL.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser>(options)
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetTag> AssetTags => Set<AssetTag>();
    public DbSet<AssetType> AssetTypes => Set<AssetType>();
    public DbSet<CorporateAction> CorporateActions => Set<CorporateAction>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Exchange> Exchanges => Set<Exchange>();
    public DbSet<MarketDataProvider> MarketDataProviders => Set<MarketDataProvider>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<PositionSnapshot> PositionSnapshots => Set<PositionSnapshot>();
    public DbSet<PriceSnapshot> PriceSnapshots => Set<PriceSnapshot>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionFee> TransactionFees => Set<TransactionFee>();
    public DbSet<Watchlist> Watchlists => Set<Watchlist>();
    public DbSet<WatchlistItem> WatchlistItems => Set<WatchlistItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Asset>(entity =>
        {
            entity.ToTable("asset");
            entity.HasKey(e => e.Id).HasName("asset_pk");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssetTypeId).HasColumnName("asset_type_id");
            entity.Property(e => e.ExchangeId).HasColumnName("exchange_id");
            entity.Property(e => e.CurrencyId).HasColumnName("currency_id");
            entity.Property(e => e.Symbol).HasColumnName("symbol").HasMaxLength(128);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256);
            entity.Property(e => e.SteamMarketHashName).HasColumnName("steam_market_hash_name").HasMaxLength(512);
            entity.Property(e => e.ExternalId).HasColumnName("external_id").HasMaxLength(128);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne(e => e.AssetType).WithMany(e => e.Assets).HasForeignKey(e => e.AssetTypeId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_1");
            entity.HasOne(e => e.Exchange).WithMany(e => e.Assets).HasForeignKey(e => e.ExchangeId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_2");
            entity.HasOne(e => e.Currency).WithMany(e => e.Assets).HasForeignKey(e => e.CurrencyId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_3");
        });

        builder.Entity<AssetTag>(entity =>
        {
            entity.ToTable("asset_tag");
            entity.HasKey(e => new { e.AssetId, e.TagId }).HasName("asset_tag_pk");
            entity.Property(e => e.AssetId).HasColumnName("asset_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");

            entity.HasOne(e => e.Asset).WithMany(e => e.AssetTags).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_21");
            entity.HasOne(e => e.Tag).WithMany(e => e.AssetTags).HasForeignKey(e => e.TagId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_22");
        });

        builder.Entity<AssetType>(entity =>
        {
            entity.ToTable("asset_type");
            entity.HasKey(e => e.Id).HasName("asset_type_pk");
            entity.HasIndex(e => e.Code).IsUnique().HasDatabaseName("AK_1");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(32);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(64);
        });

        builder.Entity<CorporateAction>(entity =>
        {
            entity.ToTable("corporate_action");
            entity.HasKey(e => e.Id).HasName("corporate_action_pk");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssetId).HasColumnName("asset_id");
            entity.Property(e => e.ActionType).HasColumnName("action_type").HasMaxLength(32);
            entity.Property(e => e.ActionDate).HasColumnName("action_date");
            entity.Property(e => e.RatioFrom).HasColumnName("ratio_from").HasPrecision(20, 8);
            entity.Property(e => e.RatioTo).HasColumnName("ratio_to").HasPrecision(20, 8);
            entity.Property(e => e.Note).HasColumnName("note");

            entity.HasOne(e => e.Asset).WithMany(e => e.CorporateActions).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_16");
        });

        builder.Entity<Currency>(entity =>
        {
            entity.ToTable("currency");
            entity.HasKey(e => e.Id).HasName("currency_pk");
            entity.HasIndex(e => e.Code).IsUnique().HasDatabaseName("AK_0");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(8);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(64);
            entity.Property(e => e.Symbol).HasColumnName("symbol").HasMaxLength(8);
        });

        builder.Entity<Exchange>(entity =>
        {
            entity.ToTable("exchange");
            entity.HasKey(e => e.Id).HasName("exchange_pk");
            entity.HasIndex(e => e.Code).IsUnique().HasDatabaseName("AK_2");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(32);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(128);
            entity.Property(e => e.Country).HasColumnName("country").HasMaxLength(64);
            entity.Property(e => e.Timezone).HasColumnName("timezone").HasMaxLength(64);
        });

        builder.Entity<MarketDataProvider>(entity =>
        {
            entity.ToTable("market_data_provider");
            entity.HasKey(e => e.Id).HasName("market_data_provider_pk");
            entity.HasIndex(e => e.Code).IsUnique().HasDatabaseName("AK_3");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(32);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(128);
            entity.Property(e => e.BaseUrl).HasColumnName("base_url");
        });

        builder.Entity<Note>(entity =>
        {
            entity.ToTable("note");
            entity.HasKey(e => e.Id).HasName("note_pk");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppUserId).HasColumnName("app_user_id").HasMaxLength(450);
            entity.Property(e => e.AssetId).HasColumnName("asset_id");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne(e => e.Asset).WithMany(e => e.Notes).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_23");
            entity.HasOne(e => e.Transaction).WithMany(e => e.Notes).HasForeignKey(e => e.TransactionId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_24");
        });

        builder.Entity<Portfolio>(entity =>
        {
            entity.ToTable("portfolio");
            entity.HasKey(e => e.Id).HasName("portfolio_pk");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppUserId).HasColumnName("app_user_id").HasMaxLength(450);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(128);
            entity.Property(e => e.BaseCurrencyId).HasColumnName("base_currency_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne(e => e.BaseCurrency).WithMany(e => e.Portfolios).HasForeignKey(e => e.BaseCurrencyId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_0");
        });

        builder.Entity<PositionSnapshot>(entity =>
        {
            entity.ToTable("position_snapshot");
            entity.HasKey(e => e.Id).HasName("position_snapshot_pk");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PortfolioId).HasColumnName("portfolio_id");
            entity.Property(e => e.AssetId).HasColumnName("asset_id");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.Quantity).HasColumnName("quantity").HasPrecision(20, 8);
            entity.Property(e => e.AvgCost).HasColumnName("avg_cost").HasPrecision(20, 8);
            entity.Property(e => e.CostCurrencyId).HasColumnName("cost_currency_id");
            entity.Property(e => e.MarketPrice).HasColumnName("market_price").HasPrecision(20, 8);
            entity.Property(e => e.MarketCurrencyId).HasColumnName("market_currency_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne(e => e.Portfolio).WithMany(e => e.PositionSnapshots).HasForeignKey(e => e.PortfolioId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_17");
            entity.HasOne(e => e.Asset).WithMany(e => e.PositionSnapshots).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_18");
            entity.HasOne(e => e.CostCurrency).WithMany(e => e.PositionCostSnapshots).HasForeignKey(e => e.CostCurrencyId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_19");
            entity.HasOne(e => e.MarketCurrency).WithMany(e => e.PositionMarketSnapshots).HasForeignKey(e => e.MarketCurrencyId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_20");
        });

        builder.Entity<PriceSnapshot>(entity =>
        {
            entity.ToTable("price_snapshot");
            entity.HasKey(e => e.Id).HasName("price_snapshot_pk");
            entity.ToTable(t => t.HasCheckConstraint("CHECK_0", "price >= 0"));
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssetId).HasColumnName("asset_id");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.Price).HasColumnName("price").HasPrecision(20, 8);
            entity.Property(e => e.CurrencyId).HasColumnName("currency_id");
            entity.Property(e => e.AsOf).HasColumnName("as_of");

            entity.HasOne(e => e.Asset).WithMany(e => e.PriceSnapshots).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_4");
            entity.HasOne(e => e.Provider).WithMany(e => e.PriceSnapshots).HasForeignKey(e => e.ProviderId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_5");
            entity.HasOne(e => e.Currency).WithMany(e => e.PriceSnapshots).HasForeignKey(e => e.CurrencyId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_6");
        });

        builder.Entity<Tag>(entity =>
        {
            entity.ToTable("tag");
            entity.HasKey(e => e.Id).HasName("tag_pk");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppUserId).HasColumnName("app_user_id").HasMaxLength(450);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(64);
            entity.Property(e => e.Color).HasColumnName("color").HasMaxLength(16);
        });

        builder.Entity<Transaction>(entity =>
        {
            entity.ToTable("transaction");
            entity.HasKey(e => e.Id).HasName("transaction_pk");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PortfolioId).HasColumnName("portfolio_id");
            entity.Property(e => e.AssetId).HasColumnName("asset_id");
            entity.Property(e => e.TradeTime).HasColumnName("trade_time");
            entity.Property(e => e.Quantity).HasColumnName("quantity").HasPrecision(20, 8);
            entity.Property(e => e.UnitPrice).HasColumnName("unit_price").HasPrecision(20, 8);
            entity.Property(e => e.CurrencyId).HasColumnName("currency_id");
            entity.Property(e => e.CashAmount).HasColumnName("cash_amount").HasPrecision(20, 8);
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne(e => e.Portfolio).WithMany(e => e.Transactions).HasForeignKey(e => e.PortfolioId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_7");
            entity.HasOne(e => e.Asset).WithMany(e => e.Transactions).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_8");
            entity.HasOne(e => e.Currency).WithMany(e => e.Transactions).HasForeignKey(e => e.CurrencyId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_10");
        });

        builder.Entity<TransactionFee>(entity =>
        {
            entity.ToTable("transaction_fee");
            entity.HasKey(e => e.Id).HasName("transaction_fee_pk");
            entity.ToTable(t => t.HasCheckConstraint("CHECK_1", "amount >= 0"));
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.FeeType).HasColumnName("fee_type").HasMaxLength(64);
            entity.Property(e => e.Amount).HasColumnName("amount").HasPrecision(20, 8);
            entity.Property(e => e.CurrencyId).HasColumnName("currency_id");

            entity.HasOne(e => e.Transaction).WithMany(e => e.TransactionFees).HasForeignKey(e => e.TransactionId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_11");
            entity.HasOne(e => e.Currency).WithMany(e => e.TransactionFees).HasForeignKey(e => e.CurrencyId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_12");
        });

        builder.Entity<Watchlist>(entity =>
        {
            entity.ToTable("watchlist");
            entity.HasKey(e => e.Id).HasName("watchlist_pk");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppUserId).HasColumnName("app_user_id").HasMaxLength(450);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(128);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        });

        builder.Entity<WatchlistItem>(entity =>
        {
            entity.ToTable("watchlist_item");
            entity.HasKey(e => new { e.WatchlistId, e.AssetId }).HasName("watchlist_item_pk");
            entity.Property(e => e.WatchlistId).HasColumnName("watchlist_id");
            entity.Property(e => e.AssetId).HasColumnName("asset_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne(e => e.Watchlist).WithMany(e => e.WatchlistItems).HasForeignKey(e => e.WatchlistId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_25");
            entity.HasOne(e => e.Asset).WithMany(e => e.WatchlistItems).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_26");
        });
    }
}
