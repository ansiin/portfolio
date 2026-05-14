using System.Net;
using App.BLL.Services;
using App.DTO.v1;
using App.DTO.v1.Assets;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AssetsController : ControllerBase
{
    private readonly AssetService _assetService;

    public AssetsController(AssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssetDto>>> GetAssets([FromQuery] Guid? portfolioId = null)
    {
        return Ok(await _assetService.GetMyAssetsAsync(portfolioId));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssetDto>> GetAsset(Guid id)
    {
        var asset = await _assetService.GetMyAssetAsync(id);
        return asset == null ? NotFound() : Ok(asset);
    }

    [HttpPost]
    public async Task<ActionResult<AssetDto>> CreateAsset([FromBody] AssetCreateDto dto)
    {
        try
        {
            var created = await _assetService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAsset), new { version = "1.0", id = created.Id }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ToErrorResponse(HttpStatusCode.BadRequest, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ToErrorResponse(HttpStatusCode.Conflict, ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsset(Guid id, [FromBody] AssetUpdateDto dto)
    {
        try
        {
            var updated = await _assetService.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ToErrorResponse(HttpStatusCode.BadRequest, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ToErrorResponse(HttpStatusCode.Conflict, ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateAsset(Guid id)
    {
        var updated = await _assetService.DeactivateAsync(id);
        return updated ? NoContent() : NotFound();
    }

    private static RestApiErrorResponse ToErrorResponse(HttpStatusCode statusCode, string message)
    {
        return new RestApiErrorResponse
        {
            Status = statusCode,
            Error = message
        };
    }
}
