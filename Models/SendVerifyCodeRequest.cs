using System.Text.Json.Serialization;

namespace XianYu.API.Models;

public class SendVerifyCodeRequest
{
    [JsonPropertyName("mobile")]
    public string PhoneNumber { get; set; } = null!;
    
    [JsonPropertyName("phoneCode")]
    public string PhoneCode { get; set; } = "86";
    
    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; } = "CN";
    
    [JsonPropertyName("umidToken")]
    public string UmidToken { get; set; } = null!;
    
    [JsonPropertyName("ua")]
    public string UserAgent { get; set; } = null!;
    
    [JsonPropertyName("pageTraceId")]
    public string PageTraceId { get; set; } = null!;
    
    [JsonPropertyName("riskControlInfo")]
    public Dictionary<string, string> RiskControlInfo { get; set; } = new();
    
    [JsonPropertyName("umidTag")]
    public string UmidTag { get; set; } = "1";
    
    // 滑块验证相关
    public string? BxUa { get; set; }
    public string? BxUmidtoken { get; set; }
    public string? XCsrfToken { get; set; }
}

public class SendVerifyCodeResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
    [JsonPropertyName("data")]
    public SendVerifyCodeResponseData? Data { get; set; }
    
    [JsonPropertyName("ret")]
    public List<string>? Ret { get; set; }
}

public class SendVerifyCodeResponseData
{
    [JsonPropertyName("url")]
    public string? CaptchaUrl { get; set; }
    
    [JsonPropertyName("smsToken")]
    public string? SmsToken { get; set; }
} 