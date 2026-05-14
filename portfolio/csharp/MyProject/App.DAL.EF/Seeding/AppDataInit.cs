using System.Data;
using App.Domain;
using App.Domain.Enums;
using App.Domain.Identity;
using Base.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Seeding;

public static class AppDataInit
{
    public static void DeleteDatabase(AppDbContext context)
    {
        context.Database.EnsureDeleted();
    }

    public static void MigrateDatabase(AppDbContext context)
    {
        context.Database.Migrate();
    }


    public static void SeedAppData(AppDbContext context)
    {
        var existingTables = GetExistingTables(context);
        if (!existingTables.Contains(nameof(AppDbContext.Currencies)) ||
            !existingTables.Contains(nameof(AppDbContext.AssetTypes)) ||
            !existingTables.Contains(nameof(AppDbContext.Exchanges)) ||
            !existingTables.Contains(nameof(AppDbContext.MarketDataProviders)))
        {
            Console.WriteLine("Skipping app data seed because investing lookup tables are not present yet.");
            return;
        }

        foreach (var currency in InitialData.Currencies)
        {
            if (context.Currencies.Any(entity => entity.Code == currency.code)) continue;

            context.Currencies.Add(new Currency
            {
                Code = currency.code,
                Symbol = currency.symbol,
                DisplayName = CreateLangStr(currency.englishName, currency.estonianName),
                IsActive = true
            });
        }

        foreach (var assetType in InitialData.AssetTypes)
        {
            if (context.AssetTypes.Any(entity => entity.Code == assetType.code)) continue;

            context.AssetTypes.Add(new AssetType
            {
                Code = assetType.code,
                DisplayName = CreateLangStr(assetType.englishName, assetType.estonianName),
                IsActive = true
            });
        }

        foreach (var exchange in InitialData.Exchanges)
        {
            if (context.Exchanges.Any(entity => entity.Code == exchange.code)) continue;

            context.Exchanges.Add(new Exchange
            {
                Code = exchange.code,
                DisplayName = CreateLangStr(exchange.englishName, exchange.estonianName),
                IsActive = true
            });
        }

        foreach (var provider in InitialData.MarketDataProviders)
        {
            if (context.MarketDataProviders.Any(entity => entity.Code == provider.code)) continue;

            context.MarketDataProviders.Add(new MarketDataProvider
            {
                Code = provider.code,
                BaseUrl = provider.baseUrl,
                DisplayName = CreateLangStr(provider.englishName, provider.estonianName),
                IsActive = true
            });
        }

        context.SaveChanges();

        SeedDemoPortfolioData(context);
    }
    
     public static void SeedIdentity(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        foreach (var roleName in InitialData.Roles)
        {
            var role = roleManager.FindByNameAsync(roleName).Result;

            if (role != null) continue;

            role = new AppRole()
            {
                Name = roleName,
            };

            var result = roleManager.CreateAsync(role).Result;
            if (!result.Succeeded)
            {
                throw new ApplicationException("Role creation failed!");
            }
        }


        foreach (var userInfo in InitialData.Users)
        {
            var user = userManager.FindByEmailAsync(userInfo.email).Result;
            if (user == null)
            {
                user = new AppUser()
                {
                    Email = userInfo.email,
                    UserName = userInfo.email,
                    EmailConfirmed = true
                };
                var result = userManager.CreateAsync(user, userInfo.password).Result;
                if (!result.Succeeded)
                {
                    throw new ApplicationException("User creation failed!");
                }
            }

            foreach (var role in userInfo.roles)
            {
                if (userManager.IsInRoleAsync(user, role).Result)
                {
                    Console.WriteLine($"User {user.UserName} already in role {role}");
                    continue;
                }
                
                var roleResult = userManager.AddToRoleAsync(user, role).Result;
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
                else
                {
                    Console.WriteLine($"User {user.UserName} added to role {role}");
                }
            }
        }
    }

    private static LangStr CreateLangStr(string englishValue, string estonianValue)
    {
        var result = new LangStr(englishValue, "en");
        result.SetTranslation(estonianValue, "et");
        return result;
    }

    private static void SeedDemoPortfolioData(AppDbContext context)
    {
        const string demoUserEmail = "user@taltech.ee";
        var demoUser = context.Users
            .AsNoTracking()
            .FirstOrDefault(user => user.Email == demoUserEmail);

        if (demoUser == null)
        {
            return;
        }

        if (context.Portfolios.Any(portfolio => portfolio.AppUserId == demoUser.Id))
        {
            return;
        }

        var eur = context.Currencies.AsNoTracking().First(currency => currency.Code == "EUR");
        var usd = context.Currencies.AsNoTracking().First(currency => currency.Code == "USD");
        var stockType = context.AssetTypes.AsNoTracking().First(assetType => assetType.Code == "stock");
        var cryptoType = context.AssetTypes.AsNoTracking().First(assetType => assetType.Code == "crypto");
        var nasdaq = context.Exchanges.AsNoTracking().First(exchange => exchange.Code == "NASDAQ");
        var manualProvider = context.MarketDataProviders.AsNoTracking().First(provider => provider.Code == "manual");

        var corePortfolio = new Portfolio
        {
            Name = "Core Growth",
            AppUserId = demoUser.Id,
            BaseCurrencyId = eur.Id,
            IsArchived = false
        };

        var satellitePortfolio = new Portfolio
        {
            Name = "Opportunity",
            AppUserId = demoUser.Id,
            BaseCurrencyId = eur.Id,
            IsArchived = false
        };

        context.Portfolios.AddRange(corePortfolio, satellitePortfolio);

        var msftAsset = new Asset
        {
            Name = "Microsoft",
            Symbol = "MSFT",
            Portfolio = corePortfolio,
            AssetTypeId = stockType.Id,
            CurrencyId = usd.Id,
            ExchangeId = nasdaq.Id,
            MarketDataProviderId = manualProvider.Id,
            IsActive = true
        };

        var nvdaAsset = new Asset
        {
            Name = "NVIDIA",
            Symbol = "NVDA",
            Portfolio = corePortfolio,
            AssetTypeId = stockType.Id,
            CurrencyId = usd.Id,
            ExchangeId = nasdaq.Id,
            MarketDataProviderId = manualProvider.Id,
            IsActive = true
        };

        var btcAsset = new Asset
        {
            Name = "Bitcoin",
            Symbol = "BTC",
            Portfolio = satellitePortfolio,
            AssetTypeId = cryptoType.Id,
            CurrencyId = eur.Id,
            MarketDataProviderId = manualProvider.Id,
            IsActive = true
        };

        context.Assets.AddRange(msftAsset, nvdaAsset, btcAsset);

        var transactions = new List<Transaction>
        {
            new()
            {
                Portfolio = corePortfolio,
                Type = TransactionType.Deposit,
                ExecutedAt = new DateTime(2025, 11, 15, 9, 0, 0, DateTimeKind.Utc),
                Quantity = 0m,
                UnitPrice = 1m,
                TotalAmount = 15000m,
                Description = "Initial capital allocation"
            },
            new()
            {
                Portfolio = corePortfolio,
                Asset = msftAsset,
                Type = TransactionType.Buy,
                ExecutedAt = new DateTime(2025, 11, 18, 14, 0, 0, DateTimeKind.Utc),
                Quantity = 18m,
                UnitPrice = 410m,
                TotalAmount = 7380m,
                Description = "Core US large-cap position"
            },
            new()
            {
                Portfolio = corePortfolio,
                Asset = nvdaAsset,
                Type = TransactionType.Buy,
                ExecutedAt = new DateTime(2025, 12, 2, 14, 0, 0, DateTimeKind.Utc),
                Quantity = 10m,
                UnitPrice = 465m,
                TotalAmount = 4650m,
                Description = "AI growth allocation"
            },
            new()
            {
                Portfolio = corePortfolio,
                Asset = msftAsset,
                Type = TransactionType.Dividend,
                ExecutedAt = new DateTime(2026, 2, 12, 10, 0, 0, DateTimeKind.Utc),
                Quantity = 0m,
                UnitPrice = 0m,
                TotalAmount = 42m,
                Description = "Quarterly dividend"
            },
            new()
            {
                Portfolio = corePortfolio,
                Asset = nvdaAsset,
                Type = TransactionType.Sell,
                ExecutedAt = new DateTime(2026, 3, 14, 13, 0, 0, DateTimeKind.Utc),
                Quantity = 2m,
                UnitPrice = 612m,
                TotalAmount = 1224m,
                Description = "Partial profit taking"
            },
            new()
            {
                Portfolio = satellitePortfolio,
                Type = TransactionType.Deposit,
                ExecutedAt = new DateTime(2026, 1, 8, 8, 30, 0, DateTimeKind.Utc),
                Quantity = 0m,
                UnitPrice = 1m,
                TotalAmount = 6000m,
                Description = "Satellite allocation"
            },
            new()
            {
                Portfolio = satellitePortfolio,
                Asset = btcAsset,
                Type = TransactionType.Buy,
                ExecutedAt = new DateTime(2026, 1, 10, 8, 30, 0, DateTimeKind.Utc),
                Quantity = 0.085m,
                UnitPrice = 58800m,
                TotalAmount = 4998m,
                Description = "High conviction crypto allocation"
            },
            new()
            {
                Portfolio = satellitePortfolio,
                Type = TransactionType.Withdrawal,
                ExecutedAt = new DateTime(2026, 3, 28, 11, 0, 0, DateTimeKind.Utc),
                Quantity = 0m,
                UnitPrice = 1m,
                TotalAmount = 750m,
                Description = "Cash rebalancing"
            }
        };

        context.Transactions.AddRange(transactions);
        context.TransactionFees.AddRange(
            new TransactionFee
            {
                Transaction = transactions[1],
                FeeType = "Brokerage",
                Amount = 3.50m
            },
            new TransactionFee
            {
                Transaction = transactions[2],
                FeeType = "Brokerage",
                Amount = 2.95m
            },
            new TransactionFee
            {
                Transaction = transactions[4],
                FeeType = "Brokerage",
                Amount = 1.95m
            },
            new TransactionFee
            {
                Transaction = transactions[6],
                FeeType = "Exchange",
                Amount = 12m
            });

        context.PriceSnapshots.AddRange(
            new PriceSnapshot
            {
                Asset = msftAsset,
                CurrencyId = usd.Id,
                MarketDataProviderId = manualProvider.Id,
                RecordedAt = new DateTime(2026, 1, 31, 16, 0, 0, DateTimeKind.Utc),
                Price = 428m
            },
            new PriceSnapshot
            {
                Asset = msftAsset,
                CurrencyId = usd.Id,
                MarketDataProviderId = manualProvider.Id,
                RecordedAt = new DateTime(2026, 4, 10, 16, 0, 0, DateTimeKind.Utc),
                Price = 452m
            },
            new PriceSnapshot
            {
                Asset = nvdaAsset,
                CurrencyId = usd.Id,
                MarketDataProviderId = manualProvider.Id,
                RecordedAt = new DateTime(2026, 1, 31, 16, 0, 0, DateTimeKind.Utc),
                Price = 518m
            },
            new PriceSnapshot
            {
                Asset = nvdaAsset,
                CurrencyId = usd.Id,
                MarketDataProviderId = manualProvider.Id,
                RecordedAt = new DateTime(2026, 4, 10, 16, 0, 0, DateTimeKind.Utc),
                Price = 635m
            },
            new PriceSnapshot
            {
                Asset = btcAsset,
                CurrencyId = eur.Id,
                MarketDataProviderId = manualProvider.Id,
                RecordedAt = new DateTime(2026, 2, 28, 8, 0, 0, DateTimeKind.Utc),
                Price = 60350m
            },
            new PriceSnapshot
            {
                Asset = btcAsset,
                CurrencyId = eur.Id,
                MarketDataProviderId = manualProvider.Id,
                RecordedAt = new DateTime(2026, 4, 10, 8, 0, 0, DateTimeKind.Utc),
                Price = 67200m
            });

        context.Watchlists.Add(new Watchlist
        {
            AppUserId = demoUser.Id,
            Name = "Next buys",
            Items = new List<WatchlistItem>
            {
                new() { Asset = msftAsset },
                new() { Asset = nvdaAsset }
            }
        });

        context.Notes.AddRange(
            new Note
            {
                AppUserId = demoUser.Id,
                Asset = msftAsset,
                Title = "Quality thesis",
                Content = "Cloud cash flows stay resilient and the position fits the lower-risk core bucket.",
                CreatedAt = new DateTime(2026, 1, 15, 7, 0, 0, DateTimeKind.Utc)
            },
            new Note
            {
                AppUserId = demoUser.Id,
                Asset = btcAsset,
                Title = "Risk sizing",
                Content = "Keep the satellite sleeve below 15% of total portfolio value and rebalance on sharp rallies.",
                CreatedAt = new DateTime(2026, 2, 20, 7, 0, 0, DateTimeKind.Utc)
            });

        context.SaveChanges();
    }

    private static HashSet<string> GetExistingTables(AppDbContext context)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var connection = context.Database.GetDbConnection();
        var wasClosed = connection.State != ConnectionState.Open;

        try
        {
            if (wasClosed)
            {
                connection.Open();
            }

            using var command = connection.CreateCommand();
            command.CommandText = "select tablename from pg_tables where schemaname = 'public'";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(reader.GetString(0));
            }
        }
        finally
        {
            if (wasClosed)
            {
                connection.Close();
            }
        }

        return result;
    }
}
