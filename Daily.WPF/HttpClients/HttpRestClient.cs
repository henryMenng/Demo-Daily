using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Daily.WPF.HttpClients;
/// <summary>
/// 调用api工具类
/// </summary>
public class HttpRestClient
{
    private readonly string _baseUrl = "http://localhost:5100/api/";

    /// <summary>
    /// 构造函数
    /// </summary>
    public HttpRestClient()
    {
    }

    /// <summary>
    /// 通过api请求并返回结果
    /// </summary>
    /// <param name="apiRequest">请求数据</param>
    /// <returns>返回的数据</returns>
    public async Task<ApiResponse?> Execute(ApiRequest apiRequest)
    {
        //响应实例
        ApiResponse? apiResponse = new();

        //序列化参数
        string jsonContent = JsonConvert.SerializeObject(apiRequest.Parameters);

        //http客户端
        using var httpClient = new HttpClient();

        //http请求
        using var httpRequestMessage = new HttpRequestMessage()
        {
            RequestUri = new Uri(_baseUrl + apiRequest.Route),
            Method = apiRequest.Method,
            Content = new StringContent(jsonContent, Encoding.UTF8, apiRequest.ContentType)
        };

        try
        {
            //发送请求
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            //判断请求是否成功
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                //获取响应内容
                string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                //反序列化响应内容
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                return apiResponse;
            }
            else
            {
                apiResponse.ResultCode = -99;
                apiResponse.Msg = "服务器忙，请稍后";

                return apiResponse;
            }
        }
        catch (Exception ex)
        {
            //处理异常
            apiResponse ??= new();
            apiResponse.ResultCode = -99;
            apiResponse.Msg = "请求发生异常：" + ex.Message;

            return apiResponse;
        }
    }
}
