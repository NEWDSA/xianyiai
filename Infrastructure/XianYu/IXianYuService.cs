using XianYu.API.Models.XianYu;

namespace XianYu.API.Infrastructure.XianYu;

public interface IXianYuService
{
    /// <summary>
    /// 创建二维码登录
    /// </summary>
    Task<QrCodeLoginResponse> CreateQrCodeAsync();

    /// <summary>
    /// 检查二维码登录状态
    /// </summary>
    Task<QrCodeLoginResponse> CheckQrCodeStatusAsync(string qrCodeId);

    /// <summary>
    /// 使用授权码进行认证
    /// </summary>
    Task<bool> AuthenticateAsync(string code);

    /// <summary>
    /// 刷新访问令牌
    /// </summary>
    Task<string> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// 同步订单数据
    /// </summary>
    Task<bool> SyncOrdersAsync();

    /// <summary>
    /// 更新订单状态
    /// </summary>
    Task<bool> UpdateOrderStatusAsync(string orderId, string status);

    /// <summary>
    /// 向买家发送消息
    /// </summary>
    Task<bool> SendMessageAsync(string buyerId, string message);

    /// <summary>
    /// 获取账户余额
    /// </summary>
    Task<decimal> GetBalanceAsync();

    /// <summary>
    /// 获取待处理订单
    /// </summary>
    Task<List<XianYuOrder>> GetPendingOrdersAsync();

    /// <summary>
    /// 登录
    /// </summary>
    Task<string> LoginAsync(string username, string password);

    /// <summary>
    /// 发送验证码
    /// </summary>
    Task<bool> SendVerifyCodeAsync(string phoneNumber);

    /// <summary>
    /// 使用验证码登录
    /// </summary>
    Task<string> LoginWithVerifyCodeAsync(string phoneNumber, string verifyCode);
}
