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
public class AssetTypesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AssetTypesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetAssetTypes()
    {
        var entities = await _context.AssetTypes
            .Where(entity => entity.IsActive)
            .OrderBy(entity => entity.Code)
            .ToListAsync();

        return Ok(entities.Select(entity => new LookupItemDto
        {
            Id = entity.Id,
            Code = entity.Code,
            DisplayName = entity.DisplayName.Translate() ?? entity.Code
        }));
    }
}
