namespace Daily.WPF.Models
{
    /// <summary>
    ///备忘录信息Dto
    /// </summary>
    public class MemoInfo
    {
        /// <summary>
        /// 备忘录Id
        /// </summary>
        public int MemoId { get; set; }

        /// <summary>
        ///备忘录标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 备忘录内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 备忘录状态
        /// </summary>
        public int Status { get; set; }
    }
}
