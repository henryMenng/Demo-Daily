using System.ComponentModel.DataAnnotations;

namespace Daily.API.Dtos;

/// <summary>
/// 编辑待办事项Dto
/// </summary>
public class EditToDoDto
{
    /// <summary>
    /// 编辑的待办事项Id
    /// </summary>
    [Required]
    public int ToDoId { get; set; }

    /// <summary>
    /// 编辑的待办事项标题
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 编辑的待办事项内容
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 编辑的待办事项状态 0：未完成 1：已完成
    /// </summary>
    [Required]
    public int Status { get; set; }
}

