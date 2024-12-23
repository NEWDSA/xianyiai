namespace XianYu.API.Infrastructure.XianYu;

public class XianYuOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string LoginUrl { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string AppKey { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
    public string JsVersion { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string TokenEnc { get; set; } = string.Empty;
    public string Cookie { get; set; } = string.Empty;
    public string CsrfToken { get; set; } = string.Empty;
    public string UmidToken { get; set; } = string.Empty;
    public string BxUa { get; set; } = string.Empty;
    public string BxUmidtoken { get; set; } = string.Empty;
    public string Ua { get; set; } = string.Empty;
    public string EagleEyeTraceId { get; set; } = string.Empty;
    public string EagleEyeSessionId { get; set; } = string.Empty;
    public string EagleEyePAppName { get; set; } = string.Empty;
}
