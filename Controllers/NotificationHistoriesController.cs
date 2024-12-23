using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XianYu.API.DTOs.NotificationHistory;
using XianYu.API.Services;

namespace XianYu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class NotificationHistoriesController : ControllerBase
{
    private readonly INotificationHistoryService _notificationHistoryService;
    private readonly ILogger<NotificationHistoriesController> _logger;

    public NotificationHistoriesController(
        INotificationHistoryService notificationHistoryService,
        ILogger<NotificationHistoriesController> logger)
    {
        _notificationHistoryService = notificationHistoryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationHistoryDto>>> GetHistory([FromQuery] NotificationHistoryQueryParams queryParams)
    {
        try
        {
            var (histories, total) = await _notificationHistoryService.GetHistoryAsync(queryParams);
            Response.Headers.Append("X-Total-Count", total.ToString());
            return Ok(histories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取通知历史记录时发生错误");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationHistoryDto>> GetHistoryById(int id)
    {
        var history = await _notificationHistoryService.GetHistoryByIdAsync(id);
        if (history == null)
            return NotFound(new { message = "通知记录不存在" });

        return Ok(history);
    }
} 