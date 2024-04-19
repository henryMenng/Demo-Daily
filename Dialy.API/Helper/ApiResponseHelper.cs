using Daily.API.ApiReponses;

namespace Daily.API.Helper;

/// <summary>
/// 实现用于处理异常响应信息，帮助类接口
/// </summary>
public class ApiResponseHelper : IApiResponseHelper
{
    /// <summary>
    /// 实现生成异常响应信息
    /// </summary>
    /// <param name="ex">异常</param>
    /// <returns>返回一个ApiResponse类型包括异常信息等</returns>
    public ApiResponse GenerateErrorResponse(Exception ex)
    {
        var apiResponse = new ApiResponse
        {
            ResultCode = ResultCodeEnum.Error,
            Msg = "服务器忙，请稍后...",
            ResultData = ex,
        };
        return apiResponse;
    }

    /// <summary>
    /// 实现生成异常响应信息
    /// </summary>
    /// <returns>业务逻辑异常</returns>
    public ApiResponse GenerateErrorResponse()
    {
        var apiResponse = new ApiResponse
        {
            ResultCode = ResultCodeEnum.Error,
            Msg = "服务器忙，请稍后...",
            ResultData = new object(),
        };
        return apiResponse;
    }
}
