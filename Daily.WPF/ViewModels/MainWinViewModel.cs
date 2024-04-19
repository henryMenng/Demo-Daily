using Daily.WPF.Models;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Daily.WPF.ViewModels;
public class MainWinViewModel : BindableBase
{
    #region Fields

    // 事件聚合器
    private readonly IEventAggregator _eventAggregator;
    //区域管理器
    private readonly IRegionManager _regionManager;
    //导航记录
    private IRegionNavigationJournal _regionNavigationJournal;

    private ImageSource _avatar = new BitmapImage(new Uri("pack://application:,,,/Daily.WPF;component/Resource/Images/user.jpg"));
    public ImageSource Avatar
    {
        get { return _avatar; }
        set { SetProperty(ref _avatar, value); }
    }

    private Point _center;
    public Point Center
    {
        get { return _center; }
        set { SetProperty(ref _center, value); }
    }

    private double _radiusX;
    public double RadiusX
    {
        get { return _radiusX; }
        set { SetProperty(ref _radiusX, value); }
    }

    private double _radiusY;
    public double RadiusY
    {
        get { return _radiusY; }
        set { SetProperty(ref _radiusY, value); }
    }


    #endregion

    #region Properties

    //登录用户姓名
    private string _logName = string.Empty;

    public string LogName
    {
        get { return _logName; }
        set
        {
            _logName = value;
            RaisePropertyChanged();
        }
    }

    // 日志消息
    private string _logMsg = string.Empty;
    public string LogMsg
    {
        get { return _logMsg; }
        set
        {
            _logMsg = value;
            RaisePropertyChanged();
        }
    }
    // 请求关闭对话框事件
    public event Action<IDialogResult>? RequestClose;
    // 对话框标题
    public string Title { get; set; } = "我是对话框标题";
    //改变用户控件命令
    public DelegateCommand<string> ChangeUserControlCom { get; private set; }
    //区域返回命令
    public DelegateCommand RegionGoBackCom { get; private set; }
    //区域前进命令
    public DelegateCommand RegionGoForwardCom { get; private set; }
    //左侧菜单信息列表是否打开属性
    private bool _isLeftDrawerOpen;
    public bool IsLeftDrawerOpen
    {
        get { return _isLeftDrawerOpen; }
        set
        {
            _isLeftDrawerOpen = value;
            RaisePropertyChanged();
        }
    }
    // 消息队列
    private SnackbarMessageQueue _msgQueue = new();
    public SnackbarMessageQueue MsgQueue
    {
        get { return _msgQueue; }
        set { SetProperty(ref _msgQueue, value); }
    }
    // 左侧菜单信息列表
    private List<LeftMenuInfo> _leftMenuInfoList;
    public List<LeftMenuInfo> LeftMenuInfoList
    {
        get { return _leftMenuInfoList; }
        set { SetProperty(ref _leftMenuInfoList, value); }
    }

    public DelegateCommand ChangeAvatarCommand { get; }


    #endregion

    #region Constructors

    // 构造函数
    public MainWinViewModel(IEventAggregator eventAggregator, IRegionManager regionManager
        , IRegionNavigationJournal regionNavigationJournal)
    {
        //注入区域管理器
        _regionManager = regionManager;
        //注入导航记录
        _regionNavigationJournal = regionNavigationJournal;
        //注入事件聚合器
        _eventAggregator = eventAggregator;
        //初始化改变用户控件命令
        ChangeUserControlCom = new DelegateCommand<string>(ChangeUserControl);
        ChangeAvatarCommand = new DelegateCommand(ChangeAvatar);

        //初始化区域返回命令
        RegionGoBackCom = new DelegateCommand(() =>
        {
            if (_regionNavigationJournal != null && _regionNavigationJournal.CanGoBack == true)
                _regionNavigationJournal.GoBack();
        });
        //初始化区域前进命令
        RegionGoForwardCom = new DelegateCommand(() =>
        {
            if (_regionNavigationJournal != null && _regionNavigationJournal.CanGoForward == true)
                _regionNavigationJournal.GoForward();
        });
        //初始化左侧菜单信息列表
        _leftMenuInfoList =
            [
                new LeftMenuInfo { MenuName = "首页", Icon = PackIconKind.Home.ToString(), ViewName = "HomeUc" },
                new LeftMenuInfo { MenuName = "待办事项", Icon = PackIconKind.AlarmMultiple.ToString(), ViewName = "ToDoUc" },
                new LeftMenuInfo { MenuName = "备忘录", Icon = PackIconKind.NoteCheck.ToString(), ViewName = "MemoUc" },
                new LeftMenuInfo { MenuName = "设置", Icon = PackIconKind.Cog.ToString(), ViewName = "SettingsUc" }
            ];
        // 读取头像路径
        if (System.IO.File.Exists("avatarPath.txt"))
        {
            try
            {
                string avatarPath = System.IO.File.ReadAllText("avatarPath.txt");
                Avatar = new BitmapImage(new Uri(avatarPath));
            }
            catch (Exception)
            {
                // 处理错误，例如显示错误消息
                // 使用默认头像
                Avatar = new BitmapImage(new Uri("pack://application:,,,/Daily.WPF;component/Resource/Images/user.jpg"));
            }
        }
        else
        {
            // 使用默认头像
            Avatar = new BitmapImage(new Uri("pack://application:,,,/Daily.WPF;component/Resource/Images/user.jpg"));
        }
    }

    #endregion

    #region Private Methods

    // 订阅消息
    private void SubMsg(string obj)
    {
        MsgQueue.Enqueue(obj);
    }

    //改变用户控件方法
    private void ChangeUserControl(string viewName)
    {
        //如果视图名称为空，则返回
        if (string.IsNullOrEmpty(viewName))
            return;
        //导航参数
        NavigationParameters paras = [];
        //添加用户名参数
        paras.Add("UserName", LogName);
        //记录导航
        _regionManager.Regions["MainViewRegion"].RequestNavigate(viewName, callback =>
        {
            _regionNavigationJournal = callback.Context.NavigationService.Journal;
        });
        //转到导航的区域
        _regionManager.Regions["MainViewRegion"].RequestNavigate(viewName, paras);
        //关闭左侧菜单
        IsLeftDrawerOpen = false;
    }

    private void ChangeAvatar()
    {
        #region 注释
        //var openFileDialog = new Microsoft.Win32.OpenFileDialog();
        //openFileDialog.Filter = "图片文件(*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

        //if (openFileDialog.ShowDialog() == true)
        //{
        //    var newAvatar = new BitmapImage();
        //    newAvatar.BeginInit();
        //    newAvatar.UriSource = new Uri(openFileDialog.FileName);
        //    newAvatar.CacheOption = BitmapCacheOption.OnLoad;
        //    newAvatar.EndInit();

        //    Avatar = newAvatar;

        //    // 获取新头像的尺寸
        //    newAvatar.DownloadCompleted += (s, e) =>
        //    {
        //        int newAvatarWidth = newAvatar.PixelWidth;
        //        int newAvatarHeight = newAvatar.PixelHeight;

        //        // 更新裁剪的参数
        //        UpdateAvatarClipParameters(newAvatarWidth, newAvatarHeight);
        //    };
        //}
        #endregion

        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "图片文件(*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                var newAvatar = new BitmapImage();
                newAvatar.BeginInit();
                newAvatar.UriSource = new Uri(openFileDialog.FileName);
                newAvatar.CacheOption = BitmapCacheOption.OnLoad;
                newAvatar.EndInit();

                Avatar = newAvatar;

                // 获取新头像的尺寸
                newAvatar.DownloadCompleted += (s, e) =>
                {
                    int newAvatarWidth = newAvatar.PixelWidth;
                    int newAvatarHeight = newAvatar.PixelHeight;

                    // 更新裁剪的参数
                    UpdateAvatarClipParameters(newAvatarWidth, newAvatarHeight);
                };

                // 将头像路径保存到文件
                System.IO.File.WriteAllText("avatarPath.txt", openFileDialog.FileName);
            }
            catch (Exception)
            {
                // 处理错误，例如显示错误消息
            }
        }
    }

    public void UpdateAvatarClipParameters(int imageWidth, int imageHeight)
    {
        // 计算裁剪的中心
        Center = new Point(imageWidth / 2.0, imageHeight / 2.0);

        // 计算裁剪的半径
        RadiusX = RadiusY = Math.Min(imageWidth, imageHeight) / 2.0;
    }


    #endregion
}
