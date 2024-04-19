using Daily.API.ApiReponses;

namespace Daily.API.Helper;

/// <summary>
/// 提供用于处理异常响应信息，帮助类接口
/// </summary>
public interface IApiResponseHelper
{
    /// <summary>
    /// 提供生成异常响应信息
    /// </summary>
    /// <param name="ex">异常</param>
    /// <returns>返回一个ApiResponse类型包括异常信息等</returns>
    ApiResponse GenerateErrorResponse(Exception ex);

    /// <summary>
    /// 提供生成异常响应信息,普通异常
    /// </summary>
    /// <returns>返回ApiResponse</returns>
    ApiResponse GenerateErrorResponse();
}
