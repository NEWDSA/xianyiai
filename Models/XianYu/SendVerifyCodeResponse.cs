namespace XianYu.API.Models.XianYu;

public class SendVerifyCodeResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string CaptchaUrl { get; set; } = string.Empty;
} 