using System.ComponentModel.DataAnnotations;

namespace Daily.API.Dtos;
/// <summary>
/// 待办事项Dto数据传输对象（接收待办事项）
/// </summary>
public class AddToDoDto
{
    /// <summary>
    /// 待办事项标题
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 待办事项内容
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 待办事项状态 0：未完成 1：已完成
    /// </summary>
    [Required]
    public int Status { get; set; }
}

