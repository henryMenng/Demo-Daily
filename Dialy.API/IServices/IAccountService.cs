using Daily.API.ApiReponses;
using Daily.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Daily.API.IServices;

/// <summary>
/// 提供账号相关的服务接口，如注册、登录等
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// 提供账号注册的服务
    /// </summary>
    /// <param name="accountInfoDto">包含账号注册信息的Dto对象，包括账号名、密码等。</param>
    /// <returns>返回一个ApiResponse对象，包含注册操作的结果信息。</returns>
    Task<ApiResponse> RegisterAsync(AccountInfoDto accountInfoDto);

    /// <summary>
    /// 提供账号登录的服务。
    /// </summary>
    /// <param name="logAccount">需要登录的账号名。</param>
    /// <param name="logPassword">账号对应的密码。</param>
    /// <returns>返回一个ApiResponse对象，包含登录操作的结果信息。</returns>
    Task<ApiResponse> LoginAsync(string logAccount, string logPassword);
}
