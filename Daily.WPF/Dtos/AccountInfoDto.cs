namespace Daily.WPF.Dtos;
/// <summary>
/// 账号Dto，用来请求注册
/// </summary>
public class AccountInfoDto
{
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 登录账号
    /// </summary>
    public string Account { get; set; } = string.Empty;

    /// <summary>
    /// 登录密码
    /// </summary>
    public string Pwd { get; set; } = string.Empty;

    /// <summary>
    /// 确认密码
    /// </summary>
    public string ConfirPwd { get; set; } = string.Empty;
}
