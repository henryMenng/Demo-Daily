using MaterialDesignThemes.Wpf;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Windows;

namespace Daily.WPF.Services;
public class DialogHostService : DialogService
{
    private readonly IContainerExtension _containerExtension;

    public DialogHostService(IContainerExtension containerExtension) : base(containerExtension)
    {
        _containerExtension = containerExtension;
    }

    //对话框主机服务的打开对话框方法
    public async Task<IDialogResult> ShowDialog(string name, IDialogParameters parameters, string dialogHostName = "Root")
    {
        parameters ??= new DialogParameters();

        //从容器窗口去除弹出的对话框实例
        var content = _containerExtension.Resolve<object>(name);

        //验证实例的有效性，是否为对话框
        if (content is not FrameworkElement dialogContent)
            throw new NullReferenceException("A dialog's content must be a FrameworkElement");
        //验证对话框的内容是否是FrameworkElement类型，且数据上下文是否为空，且ViewModelLocator.GetAutoWireViewModel(view)是否为空
        if (dialogContent is FrameworkElement view && view.DataContext is null && ViewModelLocator.GetAutoWireViewModel(view) is null)
            ViewModelLocator.SetAutoWireViewModel(view, true);
        //验证对话框的内容数据上下文（一般时ViewModel）是否实现了IDialogHostAware接口
        if (dialogContent.DataContext is not IDialogHostAware viewModel)
            throw new NullReferenceException("A dialog's content must implement IDialogHostAware");
        //对话框打开事件，打开对话框时触发
        DialogOpenedEventHandler eventHandler = (sender, eventArgs) =>
        {
            if (viewModel is IDialogHostAware dialogHostAware)
            {
                dialogHostAware.OnDialogOpeneding(parameters);
            }
            eventArgs.Session.UpdateContent(dialogContent);
        };

        //验证对话框的结果后返回
        return await DialogHost.Show(dialogContent, dialogHostName, eventHandler) as IDialogResult ?? throw new NullReferenceException("A dialog's result must be a DialogResult");
    }
}
