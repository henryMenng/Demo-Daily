using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daily.API.DataModel;
/// <summary>
/// 待办事项信息
/// </summary>
[Table("ToDoInfo")]
public class ToDoInfo
{
    /// <summary>
    /// 待办事项Id
    /// </summary>
    [Key]
    public int ToDoId { get; set; }

    /// <summary>
    /// 待办事项标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 待办事项内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 待办事项状态 0：未完成 1：已完成
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 待办事项创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
}
