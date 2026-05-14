using App.BLL.Services;
using App.DTO.v1.PositionSnapshots;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PositionSnapshotsController : ControllerBase
{
    private readonly PositionSnapshotService _positionSnapshotService;

    public PositionSnapshotsController(PositionSnapshotService positionSnapshotService)
    {
        _positionSnapshotService = positionSnapshotService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PositionSnapshotDto>>> GetPositionSnapshots(
        [FromQuery] Guid? portfolioId = null,
        [FromQuery] Guid? assetId = null)
    {
        return Ok(await _positionSnapshotService.GetMyPositionSnapshotsAsync(portfolioId, assetId));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PositionSnapshotDto>> GetPositionSnapshot(Guid id)
    {
        var snapshot = await _positionSnapshotService.GetMyPositionSnapshotAsync(id);
        return snapshot == null ? NotFound() : Ok(snapshot);
    }

    [HttpPost("generate")]
    public async Task<ActionResult<IEnumerable<PositionSnapshotDto>>> GenerateCurrentSnapshots([FromBody] PositionSnapshotGenerateDto dto)
    {
        var created = await _positionSnapshotService.GenerateCurrentSnapshotsAsync(dto.PortfolioId, dto.SnapshotAt);
        return Ok(created);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePositionSnapshot(Guid id)
    {
        var deleted = await _positionSnapshotService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
