using System.Text.Json;
using Microsoft.Extensions.Options;
using XianYu.API.Models.XianYu;
using XianYu.API.Services;

namespace XianYu.API.Infrastructure.XianYu;

public class XianYuQrCodeService : IXianYuQrCodeService
{
    private readonly HttpClient _httpClient;
    private readonly XianYuOptions _options;
    private readonly ILogger<XianYuQrCodeService> _logger;

    public XianYuQrCodeService(
        HttpClient httpClient,
        IOptions<XianYuOptions> options,
        ILogger<XianYuQrCodeService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<QrCodeLoginResponse> GetQrCodeAsync()
    {
        try
        {
            // 设置请求头
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _options.UserAgent);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8,zh-TW;q=0.7,zh-HK;q=0.5,en-US;q=0.3,en;q=0.2");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            _httpClient.DefaultRequestHeaders.Add("bx-v", "2.5.22");
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://passport.goofish.com");
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.Add("Referer", "https://passport.goofish.com/mini_login.htm?lang=zh_cn&appName=xianyu&appEntrance=web&styleType=vertical&bizParams=&notLoadSsoView=false&notKeepLogin=false&isMobile=false&qrCodeFirst=false&stie=77&rnd=0.2883010251021353");
            _httpClient.DefaultRequestHeaders.Add("EagleEye-TraceID", _options.EagleEyeTraceId);
            _httpClient.DefaultRequestHeaders.Add("EagleEye-SessionID", _options.EagleEyeSessionId);
            _httpClient.DefaultRequestHeaders.Add("EagleEye-pAppName", _options.EagleEyePAppName);
            _httpClient.DefaultRequestHeaders.Add("DNT", "1");
            _httpClient.DefaultRequestHeaders.Add("Sec-GPC", "1");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            _httpClient.DefaultRequestHeaders.Add("Priority", "u=0");
            _httpClient.DefaultRequestHeaders.Add("TE", "trailers");

            // 处理cookie
            if (!string.IsNullOrEmpty(_options.Cookie))
            {
                _httpClient.DefaultRequestHeaders.Add("Cookie", _options.Cookie);
            }

            // 构建请求参数
            var parameters = new Dictionary<string, string>
            {
                { "appName", "xianyu" },
                { "fromSite", "77" },
                { "umidToken", _options.UmidToken },
                { "ua", _options.Ua },
                { "bx-ua", _options.BxUa },
                { "bx-umidtoken", _options.BxUmidtoken }
            };

            var content = new FormUrlEncodedContent(parameters);

            // 发送请求
            var response = await _httpClient.PostAsync($"{_options.ApiUrl}/newlogin/qrcode/generate.do?appName=xianyu&fromSite=77", content);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("获取二维码失败: {StatusCode}", response.StatusCode);
                return new QrCodeLoginResponse
                {
                    Success = false,
                    Message = "获取二维码失败"
                };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("获取二维码响应: {Response}", responseContent);

            var result = JsonSerializer.Deserialize<QrCodeLoginResponse>(responseContent);
            if (result == null)
            {
                return new QrCodeLoginResponse
                {
                    Success = false,
                    Message = "解析响应失败"
                };
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取二维码时发生错误");
            return new QrCodeLoginResponse
            {
                Success = false,
                Message = "获取二维码时发生错误"
            };
        }
    }

    public async Task<QrCodeLoginResponse> CheckQrCodeStatusAsync(string token)
    {
        try
        {
            // 设置请求头
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _options.UserAgent);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8,zh-TW;q=0.7,zh-HK;q=0.5,en-US;q=0.3,en;q=0.2");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            _httpClient.DefaultRequestHeaders.Add("bx-v", "2.5.22");
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://passport.goofish.com");
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.Add("Referer", "https://passport.goofish.com/mini_login.htm?lang=zh_cn&appName=xianyu&appEntrance=web&styleType=vertical&bizParams=&notLoadSsoView=false&notKeepLogin=false&isMobile=false&qrCodeFirst=false&stie=77&rnd=0.2883010251021353");
            _httpClient.DefaultRequestHeaders.Add("EagleEye-TraceID", _options.EagleEyeTraceId);
            _httpClient.DefaultRequestHeaders.Add("EagleEye-SessionID", _options.EagleEyeSessionId);
            _httpClient.DefaultRequestHeaders.Add("EagleEye-pAppName", _options.EagleEyePAppName);
            _httpClient.DefaultRequestHeaders.Add("DNT", "1");
            _httpClient.DefaultRequestHeaders.Add("Sec-GPC", "1");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            _httpClient.DefaultRequestHeaders.Add("Priority", "u=0");
            _httpClient.DefaultRequestHeaders.Add("TE", "trailers");

            // 处理cookie
            if (!string.IsNullOrEmpty(_options.Cookie))
            {
                _httpClient.DefaultRequestHeaders.Add("Cookie", _options.Cookie);
            }

            // 构建请求参数
            var parameters = new Dictionary<string, string>
            {
                { "token", token },
                { "appName", "xianyu" },
                { "fromSite", "77" },
                { "umidToken", _options.UmidToken },
                { "ua", _options.Ua },
                { "bx-ua", _options.BxUa },
                { "bx-umidtoken", _options.BxUmidtoken }
            };

            var content = new FormUrlEncodedContent(parameters);

            // 发送请求
            var response = await _httpClient.PostAsync($"{_options.ApiUrl}/newlogin/qrcode/query.do?appName=xianyu&fromSite=77", content);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("查询二维码状态失败: {StatusCode}", response.StatusCode);
                return new QrCodeLoginResponse
                {
                    Success = false,
                    Message = "查询二维码状态失败"
                };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("查询二维码状态响应: {Response}", responseContent);

            var result = JsonSerializer.Deserialize<QrCodeLoginResponse>(responseContent);
            if (result == null)
            {
                return new QrCodeLoginResponse
                {
                    Success = false,
                    Message = "解析响应失败"
                };
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询二维码状态时发生错误");
            return new QrCodeLoginResponse
            {
                Success = false,
                Message = "查询二维码状态时发生错误"
            };
        }
    }
}
