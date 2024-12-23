using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XianYu.API.DTOs.NotificationTemplate;
using XianYu.API.Services;

namespace XianYu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class NotificationTemplatesController : ControllerBase
{
    private readonly INotificationTemplateService _templateService;

    public NotificationTemplatesController(INotificationTemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationTemplateDto>>> GetTemplates()
    {
        var templates = await _templateService.GetTemplatesAsync();
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationTemplateDto>> GetTemplate(int id)
    {
        var template = await _templateService.GetTemplateAsync(id);
        if (template == null)
            return NotFound(new { message = "模板不存在" });

        return Ok(template);
    }

    [HttpPost]
    public async Task<ActionResult<NotificationTemplateDto>> CreateTemplate(CreateNotificationTemplateRequest request)
    {
        var template = await _templateService.CreateTemplateAsync(request);
        return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<NotificationTemplateDto>> UpdateTemplate(int id, UpdateNotificationTemplateRequest request)
    {
        var template = await _templateService.UpdateTemplateAsync(id, request);
        if (template == null)
            return NotFound(new { message = "模板不存在" });

        return Ok(template);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTemplate(int id)
    {
        var result = await _templateService.DeleteTemplateAsync(id);
        if (!result)
            return NotFound(new { message = "模板不存在" });

        return Ok(new { message = "删除成功" });
    }
} 