using System.ComponentModel.DataAnnotations;

namespace Daily.API.Dtos;

/// <summary>
/// 账号Dto，用来接收注册信息
/// </summary>
public class AccountInfoDto
{
    /// <summary>
    /// 姓名
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 账号
    /// </summary>
    [Required]
    public string Account { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    public string Pwd { get; set; } = string.Empty;
}
