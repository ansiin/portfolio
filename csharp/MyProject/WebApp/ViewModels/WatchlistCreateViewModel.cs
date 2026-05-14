using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class WatchlistCreateViewModel
{
    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; } = default!;
}
