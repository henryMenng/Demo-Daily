using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daily.API.DataModel;
/// <summary>
/// 登录账号数据模型
/// </summary>
[Table("AccountInfo")]
public class AccountInfo
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [Key]//主键
    public int AccountId { get; set; }

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
}
