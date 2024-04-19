using Daily.WPF.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Daily.WPF.ViewModels
{
    public class SettingsUcViewModel : BindableBase
    {
        //区域管理器
        private readonly IRegionManager _regionManager;

        //设置信息列表
        private List<SettingsInfo> _settingsInfoList = [];
        public List<SettingsInfo> SettingsInfoList
        {
            get { return _settingsInfoList; }
            set
            {
                _settingsInfoList = value;
                RaisePropertyChanged();
            }
        }

        //导航到设置的不同区域命令
        public DelegateCommand<string> NavigateToSettingsCom { get; private set; }

        //构造函数
        public SettingsUcViewModel(IRegionManager regionManager)
        {
            //区域管理器
            _regionManager = regionManager;
            //导航到设置的不同区域命令
            NavigateToSettingsCom = new DelegateCommand<string>(NavigateToSettings);
            //调用数据初始化
            InitialData();
        }

        //数据初始化
        private void InitialData()
        {
            _settingsInfoList = [
                new SettingsInfo{SettingsName="个性化",Icon="Palette",ViewName="PersonalUc"},
                new SettingsInfo{SettingsName="系统设置",Icon="Cog",ViewName="SysSetUc"},
                new SettingsInfo{SettingsName="关于更多",Icon="Information",ViewName="AboutUc"},
                ];
        }

        //导航到设置的不同区域
        private void NavigateToSettings(string viewName)
        {
            //导航到设置区域
            if (string.IsNullOrEmpty(viewName))
                return;
            ////记录导航
            //_regionManager.Regions["MainViewRegion"].RequestNavigate(viewName, callback =>
            //{
            //    _regionNavigationJournal = callback.Context.NavigationService.Journal;
            //});
            //转到导航的区域
            _regionManager.Regions["SettingsRegion"].RequestNavigate(viewName);
            //关闭左侧菜单
        }
    }
}
