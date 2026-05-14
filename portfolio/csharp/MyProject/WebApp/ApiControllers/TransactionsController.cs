using System.Net;
using App.BLL.Services;
using App.DTO.v1;
using App.DTO.v1.Transactions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TransactionsController : ControllerBase
{
    private readonly TransactionService _transactionService;

    public TransactionsController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions([FromQuery] Guid? portfolioId = null)
    {
        return Ok(await _transactionService.GetMyTransactionsAsync(portfolioId));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransactionDto>> GetTransaction(Guid id)
    {
        var transaction = await _transactionService.GetMyTransactionAsync(id);
        return transaction == null ? NotFound() : Ok(transaction);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] TransactionCreateDto dto)
    {
        try
        {
            var created = await _transactionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetTransaction), new { version = "1.0", id = created.Id }, created);
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
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] TransactionUpdateDto dto)
    {
        try
        {
            var updated = await _transactionService.UpdateAsync(id, dto);
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
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        var deleted = await _transactionService.DeleteAsync(id);
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
