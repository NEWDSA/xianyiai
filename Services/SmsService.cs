using AlibabaCloud.SDK.Dysmsapi20170525;
using AlibabaCloud.SDK.Dysmsapi20170525.Models;
using Tea;

namespace XianYu.API.Services;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string templateCode, Dictionary<string, string> templateParams);
}

public class SmsService : ISmsService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;
    private readonly Client _client;

    public SmsService(
        IConfiguration configuration,
        ILogger<SmsService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var accessKeyId = configuration["Aliyun:AccessKeyId"];
        var accessKeySecret = configuration["Aliyun:AccessKeySecret"];
        var endpoint = "dysmsapi.aliyuncs.com";

        var config = new AlibabaCloud.OpenApiClient.Models.Config
        {
            AccessKeyId = accessKeyId,
            AccessKeySecret = accessKeySecret,
            Endpoint = endpoint
        };

        _client = new Client(config);
    }

    public async Task SendSmsAsync(string phoneNumber, string templateCode, Dictionary<string, string> templateParams)
    {
        try
        {
            var request = new SendSmsRequest
            {
                PhoneNumbers = phoneNumber,
                SignName = _configuration["Aliyun:SignName"],
                TemplateCode = templateCode,
                TemplateParam = System.Text.Json.JsonSerializer.Serialize(templateParams)
            };

            var response = await _client.SendSmsAsync(request);
            if (response.Body.Code != "OK")
            {
                throw new Exception($"发送短信失败: {response.Body.Message}");
            }

            _logger.LogInformation(
                "已发送短信到 {PhoneNumber}, 模板: {TemplateCode}, 参数: {Params}",
                phoneNumber,
                templateCode,
                templateParams);
        }
        catch (TeaException ex)
        {
            _logger.LogError(ex, "发送短信到 {PhoneNumber} 时发生错误", phoneNumber);
            throw;
        }
    }
} 