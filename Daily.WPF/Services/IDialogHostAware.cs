using Prism.Commands;
using Prism.Services.Dialogs;

namespace Daily.WPF.Services;
internal interface IDialogHostAware
{
    /// <summary>
    /// 打开对话框时触发
    /// </summary>
    void OnDialogOpeneding(IDialogParameters parameters);

    /// <summary>
    /// 确认保存时触发
    /// </summary>
    DelegateCommand SaveCommand { get; }

    /// <summary>
    /// 取消操作时触发
    /// </summary>
    DelegateCommand CancelCommand { get; }


}
