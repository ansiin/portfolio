using System.Net;
using App.BLL.Services;
using App.DTO.v1;
using App.DTO.v1.Watchlists;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WatchlistsController : ControllerBase
{
    private readonly WatchlistService _watchlistService;

    public WatchlistsController(WatchlistService watchlistService)
    {
        _watchlistService = watchlistService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WatchlistDto>>> GetWatchlists()
    {
        return Ok(await _watchlistService.GetMyWatchlistsAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WatchlistDto>> GetWatchlist(Guid id)
    {
        var watchlist = await _watchlistService.GetMyWatchlistAsync(id);
        return watchlist == null ? NotFound() : Ok(watchlist);
    }

    [HttpPost]
    public async Task<ActionResult<WatchlistDto>> CreateWatchlist([FromBody] WatchlistCreateDto dto)
    {
        var created = await _watchlistService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetWatchlist), new { version = "1.0", id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWatchlist(Guid id, [FromBody] WatchlistUpdateDto dto)
    {
        var updated = await _watchlistService.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWatchlist(Guid id)
    {
        var deleted = await _watchlistService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/items")]
    public async Task<ActionResult<WatchlistItemDto>> AddWatchlistItem(Guid id, [FromBody] WatchlistItemCreateDto dto)
    {
        try
        {
            var created = await _watchlistService.AddItemAsync(id, dto);
            return CreatedAtAction(nameof(GetWatchlist), new { version = "1.0", id }, created);
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

    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveWatchlistItem(Guid id, Guid itemId)
    {
        var deleted = await _watchlistService.RemoveItemAsync(id, itemId);
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
