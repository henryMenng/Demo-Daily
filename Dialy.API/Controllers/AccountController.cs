using Daily.API.ApiReponses;
using Daily.API.DataModel;
using Daily.API.Dtos;
using Daily.API.Filters;
using Daily.API.IServices;
using Daily.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Daily.API.Controllers;
/// <summary>
/// 账号控制器
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    /// <summary>
    /// 账号服务
    /// </summary>
    private readonly IAccountService _accountService;

    /// <summary>
    /// AccountController的构造函数
    /// </summary>
    /// <param name="accountService">依赖注入的账号服务</param>
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// 登录 HttpGet请求，使用查询字符串
    /// </summary>
    /// <param name="logAccount">登录账号</param>
    /// <param name="logPassword">登录密码</param>
    /// <returns>返回一个IActionResult类型的包括登录结果的信息</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> Login(string logAccount, string logPassword) =>
         Ok(await _accountService.LoginAsync(logAccount, logPassword));

    /// <summary>
    /// 注册 HttpPost请求，使用请求体，传递注册信息
    /// </summary>
    /// <param name="accountInfoDto">注册信息</param>
    /// <returns>返回一个IActionResult类型的包括注册结果的的信息</returns>
    [HttpPost]
    [ValidateModelState]
    public async Task<ActionResult<ApiResponse>> Register(AccountInfoDto accountInfoDto) => Ok(await _accountService.RegisterAsync(accountInfoDto));

        
}
