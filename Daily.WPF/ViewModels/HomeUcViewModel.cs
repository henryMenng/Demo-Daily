using CommunityToolkit.Mvvm.ComponentModel;
using Daily.WPF.Dtos;
using Daily.WPF.HttpClients;
using Daily.WPF.Models;
using Daily.WPF.Services;
using Daily.WPF.Views.Dialog;
using Mapster;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace Daily.WPF.ViewModels
{
    //HomeUc的ViewModel，继承ObservableObject,INavigationAware，实现导航接口，以及数据绑定（允许属性被观察）
    public partial class HomeUcViewModel : ObservableObject, INavigationAware
    {
        #region Fields

        //用户名
        private string _userName = string.Empty;
        //统计面板Dto集合
        private ObservableCollection<StatePanelInfoDto> _statePanelInfoDtoList = [];
        //HttpRestClient实例,用于API请求
        private readonly HttpRestClient _httpRestClient;
        //统计面板集合
        private ObservableCollection<StatePanelInfo> _statePanelInfoList = [];
        //待办事项Dto集合
        private ObservableCollection<ToDoInfoDto> _toDoInfoDtoList = [];
        //备忘录Dto集合
        private ObservableCollection<MemoInfo> _memoInfoList = [];
        //自定义对话框服务
        private readonly DialogHostService _dialogHostService;

        private readonly IRegionManager _regionManager;

        private ObservableCollection<MemoDto> _memoDtoList = [];

        public ObservableCollection<MemoDto> MemoDtoList
        {
            get { return _memoDtoList; }
            set { _memoDtoList = value; OnPropertyChanged(); }
        }


        #endregion

        #region Properties

        //用户名，从登录页面传递过来
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }
        //统计面板Dto集合,使用Mapster来避免对Model的改动
        public ObservableCollection<StatePanelInfoDto> StatePanelInfoDtoList
        {
            get { return _statePanelInfoDtoList; }
            set { _statePanelInfoDtoList = value; }
        }
        //当前时间
        public string NowTime { get; set; } = DateTime.Now.ToString("yyyy年MM月dd日");
        //当前星期
        public string NowWeek { get; set; } = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
        //统计面板集合
        public ObservableCollection<StatePanelInfo> StatePanelInfoList
        {
            get { return _statePanelInfoList; }
            set { _statePanelInfoList = value; }
        }
        //待办事项Dto集合
        public ObservableCollection<ToDoInfoDto> ToDoInfoDtoList
        {
            get { return _toDoInfoDtoList; }
            set
            {
                _toDoInfoDtoList = value;
                OnPropertyChanged();
            }
        }
        //备忘录Dto集合
        public ObservableCollection<MemoInfo> MemoInfoList
        {
            get { return _memoInfoList; }
            set
            {
                _memoInfoList = value;
                OnPropertyChanged();
            }
        }
        //弹窗信息队列
        public SnackbarMessageQueue MsgQueue { get; set; } = new SnackbarMessageQueue();
        #endregion

        #region Commands

        //统计面板Dto命令
        public DelegateCommand GetStatePanelInfoDtoListCom { get; }
        //打开添加待办事项对话框命令
        public DelegateCommand ShowAddToDoDiaCom { get; }
        //更改待办事项状态命令
        public DelegateCommand<int?> UpdateToDoStatusCom { get; }
        //双击打开添加待办事项对话框命令
        public DelegateCommand<ToDoInfoDto> DoubleClickShowAddToDoCom { get; }

        public DelegateCommand<StatePanelInfoDto> NavigateCom { get; }

        public DelegateCommand ShowAddMemoDiaCom { get; }

        public DelegateCommand<MemoDto> DoubleClickShowAddMemoCom { get; }

        //删除备忘录状态命令
        public DelegateCommand<int?> DeleteMemoCom { get; }

        #endregion

        #region Constructor

        //构造函数
        public HomeUcViewModel(HttpRestClient httpRestClient, DialogHostService dialogHostService, IRegionManager regionManager)
        {
            //注入HttpRestClient
            _httpRestClient = httpRestClient;
            //注入自定义对话框服务
            _dialogHostService = dialogHostService;
            _regionManager = regionManager;
            //初始化获得统计面板Dto命令
            GetStatePanelInfoDtoListCom = new DelegateCommand(GetStatisticsToDoDto);
            //初始化打开添加待办事项对话框命令
            ShowAddToDoDiaCom = new DelegateCommand(ShowAddToDoDia);
            //初始化更改待办事项状态命令
            UpdateToDoStatusCom = new DelegateCommand<int?>(UpdateToDoStatus);
            //初始化双击打开添加待办事项对话框命令
            DoubleClickShowAddToDoCom = new DelegateCommand<ToDoInfoDto>(DoubleClickShowAddToDo);

            NavigateCom = new(Navigate);

            ShowAddMemoDiaCom = new(ShowAddMemoDia);

            DoubleClickShowAddMemoCom = new(DoubleClickShowAddMemo);

            DeleteMemoCom = new(DeleteMemo);
            //初始化数据
            InitialData();
        }



        #endregion

        #region Methods

        //初始化数据
        private void InitialData()
        {
            #region 统计面版集合字段初始化
            _statePanelInfoList =
                        [
                    new StatePanelInfo() { Icon = "ClockFast", ItemName = "汇总", BackgroundColor = "#FF0CA0FF", ViewName = "ToDoUc", Result = "9" },
                    new StatePanelInfo() { Icon = "ClockCheckOutline", ItemName = "已完成", BackgroundColor = "#FF1ECA3A", ViewName = "ToDoUc", Result = "9" },
                    new StatePanelInfo() { Icon = "ChartLineVariant", ItemName = "完成比例", BackgroundColor = "#FF02C6DC", Result = "90%" },
                    new StatePanelInfo() { Icon = "PlaylistStar", ItemName = "备忘录", BackgroundColor = "#FFFFA000", ViewName = "MemoUc", Result = "20" }
                        ];
            #endregion

            //#region 待办事项集合字段初始化
            //_toDoInfoDtoList =
            //            [
            //                new ToDoInfoDto() { ToDoId = 1, Title = "待办事项1", Content = "待办事项1内容", Status = 0 },
            //        new ToDoInfoDto() { ToDoId = 2, Title = "待办事项2", Content = "待办事项2内容", Status = 1 },
            //        new ToDoInfoDto() { ToDoId = 3, Title = "待办事项3", Content = "待办事项3内容", Status = 0 },
            //        new ToDoInfoDto() { ToDoId = 4, Title = "待办事项4", Content = "待办事项4内容", Status = 1 },
            //        new ToDoInfoDto() { ToDoId = 5, Title = "待办事项5", Content = "待办事项5内容", Status = 0 },
            //        new ToDoInfoDto() { ToDoId = 6, Title = "待办事项6", Content = "待办事项6内容", Status = 1 },
            //        new ToDoInfoDto() { ToDoId = 7, Title = "待办事项7", Content = "待办事项7内容", Status = 0 },
            //        new ToDoInfoDto() { ToDoId = 8, Title = "待办事项8", Content = "待办事项8内容", Status = 1 },
            //        new ToDoInfoDto() { ToDoId = 9, Title = "待办事项9", Content = "待办事项9内容", Status = 0 },
            //        new ToDoInfoDto() { ToDoId = 10, Title = "待办事项10", Content = "待办事项10内容", Status = 1 }
            //            ];
            //#endregion

            #region 统计面板集合Dto字段初始化，使用Mapster来避免对Model的改动
            for (int i = 0; i < StatePanelInfoList.Count; i++)
            {
                StatePanelInfoDtoList.Add(new StatePanelInfoDto(StatePanelInfoList[i]));
            }
            #endregion

            #region 备忘录集合字段初始化
            //_memoInfoList =
            //            [
            //                new MemoInfo() { MemoId = 1, Title = "备忘录1", Content = "备忘录1内容", Status = 0 },
            //        new MemoInfo() { MemoId = 2, Title = "备忘录2", Content = "备忘录2内容", Status = 1 },
            //        new MemoInfo() { MemoId = 3, Title = "备忘录3", Content = "备忘录3内容", Status = 0 },
            //        new MemoInfo() { MemoId = 4, Title = "备忘录4", Content = "备忘录4内容", Status = 1 },
            //        new MemoInfo() { MemoId = 5, Title = "备忘录5", Content = "备忘录5内容", Status = 0 },
            //        new MemoInfo() { MemoId = 6, Title = "备忘录6", Content = "备忘录6内容", Status = 1 },
            //        new MemoInfo() { MemoId = 7, Title = "备忘录7", Content = "备忘录7内容", Status = 0 },
            //        new MemoInfo() { MemoId = 8, Title = "备忘录8", Content = "备忘录8内容", Status = 1 },
            //        new MemoInfo() { MemoId = 9, Title = "备忘录9", Content = "备忘录9内容", Status = 0 },
            //        new MemoInfo() { MemoId = 10, Title = "备忘录10", Content = "备忘录10内容", Status = 1 }
            //            ];
            #endregion

            //从API获取待办中事项的数据
            GetActiveToDoList();

            GetAllMemoList();

            GetStatisticsToDoDto();
        }

        private void Navigate(StatePanelInfoDto statePanelInfoDto)
        {
            if (statePanelInfoDto == null)
                return;
            NavigationParameters paras = [];
            if (statePanelInfoDto.ItemName == "已完成")
            {
                paras.Add("ShowFinished", true);
            }
            _regionManager.Regions["MainViewRegion"].RequestNavigate(statePanelInfoDto.ViewName, callback => { }, paras);
        }

        #region INavigationAware接口方法
        //导航到此页面时触发
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("UserName"))
            {
                var temp = navigationContext.Parameters["UserName"] as string;
                if (!string.IsNullOrEmpty(temp))
                {
                    UserName = temp;
                    GetStatePanelInfoDtoListCom?.Execute();
                }
            }
        }
        //是否可以导航到此页面
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        //导航离开此页面时触发,做拦截
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
        #endregion

        //获取统计待办事项数据Dto(待办情况和已完成情况)
        private async void GetStatisticsToDoDto()
        {
            //API请求实例
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Get,
                Route = "ToDo/StatisticsToDo",
            };
            //API响应实例
            ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest);
            //判断API响应是否成功
            if (apiResponse != null)
            {
                //判断API响应结果是否成功
                if (apiResponse.ResultCode == 1)
                {
                    //反序列化API响应结果
                    var resultData = JsonConvert.DeserializeObject<StatisticsToDoDto>(apiResponse?.ResultData?.ToString() ?? string.Empty);
                    //判断反序列化结果是否为空
                    if (resultData != null)
                    {
                        //更新统计面板Dto数据
                        StatePanelInfoDtoList[0].Result = resultData.Total.ToString();
                        StatePanelInfoDtoList[0].ApplyChanges();
                        StatePanelInfoDtoList[1].Result = resultData.Completed.ToString();
                        StatePanelInfoDtoList[1].ApplyChanges();
                        StatePanelInfoDtoList[2].Result = resultData.CompletedPercent;
                        StatePanelInfoDtoList[2].ApplyChanges();
                    }
                }
                else
                {
                    apiResponse.Msg = "服务器忙，请稍后...";
                }
            }
            else
            {
                apiResponse ??= new();
                apiResponse.Msg = "服务器忙，请稍后...";
            }
        }
        //获取待办中事项的数据（未完成的待办事项）
        private async void GetActiveToDoList()
        {
            //API请求实例,调用API请求待办中事项列表
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Get,
                Route = "ToDo/GetActiveToDoList",
            };

            //API响应实例,调用API请求待办中事项列表,返回赋值给apiResponse
            ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest);

            if (apiResponse == null)
            {
                //弹出提示框
                MsgQueue.Enqueue("服务器忙，请稍后...");
                return;
            }

            if (apiResponse.ResultCode != 1)
            {
                //弹出提示框
                MsgQueue.Enqueue(apiResponse.Msg);
                return;
            }

            //反序列化API响应结果
            var resultData = JsonConvert.DeserializeObject<List<ToDoingDto>>(apiResponse?.ResultData?.ToString() ?? string.Empty);

            //更新待办事项Dto数据,从ToDoingDto适配到ToDoInfoDto
            var ToDoingDtoList = resultData.Adapt<ObservableCollection<ToDoInfoDto>>();
            ToDoInfoDtoList = ToDoingDtoList ?? ToDoInfoDtoList;
            return;

        }
        //打开添加待办事项对话框方法,并请求API添加待办事项
        private async void ShowAddToDoDia()
        {
            //打开自定义对话框，传递参数
            var result = await _dialogHostService.ShowDialog(nameof(AddToDoUc), new DialogParameters(), "RootDialog");
            //判断对话框返回结果,如果不是OK,直接返回
            if (result.Result != ButtonResult.OK)
                return;

            //判断对话框返回参数是否包含msg,有则弹出提示框
            if (result.Parameters.ContainsKey("msg"))
            {
                //获取对话框返回参数msg
                var msg = result.Parameters.GetValue<string>("msg");
                //弹出提示框
                MsgQueue.Enqueue(msg);
                return;
            }

            //判断对话框返回参数是否包含AddToDoInfo
            if (!result.Parameters.ContainsKey("AddToDoInfo"))
                return;

            //获取对话框返回参数AddToDoInfo
            var addModel = result.Parameters.GetValue<ToDoInfoDto>("AddToDoInfo");
            //判断AddToDoInfo是否为空 
            if (addModel == null)
                return;

            //API请求实例
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Post,
                Route = "ToDo/AddToDo",
                //适配AddToDoInfoDto
                Parameters = addModel.Adapt<ToDoInfoDto>()
            };
            //API响应实例
            ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest);
            //判断API响应是否成功
            if (apiResponse == null)
            {
                //弹出提示框
                MsgQueue.Enqueue("服务器忙，请稍后...");
                return;
            }

            //判断API响应结果是否成功
            if (apiResponse.ResultCode == 1)
            {
                //弹出提示框
                MsgQueue.Enqueue("添加成功");
                //重新获取统计待办事项数据Dto,触发绑定更新数据
                GetStatisticsToDoDto();
            }
            else
                //弹出提示框
                MsgQueue.Enqueue("添加失败");
            //重新获取待办中事项的数据
            GetActiveToDoList();
        }
        //双击打开编辑待办事项对话框方法
        private async void DoubleClickShowAddToDo(ToDoInfoDto editToDoDto)
        {
            //打开自定义对话框，传递参数
            var result = await _dialogHostService.ShowDialog(nameof(EditToDoUc), new DialogParameters(), "RootDialog");

            //判断editToDoId是否为空
            if (editToDoDto == null)
                return;

            //判断对话框返回结果,如果不是OK,直接返回
            if (result.Result != ButtonResult.OK)
                return;

            //判断对话框返回参数是否包含msg,有则弹出提示框
            if (result.Parameters.ContainsKey("msg"))
            {
                //获取对话框返回参数msg
                var msg = result.Parameters.GetValue<string>("msg");
                //弹出提示框
                MsgQueue.Enqueue(msg);
                return;
            }

            //判断对话框返回参数是否包含AddToDoInfo
            if (!result.Parameters.ContainsKey("EditToDoInfo"))
                return;

            //获取对话框返回参数AddToDoInfo
            var EditModel = result.Parameters.GetValue<ToDoInfoDto>("EditToDoInfo");

            //判断AddToDoInfo是否为空 
            if (EditModel == null)
                return;

            //把editToDoId赋值给EditModel.ToDoId，用于API请求
            EditModel.ToDoId = editToDoDto.ToDoId;
            //API请求实例
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Post,
                Route = "ToDo/EditToDo",
                //适配AddToDoInfoDto
                Parameters = EditModel.Adapt<ToDoInfoDto>()
            };
            //API响应实例
            ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest);

            //判断API响应是否成功
            if (apiResponse == null)
            {
                //弹出提示框
                MsgQueue.Enqueue("服务器忙，请稍后...");
                return;
            }

            //判断API响应结果是否成功
            if (apiResponse.ResultCode == 1)
                //弹出提示框
                MsgQueue.Enqueue(apiResponse.Msg);
            else
                //弹出提示框
                MsgQueue.Enqueue("服务器忙，请稍后...");

            //重新获取待办中事项的数据
            GetActiveToDoList();
            //刷新统计待办事项数据
            GetStatisticsToDoDto();
        }
        //更改待办事项状态
        private async void UpdateToDoStatus(int? id)
        {
            //判断id是否为空
            if (id == null)
            {
                //弹出提示框
                MsgQueue.Enqueue("id状态错误");
                return;
            }
            //API请求实例
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Get,
                Route = $"ToDo/UpdateToDoStatus?id={id}"
            };
            //API响应实例
            ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest);

            //判断API响应是否成功
            if (apiResponse == null)
            {
                //弹出提示框
                MsgQueue.Enqueue("服务器忙，请稍后...");
                return;
            }
            //弹出提示框
            MsgQueue.Enqueue(apiResponse.Msg);
            //刷新待办事项数据
            GetActiveToDoList();
            //刷新统计待办事项数据
            GetStatisticsToDoDto();
            return;
        }





        //获取备忘录数据（所有数据，解决总数和展示数据）刷新数据使用
        private async void GetAllMemoList()
        {
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Get,
                Route = "Memo/GetAllMemoList",
            };

            ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest);

            if (apiResponse == null)
            {
                MsgQueue.Enqueue("服务器忙，请稍后...");
                return;
            }

            if (apiResponse.ResultCode != 1)
            {
                MsgQueue.Enqueue(apiResponse.Msg);
                return;
            }

            if (apiResponse.ResultData == null)
            {
                MsgQueue.Enqueue(apiResponse.Msg);
                return;
            }

            var resultData = JsonConvert.DeserializeObject<List<MemoDto>>(apiResponse.ResultData.ToString() ?? string.Empty);

            MemoDtoList = resultData.Adapt<ObservableCollection<MemoDto>>() ?? MemoDtoList;
            StatePanelInfoDtoList[3].Result = MemoDtoList.Count.ToString();
            StatePanelInfoDtoList[3].ApplyChanges();
            return;
        }

        //打开添加备忘录对话框方法
        private async void ShowAddMemoDia()
        {
            //打开自定义对话框，传递参数
            var result = await _dialogHostService.ShowDialog(nameof(AddMemoUc), new DialogParameters(), "RootDialog");

            if (result == null)
                return;

            if (result.Result != ButtonResult.OK)
                return;

            //判断对话框返回结果,如果不是OK,直接返回
            if (result.Result != ButtonResult.OK)
                return;

            //判断对话框返回参数是否包含msg,有则弹出提示框
            if (result.Parameters.ContainsKey("msg"))
            {
                //获取对话框返回参数msg
                var msg = result.Parameters.GetValue<string>("msg");
                //弹出提示框
                MsgQueue.Enqueue(msg);
                return;
            }

            //判断对话框返回参数是否包含AddMemoDto
            if (!result.Parameters.ContainsKey("AddMemo"))
                return;

            //获取对话框返回参数AddToDoInfo
            var addModel = result.Parameters.GetValue<MemoDto>("AddMemo");
            //判断AddToDoInfo是否为空 
            if (addModel == null)
                return;

            //API请求实例
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Post,
                Route = "Memo/AddMemo",
                //适配AddToDoInfoDto
                Parameters = addModel.Adapt<ToDoInfoDto>()
            };
            //API响应实例
            ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest);
            //判断API响应是否成功
            if (apiResponse == null)
            {
                //弹出提示框
                MsgQueue.Enqueue("服务器忙，请稍后...");
                return;
            }

            //判断API响应结果是否成功
            if (apiResponse.ResultCode == 1)
            {
                //弹出提示框
                MsgQueue.Enqueue("添加成功");
                //重新获取统计待办事项数据Dto,触发绑定更新数据
                GetStatisticsToDoDto();
            }
            else
                //弹出提示框
                MsgQueue.Enqueue("添加失败");
            //重新获取备忘录的数据
            GetAllMemoList();
        }

        //双击打开编辑备忘录对话框方法
        private async void DoubleClickShowAddMemo(MemoDto editMemoDto)
        {
            //打开自定义对话框，传递参数
            var result = await _dialogHostService.ShowDialog(nameof(EditMemoUc), new DialogParameters(), "RootDialog");

            //判断editToDoId是否为空
            if (editMemoDto == null)
                return;

            //判断对话框返回结果,如果不是OK,直接返回
            if (result.Result != ButtonResult.OK)
                return;

            //判断对话框返回参数是否包含msg,有则弹出提示框
            if (result.Parameters.ContainsKey("msg"))
            {
                //获取对话框返回参数msg
                var msg = result.Parameters.GetValue<string>("msg");
                //弹出提示框
                MsgQueue.Enqueue(msg);
                return;
            }

            //判断对话框返回参数是否包含AddToDoInfo
            if (!result.Parameters.ContainsKey("EditMemoInfo"))
                return;

            //获取对话框返回参数AddToDoInfo
            var EditModel = result.Parameters.GetValue<MemoDto>("EditMemoInfo");

            //判断AddToDoInfo是否为空 
            if (EditModel == null)
                return;

            //把editToDoId赋值给EditModel.ToDoId，用于API请求
            EditModel.MemoId = editMemoDto.MemoId;
            //API请求实例
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Post,
                Route = "Memo/EditMemo",
                //适配AddToDoInfoDto
                Parameters = EditModel.Adapt<MemoDto>()
            };
            //API响应实例
            ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest);

            //判断API响应是否成功
            if (apiResponse == null)
            {
                //弹出提示框
                MsgQueue.Enqueue("服务器忙，请稍后...");
                return;
            }

            //判断API响应结果是否成功
            if (apiResponse.ResultCode == 1)
                //弹出提示框
                MsgQueue.Enqueue(apiResponse.Msg);
            else
                //弹出提示框
                MsgQueue.Enqueue("服务器忙，请稍后...");

            //重新获取备忘录的数据
            GetAllMemoList();
        }

        //更改备忘录状态(也就是删除)
        private async void DeleteMemo(int? id)
        {
            //判断id是否为空
            if (id == null)
            {
                //弹出提示框
                MsgQueue.Enqueue("id状态错误");
                return;
            }

            //API请求实例
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Get,
                //ttp://localhost:5297/api/Memo/DeleteMemo?id=18
                Route = $"Memo/DeleteMemo?id={id}",

            };
            //API响应实例
            ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest);

            //判断API响应是否成功
            if (apiResponse == null)
            {
                //弹出提示框
                MsgQueue.Enqueue("服务器忙，请稍后...");
                return;
            }
            //弹出提示框
            MsgQueue.Enqueue(apiResponse.Msg);
            //重新获取备忘录的数据
            GetAllMemoList();
            return;
        }




        #endregion
    }
}
