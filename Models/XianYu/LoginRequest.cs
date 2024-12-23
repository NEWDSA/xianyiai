namespace XianYu.API.Models.XianYu;

public class LoginRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string VerifyCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = "86";
    public string Type { get; set; } = "login";
    public string Ttid { get; set; } = "h5@mobile_h5_2.0.0";
    public string AppName { get; set; } = "xianyu";
    public string AppEntrance { get; set; } = "taobao_h5";
    public string FromSite { get; set; } = "77";
} 