using Daily.API.ApiReponses;
using Daily.API.DataModel;
using Daily.API.Dtos;
using Daily.API.Helper;
using Daily.API.IServices;
using Daily.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Daily.API.Services;

/// <summary>
/// 实现账号相关的服务接口，如注册、登录等
/// </summary>
public class AccountService : IAccountService
{

    /// <summary>
    /// api响应帮助类,用于返回api响应
    /// </summary>
    private readonly IApiResponseHelper _apiResponseHelper;

    /// <summary>
    /// 账户仓储层服务
    /// </summary>
    private readonly IAccountRepoService _accountRepoService;

    /// <summary>
    /// 账号服务的构造函数
    /// </summary>
    /// <param name="apiResponseHelper">api响应帮助类</param>
    /// <param name="accountRepoService">账户仓储层服务</param>
    public AccountService(IApiResponseHelper apiResponseHelper, IAccountRepoService accountRepoService)
    {
        _apiResponseHelper = apiResponseHelper;
        _accountRepoService = accountRepoService;
    }

    /// <summary>
    /// 实现账号登录的服务。
    /// </summary>
    /// <param name="logAccount">需要登录的账号名。(查询字符串)</param>
    /// <param name="logPassword">账号对应的密码。(查询字符串)</param>
    /// <returns>返回一个ApiResponse对象，包含登录操作的结果信息。</returns>
    public async Task<ApiResponse> LoginAsync(string logAccount, string logPassword)
    {

        ApiResponse apiResponse = new();
        try
        {
            if (string.IsNullOrEmpty(logAccount) || string.IsNullOrEmpty(logPassword))
            {
                var result = await _accountRepoService.LoginAsync(logAccount, logPassword);
                if (result.Code == 10 && !result.Success)
                {
                    apiResponse.ResultCode = ResultCodeEnum.DtoError;
                    apiResponse.Msg = "账号或密码为空";
                    apiResponse.ResultData = null;
                    return apiResponse;
                }
            }
            var queryResult = await _accountRepoService.LoginAsync(logAccount, logPassword);

            if (queryResult.Code == 5)
            {
                apiResponse.ResultCode = ResultCodeEnum.NotFound;
                apiResponse.Msg = "账号或密码错误";
                apiResponse.ResultData = queryResult;
                return apiResponse;
            }

            if (queryResult.Code == 10)
            {
                apiResponse.ResultCode = ResultCodeEnum.DtoError;
                apiResponse.Msg = "用户信息不全或空";
                apiResponse.ResultData = queryResult;
                return apiResponse;
            }

            if (queryResult.Code == 0)
            {
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.Msg = "登录成功";
                apiResponse.ResultData = queryResult;
                return apiResponse;
            }
        }
        catch (Exception ex)
        {
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        return _apiResponseHelper.GenerateErrorResponse();
    }

    /// <summary>
    /// 实现账号注册的服务
    /// </summary>
    /// <param name="accountInfoDto">包含账号注册信心的Dto对象，包括账号名，密码等</param>
    /// <returns>返回一个ApiResponse对象，包含注册操作的结果信息</returns>
    public async Task<ApiResponse> RegisterAsync(AccountInfoDto accountInfoDto)
    {
        // 创建一个ApiResponse对象，用于返回注册操作的结果信息
        ApiResponse apiResponse = new();

        try
        {
            var accountInfo = new AccountInfo()
            {
                Account = accountInfoDto.Account,
                Pwd = accountInfoDto.Pwd,
                Name = accountInfoDto.Name,
            };

            var queryResult = await _accountRepoService.AddAsync(accountInfo);

            if (accountInfoDto == null || (queryResult.Code == 10 && !queryResult.Success))
            {
                apiResponse.ResultCode = ResultCodeEnum.DtoError;
                apiResponse.Msg = "服务器忙，请稍后...";
                apiResponse.ResultData = null;
                return apiResponse;
            }


            // 如果查询结果不为空，则返回账号已存在
            if (queryResult.Code == 5 && !queryResult.Success)
            {
                apiResponse.ResultCode = ResultCodeEnum.Exist;
                apiResponse.Msg = "账号已存在";
                apiResponse.ResultData = queryResult;
                return apiResponse;
            }

            if (queryResult.Code == 0 && queryResult.Success)
            {
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.Msg = "注册成功！";
                apiResponse.ResultData = queryResult;
                return apiResponse;
            }
        }
        catch (Exception ex)
        {
            // 如果发生异常，则返回异常错误,并使用api响应帮助类生成异常响应
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }

        return _apiResponseHelper.GenerateErrorResponse(); 
    }
}
