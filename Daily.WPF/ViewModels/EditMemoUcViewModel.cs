using Daily.WPF.Dtos;
using Daily.WPF.Services;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Daily.WPF.ViewModels;
public class EditMemoUcViewModel : BindableBase, IDialogHostAware
{
    private int _selectIndex;

    public int SelectIndex
    {
        get { return _selectIndex; }
        set { _selectIndex = value; RaisePropertyChanged(); }
    }


    //对话框唯一标识
    private const string _dialogHostName = "RootDialog";
    //备忘录Dto，用来添加待办事项
    public MemoDto MemoDto { get; set; } = new();
    //保存命令
    public DelegateCommand SaveCommand { get; }
    //取消命令
    public DelegateCommand CancelCommand { get; }

    //构造函数
    public EditMemoUcViewModel()
    {
        //初始化保存命令
        SaveCommand = new(Save);
        //初始化取消命令
        CancelCommand = new(Cancel);
    }

    //打开对话框时触发，可以接收参数
    public void OnDialogOpeneding(IDialogParameters parameters)
    {

    }
    //保存操作,保存数据,关闭对话框
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
            //用下拉框的值替换待办事项状态
            MemoDto.Status = SelectIndex;
            //添加待办事项信息到对话框参数
            paras.Add("EditMemoInfo", MemoDto);
            //关闭对话框,返回ButtonResult.Yes，同时返回参数ToDoInfoDto
            DialogHost.Close(_dialogHostName, new DialogResult(ButtonResult.OK, paras));
        }
    }
    //取消操作,关闭对话框
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
}
