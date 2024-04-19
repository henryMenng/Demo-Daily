using System.ComponentModel.DataAnnotations;

namespace Daily.API.Dtos;

/// <summary>
/// 登录数据传输对象
/// </summary>
public class LoginDto
{
    /// <summary>
    /// 登录账号
    /// </summary>
    [Required]
    public string LogAccount { get; set; } = string.Empty;

    /// <summary>
    /// 登录密码
    /// </summary>
    [Required]
    public string LogPassword { get; set; } = string.Empty;
}
