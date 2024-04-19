using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daily.API.DataModel;
/// <summary>
/// 备忘录信息
/// </summary>
[Table("MemoInfo")]
public class MemoInfo
{
    /// <summary>
    /// 备忘录Id
    /// </summary>
    [Key]
    public int MemoId { get; set; }

    /// <summary>
    /// 备忘录标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 备忘录内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 备忘录状态 0：未完成 1：已完成
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 备忘录创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
}
