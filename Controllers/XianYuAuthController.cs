using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XianYu.API.Models.XianYu;
using XianYu.API.Services;

namespace XianYu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class XianYuAuthController : ControllerBase
{
    private readonly IXianYuAuthService _authService;
    private readonly ILogger<XianYuAuthController> _logger;

    public XianYuAuthController(
        IXianYuAuthService authService,
        ILogger<XianYuAuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("sendVerifyCode")]
    public async Task<ActionResult<SendVerifyCodeResponse>> SendVerifyCode([FromBody] SendVerifyCodeRequest request)
    {
        try
        {
            var response = await _authService.SendVerifyCodeAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送验证码时发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录时发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }

    [HttpPost("refreshToken")]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新令牌时发生错误");
            return StatusCode(500, new { message = "服务器内部错误" });
        }
    }
}
