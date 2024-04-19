
using System.ComponentModel.DataAnnotations;

namespace Daily.API.Dtos;
/// <summary>
/// 编辑备忘录Dto
/// </summary>
public class EditMemoDto
{
    /// <summary>
    /// 编辑的备忘录Id
    /// </summary>
    [Required]
    public int MemoId { get; set; }

    /// <summary>
    /// 编辑的备忘录标题
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 编辑的备忘录内容
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 编辑的备忘录状态 0：未完成 1：已完成
    /// </summary>
    [Required]
    public int Status { get; set; }
}

