namespace XianYu.API.Models.XianYu;

public class SendVerifyCodeRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string CountryCode { get; set; } = "86";
    public string PhoneCode { get; set; } = "86";
    public string CodeLength { get; set; } = "6";
    public string IsIframe { get; set; } = "true";
    public string DocumentReferer { get; set; } = string.Empty;
    public string DefaultView { get; set; } = string.Empty;
    public string Ua { get; set; } = string.Empty;
    public string UmidGetStatusVal { get; set; } = "255";
    public string ScreenPixel { get; set; } = string.Empty;
    public string NavLanguage { get; set; } = string.Empty;
    public string NavUserAgent { get; set; } = string.Empty;
    public string NavPlatform { get; set; } = string.Empty;
    public string AppName { get; set; } = "xianyu";
    public string AppEntrance { get; set; } = "web";
    public string BizParams { get; set; } = string.Empty;
    public string MainPage { get; set; } = "true";
    public string IsMobile { get; set; } = "false";
    public string Lang { get; set; } = "zh_cn";
    public string ReturnUrl { get; set; } = string.Empty;
    public string FromSite { get; set; } = "77";
    public string UmidTag { get; set; } = string.Empty;
    public string JsVersion { get; set; } = "2.5.22";
    public string DeviceId { get; set; } = string.Empty;
    public string PageTraceId { get; set; } = string.Empty;
    public string WeiBoMpBridge { get; set; } = "false";
    public string StyleType { get; set; } = "vertical";
    public string NotLoadSsoView { get; set; } = "false";
    public string NotKeepLogin { get; set; } = "false";
    public string QrCodeFirst { get; set; } = "false";
    public string RenderRefer { get; set; } = "mini_login.htm";
} 