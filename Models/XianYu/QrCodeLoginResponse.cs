namespace XianYu.API.Models.XianYu;

public class QrCodeLoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string QrCodeUrl { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
} 