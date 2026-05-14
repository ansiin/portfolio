using App.DAL.EF;
using App.DTO.v1.Lookups;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CurrenciesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CurrenciesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetCurrencies()
    {
        var currencies = await _context.Currencies
            .Where(currency => currency.IsActive)
            .OrderBy(currency => currency.Code)
            .ToListAsync();

        var result = currencies.Select(currency => new CurrencyDto
        {
            Id = currency.Id,
            Code = currency.Code,
            Symbol = currency.Symbol,
            DisplayName = currency.DisplayName.Translate() ?? currency.Code
        });

        return Ok(result);
    }
}
