using System.Net;
using App.BLL.Services;
using App.DTO.v1;
using App.DTO.v1.Portfolios;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PortfoliosController : ControllerBase
{
    private readonly PortfolioService _portfolioService;

    public PortfoliosController(PortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PortfolioDto>>> GetPortfolios()
    {
        return Ok(await _portfolioService.GetMyPortfoliosAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PortfolioDto>> GetPortfolio(Guid id)
    {
        var portfolio = await _portfolioService.GetMyPortfolioAsync(id);
        return portfolio == null ? NotFound() : Ok(portfolio);
    }

    [HttpPost]
    public async Task<ActionResult<PortfolioDto>> CreatePortfolio([FromBody] PortfolioCreateDto dto)
    {
        try
        {
            var created = await _portfolioService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPortfolio), new { version = "1.0", id = created.Id }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new RestApiErrorResponse
            {
                Status = HttpStatusCode.BadRequest,
                Error = ex.Message
            });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePortfolio(Guid id, [FromBody] PortfolioUpdateDto dto)
    {
        try
        {
            var updated = await _portfolioService.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new RestApiErrorResponse
            {
                Status = HttpStatusCode.BadRequest,
                Error = ex.Message
            });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePortfolio(Guid id)
    {
        try
        {
            var deleted = await _portfolioService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new RestApiErrorResponse
            {
                Status = HttpStatusCode.Conflict,
                Error = ex.Message
            });
        }
    }
}
