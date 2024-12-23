using XianYu.API.Models.XianYu;

namespace XianYu.API.Services;

public interface IXianYuQrCodeService
{
    Task<QrCodeLoginResponse> GetQrCodeAsync();
    Task<QrCodeLoginResponse> CheckQrCodeStatusAsync(string token);
} 