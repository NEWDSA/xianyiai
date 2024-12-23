using XianYu.API.Models.XianYu;

namespace XianYu.API.Services;

public class AuthService : IAuthService
{
    private readonly IXianYuAuthService _xianYuAuthService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IXianYuAuthService xianYuAuthService,
        ILogger<AuthService> logger)
    {
        _xianYuAuthService = xianYuAuthService;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _xianYuAuthService.LoginAsync(request);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录失败");
            return new LoginResponse
            {
                Success = false,
                Message = "登录失败"
            };
        }
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var response = await _xianYuAuthService.RefreshTokenAsync(refreshToken);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新令牌失败");
            return new LoginResponse
            {
                Success = false,
                Message = "刷新令牌失败"
            };
        }
    }
} 