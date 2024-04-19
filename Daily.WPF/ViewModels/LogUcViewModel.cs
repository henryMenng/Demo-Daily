using Daily.WPF.Dtos;
using Daily.WPF.Events;
using Daily.WPF.Helper;
using Daily.WPF.HttpClients;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Net.Http;

namespace Daily.WPF.ViewModels;

// 日志用户控件视图模型
public class LogUcViewModel : BindableBase, IDialogAware
{
    #region Fields

    private readonly HttpRestClient _httpRestClient;

    private readonly IEventAggregator _eventAggregator;

    #endregion

    #region Properties

    // 对话框标题
    public string Title { get; set; } = "我的日常";

    // 关闭事件
    public event Action<IDialogResult> RequestClose = default!;

    // 登录命令
    public DelegateCommand LogCom { get; set; }

    // 注册命令
    public DelegateCommand RegCom { get; set; }

    // 显示注册页面还是登录页面命令
    public DelegateCommand ShowLogOrRegCom { get; set; }

    private AccountInfoDto _accountInfoDto = new();

    // 账户信息数据传输对象
    public AccountInfoDto AccountInfoDto
    {
        get { return _accountInfoDto; }
        set
        {
            _accountInfoDto = value;
            RaisePropertyChanged();
        }
    }

    private int _logOrReg;

    // 登录或注册标志
    public int LogOrReg
    {
        get { return _logOrReg; }
        set
        {
            _logOrReg = value;
            RaisePropertyChanged();
        }
    }

    private LoginDto _loginDto = new();

    // 登录数据传输对象
    public LoginDto LoginDto
    {
        get { return _loginDto; }
        set
        {
            _loginDto = value;
            RaisePropertyChanged();
        }
    }


    private SnackbarMessageQueue _msgQueue = new();

    public SnackbarMessageQueue MsgQueue
    {
        get { return _msgQueue; }
        set { _msgQueue = value; }
    }


    #endregion

    #region Constructor

    public LogUcViewModel(HttpRestClient httpRestClient, IEventAggregator eventAggregator)
    {
        LogCom = new(Log);

        ShowLogOrRegCom = new(ShowLogOrReg);

        RegCom = new(async () => await Reg());

        _httpRestClient = httpRestClient;

        // 事件聚合器
        _eventAggregator = eventAggregator;

        // 订阅了一个行为
        _eventAggregator.GetEvent<SnackbarMsgEvent>().Subscribe(SubMessage);
    }


    #endregion

    #region Methods

    /// <summary>
    /// 行为，执行业务
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void SubMessage(string obj)
    {
        //把消息放到消息队列
        MsgQueue.Enqueue(obj);
    }



    // 切换登录或注册页面
    private void ShowLogOrReg()
    {
        LogOrReg = LogOrReg == 0 ? 1 : 0;
    }

    // 注册方法
    private async Task Reg()
    {
        if (string.IsNullOrEmpty(AccountInfoDto.Name) ||
            string.IsNullOrEmpty(AccountInfoDto.Account) ||
            string.IsNullOrEmpty(AccountInfoDto.Pwd) ||
            string.IsNullOrEmpty(AccountInfoDto.ConfirPwd))
        {
            //MessageBox.Show("注册信息不全");
            _eventAggregator.GetEvent<SnackbarMsgEvent>().Publish("注册信息不全");
            return;
        }

        if (AccountInfoDto.Pwd != AccountInfoDto.ConfirPwd)
        {
            //MessageBox.Show("两次密码不一致");
            _eventAggregator.GetEvent<SnackbarMsgEvent>().Publish("两次密码不一致");
            return;
        }

        // 密码加密
        AccountInfoDto.Pwd = PasswordMd5.Md5(AccountInfoDto.Pwd);

        // 调用API
        ApiRequest apiRequest = new()
        {
            Method = HttpMethod.Post,
            Route = "Account/Register",
            Parameters = AccountInfoDto
        };

        ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest); // 请求API

        if (apiResponse != null)
        {
            if (apiResponse.ResultCode == 1)
            {
                //MessageBox.Show(apiResponse.Msg); // 注册成功，切换到登录
                _eventAggregator.GetEvent<SnackbarMsgEvent>().Publish(apiResponse.Msg);
                LogOrReg = 0;
            }
            else if (apiResponse.ResultCode == 2)
            {
                //MessageBox.Show(apiResponse.Msg);
                _eventAggregator.GetEvent<SnackbarMsgEvent>().Publish(apiResponse.Msg);
                LogOrReg = 1;
            }
            else
            {
                //MessageBox.Show(apiResponse.Msg);
                _eventAggregator.GetEvent<SnackbarMsgEvent>().Publish(apiResponse.Msg);
            }
        }
        else
        {
            apiResponse = new()
            {
                Msg = "服务器忙，请稍后"
            };
            //MessageBox.Show(apiResponse.Msg);
            _eventAggregator.GetEvent<SnackbarMsgEvent>().Publish(apiResponse.Msg);

        }
    }

    /// <summary>
    /// 登录方法
    /// </summary>
    private async void Log()
    {
        // 数据验证
        if (string.IsNullOrEmpty(LoginDto.LogAccount) || string.IsNullOrEmpty(LoginDto.LogPassword))
        {
            //MessageBox.Show("账号或密码不能为空");
            _eventAggregator.GetEvent<SnackbarMsgEvent>().Publish("账号或密码不能为空");
            return;
        }

        // 密码加密
        LoginDto.LogPassword = PasswordMd5.Md5(LoginDto.LogPassword);

        // API请求
        ApiRequest apiRequest = new()
        {
            Method = HttpMethod.Get,
            Route = $"Account/Login?logAccount={LoginDto.LogAccount}&logPassword={LoginDto.LogPassword}",
        };

        
        ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest); // 请求API

        string msg = apiResponse?.Msg ?? "服务器忙，请稍后";

        if (apiResponse != null)
        {
            if (apiResponse.ResultCode == 1)
            {
                //MessageBox.Show(msg);
                //_eventAggregator.GetEvent<SnackbarMsgEvent>().Publish(msg);
                //var temp2 = apiResponse.ResultData;

                var resultData = JsonConvert.DeserializeObject<AccountInfoDto>(apiResponse?.ResultData?.ToString() ?? string.Empty);

                DialogParameters parameters = [];
                parameters.Add("msg", msg);
                if (resultData != null)
                {
                    parameters.Add("accountName", resultData.Name);
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
                }
                else
                {
                    parameters.Add("msg", "没有用户名，未知错误");
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
                }
            }
            else
            {
                //MessageBox.Show(msg);
                _eventAggregator.GetEvent<SnackbarMsgEvent>().Publish(msg);
            }
        }
        else
        {
            //MessageBox.Show(msg);
            _eventAggregator.GetEvent<SnackbarMsgEvent>().Publish(msg);

        }
    }

    public bool CanCloseDialog()
    {
        return true;
    }

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
    }

    #endregion
}
