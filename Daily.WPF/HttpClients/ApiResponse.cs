namespace Daily.WPF.HttpClients;
/// <summary>
/// 响应/接收模型
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// 结果码
    /// </summary>
    public int ResultCode { get; set; }

    /// <summary>
    /// 结果信息
    /// </summary>
    public string Msg { get; set; } = string.Empty;

    /// <summary>
    /// 结果数据
    /// </summary>
    public object ResultData { get; set; } = new();
}
