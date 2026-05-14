using App.BLL.Services;
using App.DTO.v1.Dashboard;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        return Ok(await _dashboardService.GetSummaryAsync());
    }

    [HttpGet("allocation")]
    public async Task<ActionResult<IEnumerable<DashboardAllocationItemDto>>> GetAllocation()
    {
        return Ok(await _dashboardService.GetAllocationAsync());
    }

    [HttpGet("timeline")]
    public async Task<ActionResult<IEnumerable<DashboardTimelinePointDto>>> GetTimeline()
    {
        return Ok(await _dashboardService.GetTimelineAsync());
    }
}
