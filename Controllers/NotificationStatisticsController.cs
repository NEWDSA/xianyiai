using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XianYu.API.DTOs.NotificationStatistics;
using XianYu.API.Services;

namespace XianYu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class NotificationStatisticsController : ControllerBase
{
    private readonly INotificationStatisticsService _statisticsService;

    public NotificationStatisticsController(INotificationStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet]
    public async Task<ActionResult<NotificationStatisticsDto>> GetStatistics([FromQuery] NotificationStatisticsQueryParams queryParams)
    {
        var statistics = await _statisticsService.GetStatisticsAsync(queryParams);
        return Ok(statistics);
    }
} 