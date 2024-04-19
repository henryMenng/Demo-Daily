using Daily.WPF.Dtos;
using Daily.WPF.HttpClients;
using Mapster;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace Daily.WPF.ViewModels
{
    /// <summary>
    /// 待办事项视图模型
    /// </summary>
    public class ToDoUcViewModel : BindableBase, INavigationAware
    {
        private string _queryText = string.Empty;

        public string QueryText
        {
            get { return _queryText; }
            set { _queryText = value; RaisePropertyChanged(); }
        }

        private int _querySelectedIndex;

        public int QuerySelectedIndex
        {
            get { return _querySelectedIndex; }
            set { _querySelectedIndex = value; RaisePropertyChanged(); }
        }

        private bool _isLeftDrawerOpen;

        public bool IsLeftDrawerOpen
        {
            get { return _isLeftDrawerOpen; }
            set { _isLeftDrawerOpen = value; RaisePropertyChanged(); }
        }

        public DelegateCommand QueryToDoCom { get; set; }

        public DelegateCommand<int?> DeleteToDoCom { get; set; }

        private readonly HttpRestClient _httpRestClient;

        private SnackbarMessageQueue _msgQueue = new();

        public SnackbarMessageQueue MsgQueue
        {
            get { return _msgQueue; }
            set { _msgQueue = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<ToDoInfoDto> AddToDoCom { get; }

        public ToDoInfoDto ToDoInfoDto { get; set; } = new();


        //待办事项信息列表Dto
        private ObservableCollection<ToDoInfoDto> _toDoInfoDtoList = [];
        public ObservableCollection<ToDoInfoDto> ToDoInfoDtoList
        {
            get { return _toDoInfoDtoList; }
            set
            {
                _toDoInfoDtoList = value;
                RaisePropertyChanged();
            }
        }

        public ToDoUcViewModel(HttpRestClient httpRestClient)
        {
            _httpRestClient = httpRestClient;
            QueryToDoCom = new(QueryToDo);
            DeleteToDoCom = new(DeleteToDo);
            AddToDoCom = new(AddToDo);
            InitialData();
        }

        private async void AddToDo(ToDoInfoDto dto)
        {
            //判断AddToDoInfo是否为空 
            if (dto == null)
                return;

            //API请求实例
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Post,
                Route = "ToDo/AddToDo",
                //适配AddToDoInfoDto
                Parameters = dto.Adapt<ToDoInfoDto>()
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
                MsgQueue.Enqueue("添加成功");
            else
                //弹出提示框
                MsgQueue.Enqueue("添加失败");
            //重新获取待办中事项的数据
            GetAllToDoList();
            IsLeftDrawerOpen = false;

        }

        private void QueryToDo()
        {
            switch (QuerySelectedIndex)
            {
                case 0 when string.IsNullOrEmpty(QueryText):
                    GetAllToDoList();
                    break;
                case 0:
                    GetConditionQueryToDoList();
                    break;
                case 1 when string.IsNullOrEmpty(QueryText):
                    GetActiveToDoList();
                    break;
                case 1:
                    GetConditionQueryToDoList();
                    break;

                case 2 when string.IsNullOrEmpty(QueryText):
                    GetCompletedToDoList();
                    break;
                case 2:
                    GetConditionQueryToDoList();
                    break;
                default:
                    break;
            }
        }

        private async void GetConditionQueryToDoList()
        {
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Get,
                Route = $"ToDo/GetConditionQueryToDoList?status={QuerySelectedIndex}&searchText={QueryText}"
            };

            var apiResponse = await _httpRestClient.Execute(apiRequest);

            if (apiResponse == null)
            {
                MsgQueue.Enqueue("服务器忙，请稍后...");
                return;
            }

            if (apiResponse.ResultCode != 1 || apiResponse.ResultData == null)
            {
                MsgQueue.Enqueue(apiResponse.Msg);
                return;
            }

            var DeserRes = JsonConvert.DeserializeObject<List<ToDoInfoDto>>(apiResponse?.ResultData?.ToString() ?? string.Empty);

            ToDoInfoDtoList = DeserRes.Adapt<ObservableCollection<ToDoInfoDto>>();

            MsgQueue.Enqueue(apiResponse?.Msg ?? string.Empty);

            if (ToDoInfoDtoList == null)
                return;
            foreach (var item in ToDoInfoDtoList)
            {
                if (item.Status == 0)
                    item.Background = "#1E90FF";
                else
                    item.Background = "#3CB371";
            }
            return;
        }

        private async void GetCompletedToDoList()
        {
            //API请求实例,调用API请求已完成事项列表
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Get,
                Route = "ToDo/GetCompletedToDoList",
            };

            //API响应实例,调用API请求已完成事项列表,返回赋值给apiResponse
            ApiResponse? apiResponse = await _httpRestClient.Execute(apiRequest);

            if (apiResponse == null)
            {
                //弹出提示框
                MsgQueue.Enqueue("服务器忙，请稍后...");
                return;
            }

            if (apiResponse.ResultCode != 1)
            {
                MsgQueue.Enqueue(apiResponse.Msg);
                //弹出提示框
                return;
            }

            //反序列化API响应结果
            var resultData = JsonConvert.DeserializeObject<List<ToDoingDto>>(apiResponse?.ResultData?.ToString() ?? string.Empty);

            //更新待办事项Dto数据,从ToDoingDto适配到ToDoInfoDto
            var ToDoingDtoList = resultData.Adapt<ObservableCollection<ToDoInfoDto>>();
            ToDoInfoDtoList = ToDoingDtoList ?? ToDoInfoDtoList;
            MsgQueue.Enqueue((apiResponse ?? new ApiResponse()).Msg);
            if (ToDoInfoDtoList == null)
                return;
            foreach (var item in ToDoInfoDtoList)
            {
                item.Background = "#3CB371";
            }
            return;
        }

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
            if (ToDoInfoDtoList == null)
            {
                MsgQueue.Enqueue((apiResponse ?? new ApiResponse()).Msg);
                return;

            }
            foreach (var item in ToDoInfoDtoList)
            {
                item.Background = "#1E90FF";
            }
            return;
        }

        //数据初始化
        private void InitialData()
        {
            //初始化待办事项信息列表
            GetAllToDoList();
        }

        //获取全部待办事项信息
        private async void GetAllToDoList()
        {
            ApiRequest apiRequest = new()
            {
                Route = "ToDo/GetAllToDoList",
                Method = HttpMethod.Get
            };

            var apiResponse = await _httpRestClient.Execute(apiRequest);

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

            //反序列化API响应结果

            var resultData = JsonConvert.DeserializeObject<ObservableCollection<ToDoInfoDto>>(apiResponse.ResultData?.ToString() ?? string.Empty);

            //更新待办事项Dto数据
            ToDoInfoDtoList = resultData.Adapt<ObservableCollection<ToDoInfoDto>>() ?? ToDoInfoDtoList;

            MsgQueue.Enqueue(apiResponse.Msg);
            if (ToDoInfoDtoList == null)
                return;
            foreach (var item in ToDoInfoDtoList)
            {
                if (item.Status == 0)
                    item.Background = "#1E90FF";
                else
                    item.Background = "#3CB371";
            }
            return;

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("ShowFinished"))
                GetCompletedToDoList();
            else
                GetAllToDoList();


        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        //删除待办事项
        private async void DeleteToDo(int? id)
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
                Route = $"ToDo/DeleteToDo?id={id}",

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
            GetAllToDoList();
            return;
        }
    }
}
