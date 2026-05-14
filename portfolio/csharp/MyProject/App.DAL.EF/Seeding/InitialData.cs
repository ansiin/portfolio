namespace App.DAL.EF.Seeding;

public static class InitialData
{
    public static readonly string[] Roles = new[]
    {
        "user",
        "admin"
    };

    public static readonly (string email, string password, string[] roles)[] Users = new[]
    {
        ("admin@taltech.ee", "Kala.12345", new[] { "admin" }),
        ("user@taltech.ee", "Kala.12345", new[] { "user" }),
    };

    public static readonly (string code, string? symbol, string englishName, string estonianName)[] Currencies = new (string code, string? symbol, string englishName, string estonianName)[]
    {
        ("EUR", "\u20AC", "Euro", "Euro"),
        ("USD", "$", "US Dollar", "USA dollar"),
        ("GBP", "\u00A3", "British Pound", "Suurbritannia nael")
    };

    public static readonly (string code, string englishName, string estonianName)[] AssetTypes = new[]
    {
        ("stock", "Stock", "Aktsia"),
        ("crypto", "Crypto", "Kr\u00FCpto"),
        ("cs2-skin", "CS2 Skin", "CS2 skin")
    };

    public static readonly (string code, string englishName, string estonianName)[] Exchanges = new[]
    {
        ("NASDAQ", "Nasdaq", "Nasdaq"),
        ("NYSE", "New York Stock Exchange", "New Yorgi b\u00F6rs"),
        ("STEAM", "Steam Market", "Steam Market")
    };

    public static readonly (string code, string englishName, string estonianName, string? baseUrl)[] MarketDataProviders = new[]
    {
        ("manual", "Manual", "K\u00E4sitsi", null),
        ("yfinance", "Yahoo Finance", "Yahoo Finance", "https://finance.yahoo.com")
    };
}
