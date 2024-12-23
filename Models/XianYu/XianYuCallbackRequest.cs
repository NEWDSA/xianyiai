namespace XianYu.API.Models.XianYu;

public class XianYuCallbackRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public long ExpiresIn { get; set; }
} 