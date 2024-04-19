using System.ComponentModel.DataAnnotations;

namespace Daily.API.Dtos;
/// <summary>
/// 更新待办状态数据传输对象
/// </summary>
public class UpdateToDoStatusDto
{
    /// <summary>
    /// 更新待办事项状态Id
    /// </summary>
    [Required]
    public int UpdateToDoStatusId { get; set; }
}

