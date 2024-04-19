using Prism.Mvvm;

namespace Daily.WPF.Dtos;
/// <summary>
/// 备忘录Dto
/// </summary>
public class MemoDto : BindableBase
{
    /// <summary>
    /// 备忘录Id
    /// </summary>

    private int _memoId;

    public int MemoId
    {
        get { return _memoId; }
        set { _memoId = value; RaisePropertyChanged(); }
    }

    /// <summary>
    /// 备忘录标题
    /// </summary>
    private string _title = string.Empty;

    public string Title
    {
        get { return _title; }
        set { _title = value; RaisePropertyChanged(); }
    }

    /// <summary>
    /// 备忘录内容
    /// </summary>
    private string _content = string.Empty;

    public string Content
    {
        get { return _content; }
        set { _content = value; RaisePropertyChanged(); }
    }

    /// <summary>
    /// 备忘录状态
    /// </summary>
    private int _status;

    public int Status
    {
        get { return _status; }
        set { _status = value; RaisePropertyChanged(); }
    }

}
