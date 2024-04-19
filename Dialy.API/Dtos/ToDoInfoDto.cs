using System.ComponentModel.DataAnnotations;

namespace Daily.API.Dtos;
/// <summary>
/// 全部待办事项信息数据传输对象
/// </summary>
public class ToDoInfoDto
{
    /// <summary>
    /// 待办事项Id
    /// </summary>
    [Required]
    public int ToDoId { get; set; }

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
