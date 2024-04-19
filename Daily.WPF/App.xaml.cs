using Daily.WPF.HttpClients;
using Daily.WPF.Services;
using Daily.WPF.ViewModels;
using Daily.WPF.Views;
using Daily.WPF.Views.Dialog;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System.Windows;

namespace Daily.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    //第二执行
    protected override Window CreateShell()
    {
        var mainWin = Container.Resolve<MainWin>();

        return mainWin;
    }

    //最先执行
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        //注入主界面 主界面不用注入
        //containerRegistry.RegisterForNavigation<MainWin>();
        //注入区域
        containerRegistry.RegisterForNavigation<HomeUc, HomeUcViewModel>();
        //注入待办事项区域
        containerRegistry.RegisterForNavigation<ToDoUc, ToDoUcViewModel>();
        //注入备忘录区域
        containerRegistry.RegisterForNavigation<MemoUc, MemoUcViewModel>();
        //注入设置区域
        containerRegistry.RegisterForNavigation<SettingsUc, SettingsUcViewModel>();
        //注入关于区域
        containerRegistry.RegisterForNavigation<AboutUc, AboutUcViewModel>();
        //注入个性化区域
        containerRegistry.RegisterForNavigation<PersonalUc, PersonalUcViewModel>();
        //注入系统设置区域
        containerRegistry.RegisterForNavigation<SysSetUc>();
        //注入登录对话框界面
        containerRegistry.RegisterDialog<LogUC, LogUcViewModel>();
        //注入添加待办事项对话框界面,这个个自定义对话框
        containerRegistry.RegisterForNavigation<AddToDoUc, AddToDoUcViewModel>();
        //注入添加待备忘录对话框界面,这个个自定义对话框
        containerRegistry.RegisterForNavigation<AddMemoUc, AddMemoUcViewModel>();
        //注入编辑待办事项对话框界面
        containerRegistry.RegisterForNavigation<EditToDoUc, EditToDoUcViewModel>();
        //注入编辑备忘录对话框界面
        containerRegistry.RegisterForNavigation<EditMemoUc, EditMemoUcViewModel>();
        //注入自定义对话框服务
        containerRegistry.Register<DialogHostService>();

        //注入请求
        //made：底层是用一个工厂来建造的
        //containerRegistry.GetContainer().Register<HttpRestClient>(made:Parameters.Of.Type<string>(serviceKey:"webUrl"));
        containerRegistry.GetContainer().Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
    }
    //第三执行
    protected override void OnInitialized()
    {
        var dialog = Container.Resolve<IDialogService>();

        dialog.ShowDialog(nameof(LogUC), callback =>
        {
            if (callback.Result != ButtonResult.OK)
            {
                Environment.Exit(0);
                return;
            }
            var mainWinViewModel = App.Current.MainWindow.DataContext as MainWinViewModel;
            if (mainWinViewModel != null)
            {

                if (callback.Parameters.ContainsKey("msg"))
                {
                    mainWinViewModel.LogMsg = callback.Parameters.GetValue<string>("msg");
                    if (callback.Parameters.ContainsKey("accountName"))
                    {
                        mainWinViewModel.LogName = callback.Parameters.GetValue<string>("accountName");
                        mainWinViewModel.ChangeUserControlCom.Execute("HomeUc");
                        mainWinViewModel.MsgQueue.Enqueue("欢迎使用Daily");
                    }
                }
            }
        });
        base.OnInitialized();
    }
}
