using System.Net.Http;

namespace Daily.WPF.HttpClients;
/// <summary>
/// Api请求模型
/// </summary>
public class ApiRequest
{
    /// <summary>
    /// 请求地址/api/路由地址
    /// </summary>
    public string Route { get; set; } = string.Empty;

    /// <summary>
    /// 请求方式（get/post/put/delete）
    /// </summary>
    public HttpMethod Method { get; set; } = default!;

    /// <summary>
    /// 请求参数
    /// </summary>
    public object Parameters { get; set; } = new();

    /// <summary>
    /// 发送的数据类型，请求的数据类型json
    /// </summary>
    public string ContentType { get; set; } = "application/json";


}
