using System.Net;
using App.BLL.Services;
using App.DTO.v1;
using App.DTO.v1.Notes;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotesController : ControllerBase
{
    private readonly NoteService _noteService;

    public NotesController(NoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotes([FromQuery] Guid? assetId = null, [FromQuery] Guid? transactionId = null)
    {
        return Ok(await _noteService.GetMyNotesAsync(assetId, transactionId));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<NoteDto>> GetNote(Guid id)
    {
        var note = await _noteService.GetMyNoteAsync(id);
        return note == null ? NotFound() : Ok(note);
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> CreateNote([FromBody] NoteCreateDto dto)
    {
        try
        {
            var created = await _noteService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetNote), new { version = "1.0", id = created.Id }, created);
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
    public async Task<IActionResult> UpdateNote(Guid id, [FromBody] NoteUpdateDto dto)
    {
        try
        {
            var updated = await _noteService.UpdateAsync(id, dto);
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
    public async Task<IActionResult> DeleteNote(Guid id)
    {
        var deleted = await _noteService.DeleteAsync(id);
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
