using System.ComponentModel.DataAnnotations;

namespace Daily.API.Dtos;
/// <summary>
///待办中事项传输对象（还未完成的待办事项）
/// </summary>
public class ToDoingDto
{
    /// <summary>
    /// 待办中事项Id
    /// </summary>
    [Required]
    public int ToDoId { get; set; }

    /// <summary>
    /// 待办中事项标题
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 待办中事项内容
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

}
