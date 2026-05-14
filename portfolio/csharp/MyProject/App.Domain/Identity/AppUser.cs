using Base.Contracts;
using Microsoft.AspNetCore.Identity;
using App.Domain;

namespace App.Domain.Identity;

public class AppUser : IdentityUser<Guid>, IBaseEntity
{
    public ICollection<ListItem>? ListItems { get; set; }
    public ICollection<Portfolio>? Portfolios { get; set; }
    public ICollection<Watchlist>? Watchlists { get; set; }
    public ICollection<Tag>? Tags { get; set; }
    public ICollection<Note>? Notes { get; set; }
    public ICollection<AppRefreshToken>? RefreshTokens { get; set; }
}
