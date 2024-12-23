using XianYu.API.Models.XianYu;

namespace XianYu.API.Services
{
    public interface IXianYuAuthService
    {
        Task<SendVerifyCodeResponse> SendVerifyCodeAsync(SendVerifyCodeRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(string refreshToken);
        Task<LoginResponse> HandleCallbackAsync(XianYuCallbackRequest request);
    }
}
