using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using XianYu.API.Models;
using XianYu.API.Options;

namespace XianYu.API.Services;

public class XianYuAuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<XianYuAuthService> _logger;
    private readonly CookieContainer _cookieContainer;
    private readonly string _baseUrl = "https://passport.goofish.com";

    public XianYuAuthService(
        HttpClient httpClient,
        ILogger<XianYuAuthService> logger)
    {
        _cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = _cookieContainer,
            UseCookies = true,
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        
        _httpClient = new HttpClient(handler);
        _httpClient.BaseAddress = new Uri(_baseUrl);
        
        // 设置默认请求头
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-CN"));
        _httpClient.DefaultRequestHeaders.Add("Origin", "https://passport.goofish.com");
        _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
        _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
        _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
        
        _logger = logger;
    }

    public async Task<SendVerifyCodeResponse> SendVerifyCodeAsync(SendVerifyCodeRequest request)
    {
        try
        {
            // 构建请求内容
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["mobile"] = request.PhoneNumber,
                ["phoneCode"] = request.PhoneCode,
                ["countryCode"] = request.CountryCode,
                ["umidToken"] = request.UmidToken,
                ["ua"] = request.UserAgent,
                ["pageTraceId"] = request.PageTraceId,
                ["riskControlInfo"] = JsonSerializer.Serialize(request.RiskControlInfo),
                ["umidTag"] = request.UmidTag,
                // 添加更多必要参数
                ["appName"] = "xianyu",
                ["appEntrance"] = "web",
                ["styleType"] = "vertical",
                ["bizParams"] = "",
                ["notLoadSsoView"] = "false",
                ["notKeepLogin"] = "false",
                ["isMobile"] = "false",
                ["qrCodeFirst"] = "false",
                ["fromSite"] = "77"
            });

            // 设置请求头
            using var request = new HttpRequestMessage(HttpMethod.Post, "/newlogin/sms/send.do");
            request.Content = content;
            
            // 添加必要的请求头
            request.Headers.Add("User-Agent", request.UserAgent);
            request.Headers.Add("Referer", "https://passport.goofish.com/mini_login.htm");
            
            // 添加滑块验证相关的请求头
            if (!string.IsNullOrEmpty(request.BxUa))
            {
                request.Headers.Add("bx-ua", request.BxUa);
            }
            if (!string.IsNullOrEmpty(request.BxUmidtoken))
            {
                request.Headers.Add("bx-umidtoken", request.BxUmidtoken);
            }
            if (!string.IsNullOrEmpty(request.XCsrfToken))
            {
                request.Headers.Add("x-csrf-token", request.XCsrfToken);
            }

            // 发送请求
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Send verify code response: {Response}", responseContent);
            
            // 解析响应
            var result = JsonSerializer.Deserialize<SendVerifyCodeResponse>(responseContent);
            if (result == null)
            {
                throw new Exception("Failed to parse response");
            }

            // 处理需要滑块验证的情况
            if (result.Ret?.Contains("FAIL_SYS_USER_VALIDATE") == true)
            {
                result.Success = false;
                result.Message = "需要完成滑块验证码";
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verify code");
            throw;
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            // 构建请求内容
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["loginId"] = request.PhoneNumber,
                ["phoneCode"] = request.PhoneCode,
                ["countryCode"] = request.CountryCode,
                ["smsCode"] = request.VerifyCode,
                ["umidToken"] = request.UmidToken,
                ["ua"] = request.UserAgent,
                ["pageTraceId"] = request.PageTraceId,
                ["riskControlInfo"] = JsonSerializer.Serialize(request.RiskControlInfo),
                ["umidTag"] = request.UmidTag,
                // 添加更多必要参数
                ["appName"] = "xianyu",
                ["appEntrance"] = "web",
                ["styleType"] = "vertical",
                ["bizParams"] = "",
                ["notLoadSsoView"] = "false",
                ["notKeepLogin"] = "false",
                ["isMobile"] = "false",
                ["qrCodeFirst"] = "false",
                ["fromSite"] = "77"
            });

            // 设置请求头
            using var request = new HttpRequestMessage(HttpMethod.Post, "/newlogin/login.do");
            request.Content = content;
            
            // 添加必要的请求头
            request.Headers.Add("User-Agent", request.UserAgent);
            request.Headers.Add("Referer", "https://passport.goofish.com/mini_login.htm");
            
            // 添加滑块验证相关的请求头
            if (!string.IsNullOrEmpty(request.BxUa))
            {
                request.Headers.Add("bx-ua", request.BxUa);
            }
            if (!string.IsNullOrEmpty(request.BxUmidtoken))
            {
                request.Headers.Add("bx-umidtoken", request.BxUmidtoken);
            }
            if (!string.IsNullOrEmpty(request.XCsrfToken))
            {
                request.Headers.Add("x-csrf-token", request.XCsrfToken);
            }

            // 发送请求
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Login response: {Response}", responseContent);
            
            // 解析响应
            var result = JsonSerializer.Deserialize<LoginResponse>(responseContent);
            if (result == null)
            {
                throw new Exception("Failed to parse response");
            }

            // 处理需要滑块验证的情况
            if (result.Ret?.Contains("FAIL_SYS_USER_VALIDATE") == true)
            {
                result.Success = false;
                result.Message = "需要完成滑块验证码";
            }

            // 如果登录成功，保存cookies
            if (result.Success && result.Data?.Cookies != null)
            {
                foreach (var cookie in result.Data.Cookies)
                {
                    _cookieContainer.SetCookies(new Uri(_baseUrl), cookie);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to login");
            throw;
        }
    }
}
