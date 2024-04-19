using System.ComponentModel.DataAnnotations;

namespace Daily.API.Dtos;

/// <summary>
/// 备忘录信息Dto
/// </summary>
public class MemoInfoDto
{
    /// <summary>
    /// 备忘录Id
    /// </summary>
    [Required]
    public int MemoId { get; set; }

    /// <summary>
    /// 备忘录标题
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 备忘录内容
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 备忘录状态 0：未完成 1：已完成
    /// </summary>
    [Required]
    public int Status { get; set; }
}

