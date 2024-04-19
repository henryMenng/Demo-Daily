namespace Daily.WPF.Dtos;
public class ToDoingDto
{
    /// <summary>
    /// 待办中事项Id
    /// </summary>
    public int ToDoId { get; set; }

    /// <summary>
    /// 待办中事项标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 待办中事项内容
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
