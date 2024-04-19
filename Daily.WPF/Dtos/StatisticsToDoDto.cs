namespace Daily.WPF.Dtos;
/// <summary>
/// 接收API返回的统计待办事项Dto
/// </summary>
public class StatisticsToDoDto
{
    /// <summary>
    /// 统计待办事项总数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 统计待办事项已完成数
    /// </summary>
    public int Completed { get; set; }

    /// <summary>
    /// 统计待办事项完成百分比
    /// </summary>
    public string CompletedPercent { get; set; } = string.Empty;
}
