using Daily.WPF.Dtos;
using Daily.WPF.Services;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Daily.WPF.ViewModels;
public class AddMemoUcViewModel : IDialogHostAware
{
    //对话框唯一标识
    private const string _dialogHostName = "RootDialog";

    public DelegateCommand SaveCommand { get; }

    public DelegateCommand CancelCommand { get; }

    public MemoDto MemoDto { get; set; } = new MemoDto();

    public AddMemoUcViewModel()
    {
        SaveCommand = new(Save);
        CancelCommand = new(Cancel);
    }

    private void Cancel()
    {
        //判断对话框是否打开
        var isOpenDia = DialogHost.IsDialogOpen(_dialogHostName);
        if (isOpenDia)
        {
            //关闭对话框,返回ButtonResult.No
            DialogHost.Close(_dialogHostName, new DialogResult(ButtonResult.No));
        }
    }

    private void Save()
    {
        //验证待办事项信息
        var verifyToDoInfo = string.IsNullOrEmpty(MemoDto.Title) || string.IsNullOrEmpty(MemoDto.Content);
        //如果标题或内容为空，弹出提示框，可以优化，不使用MessageBox
        if (verifyToDoInfo)
        {
            //弹出提示框
            var isOpenDialog = DialogHost.IsDialogOpen(_dialogHostName);
            if (isOpenDialog)
            {
                DialogParameters parasmeters = [];
                parasmeters.Add("msg", "标题和内容不能为空");
                DialogHost.Close(_dialogHostName, new DialogResult(ButtonResult.OK, parasmeters));
            }
            return;
        }

        //判断对话框是否打开
        var isOpenDia = DialogHost.IsDialogOpen(_dialogHostName);
        //如果对话框打开
        if (isOpenDia)
        {
            DialogParameters paras = [];
            //添加待办事项信息到对话框参数
            paras.Add("AddMemo", MemoDto);
            //关闭对话框,返回ButtonResult.Yes，同时返回参数ToDoInfoDto
            DialogHost.Close(_dialogHostName, new DialogResult(ButtonResult.OK, paras));
        }
    }

    public void OnDialogOpeneding(IDialogParameters parameters)
    {

    }
}
