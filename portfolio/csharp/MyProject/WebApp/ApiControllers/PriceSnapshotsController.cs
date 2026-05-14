using System.Net;
using App.BLL.Services;
using App.DTO.v1;
using App.DTO.v1.PriceSnapshots;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PriceSnapshotsController : ControllerBase
{
    private readonly PriceSnapshotService _priceSnapshotService;

    public PriceSnapshotsController(PriceSnapshotService priceSnapshotService)
    {
        _priceSnapshotService = priceSnapshotService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PriceSnapshotDto>>> GetPriceSnapshots([FromQuery] Guid? assetId = null)
    {
        return Ok(await _priceSnapshotService.GetMyPriceSnapshotsAsync(assetId));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PriceSnapshotDto>> GetPriceSnapshot(Guid id)
    {
        var snapshot = await _priceSnapshotService.GetMyPriceSnapshotAsync(id);
        return snapshot == null ? NotFound() : Ok(snapshot);
    }

    [HttpPost]
    public async Task<ActionResult<PriceSnapshotDto>> CreatePriceSnapshot([FromBody] PriceSnapshotCreateDto dto)
    {
        try
        {
            var created = await _priceSnapshotService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPriceSnapshot), new { version = "1.0", id = created.Id }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ToErrorResponse(HttpStatusCode.BadRequest, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ToErrorResponse(HttpStatusCode.BadRequest, ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePriceSnapshot(Guid id, [FromBody] PriceSnapshotUpdateDto dto)
    {
        try
        {
            var updated = await _priceSnapshotService.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ToErrorResponse(HttpStatusCode.BadRequest, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ToErrorResponse(HttpStatusCode.BadRequest, ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePriceSnapshot(Guid id)
    {
        var deleted = await _priceSnapshotService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
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
