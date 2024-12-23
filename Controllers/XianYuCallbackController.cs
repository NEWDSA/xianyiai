using Microsoft.AspNetCore.Mvc;
using XianYu.API.Models.XianYu;
using XianYu.API.Services;

namespace XianYu.API.Controllers;

[ApiController]
[Route("api/xianyucallback")]
public class XianYuCallbackController : ControllerBase
{
    private readonly ILogger<XianYuCallbackController> _logger;
    private readonly IXianYuAuthService _xianYuAuthService;

    public XianYuCallbackController(
        ILogger<XianYuCallbackController> logger,
        IXianYuAuthService xianYuAuthService)
    {
        _logger = logger;
        _xianYuAuthService = xianYuAuthService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<XianYuCallbackResponse>> HandleLoginCallback([FromBody] XianYuCallbackRequest request)
    {
        try
        {
            var response = await _xianYuAuthService.HandleCallbackAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理闲鱼登录回调失败");
            return StatusCode(500, new XianYuCallbackResponse
            {
                Success = false,
                Message = "处理回调失��"
            });
        }
    }
}
