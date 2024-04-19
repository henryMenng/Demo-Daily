namespace Daily.API.ApiReponses;

/// <summary>
/// 响应/接收模型
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// 结果码
    /// </summary>
    public ResultCodeEnum ResultCode { get; set; }

    /// <summary>
    /// 结果信息，可以展示给用户
    /// </summary>
    public string Msg { get; set; } = string.Empty;

    /// <summary>
    /// 结果数据
    /// </summary>
    public object? ResultData { get; set; } = new();
}

/// <summary>
/// 结果码枚举1-成功，-99-异常错误，2-账号已存在，-2-账号或密码不正确，-3-传输的数据异常，-4-数据库异常
/// </summary>
public enum ResultCodeEnum
{
    /// <summary>
    /// 成功
    /// </summary>
    Success = 1,

    /// <summary>
    /// 异常错误
    /// </summary>
    Error = -99,

    /// <summary>
    /// 账号已存在
    /// </summary>
    Exist = 2,

    /// <summary>
    /// 账号或密码不正确
    /// </summary>
    NotFound = -2,
    // Add more result codes as needed

    /// <summary>
    /// 传输的数据异常
    /// </summary>
    DtoError = -3,

    /// <summary>
    /// 数据库异常
    /// </summary>
    DataBaseError = -4,

    /// <summary>
    /// Dto信息不全
    /// </summary>
    DtoNotComplete = -5

}
