using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XianYu.API.Services;
using XianYu.API.Models.XianYu;

namespace XianYu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IXianYuAuthService _xianYuAuthService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IXianYuAuthService xianYuAuthService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _xianYuAuthService = xianYuAuthService;
        _logger = logger;
    }

    [HttpPost("sendVerifyCode")]
    public async Task<IActionResult> SendVerifyCode([FromBody] SendVerifyCodeRequest request)
    {
        try
        {
            _logger.LogInformation("开始发送验证码: {PhoneNumber}", request.PhoneNumber);
            var response = await _xianYuAuthService.SendVerifyCodeAsync(request);
            
            // 不管是否需要验证码，都返回 200 状态码
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送验证码时发生错误");
            return StatusCode(500, new SendVerifyCodeResponse
            {
                Success = false,
                Message = "发送验证码时发生错误"
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("开始处理登录请求: {PhoneNumber}", request.PhoneNumber);
            var response = await _xianYuAuthService.LoginAsync(request);
            
            // 不管是否需要验证码，都返回 200 状态码
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录时发生错误");
            return StatusCode(500, new LoginResponse
            {
                Success = false,
                Message = "登录时发生错误"
            });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        _logger.LogInformation("开始处理刷新令牌请求");
        
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            _logger.LogWarning("刷新令牌为空");
            return BadRequest(new LoginResponse
            {
                Success = false,
                Message = "刷新令牌不能为空"
            });
        }

        var response = await _authService.RefreshTokenAsync(request.RefreshToken);
        if (!response.Success)
        {
            _logger.LogWarning("刷新令牌失败: {Message}", response.Message);
            return BadRequest(response);
        }

        _logger.LogInformation("刷新令牌成功");
        return Ok(response);
    }
} 