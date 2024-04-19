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
    /// 备忘录数据模型
    /// </summary>
    public class MemoUcViewModel : BindableBase, INavigationAware
    {
        private readonly HttpRestClient _httpRestClient;

        private SnackbarMessageQueue _msgQueue = new();

        public string QueryText { get; set; } = string.Empty;

        private bool _isLeftDrawerOpen;

        public bool IsLeftDrawerOpen
        {
            get { return _isLeftDrawerOpen; }
            set { _isLeftDrawerOpen = value; RaisePropertyChanged(); }
        }


        public DelegateCommand<int?> DeleteMemoCom { get; }

        public SnackbarMessageQueue MsgQueue
        {
            get { return _msgQueue; }
            set { _msgQueue = value; RaisePropertyChanged(); }
        }

        public MemoDto Memo { get; set; } = new();

        public DelegateCommand<MemoDto> AddMemoCom { get; }

        public DelegateCommand GetConditionQueryMemoListCom { get; }

        //备忘录Dto列表
        private ObservableCollection<MemoDto> _memoDtoList = [];
        public ObservableCollection<MemoDto> MemoDtoList
        {
            get { return _memoDtoList; }
            set
            {
                _memoDtoList = value;
                RaisePropertyChanged();
            }
        }

        //构造函数
        public MemoUcViewModel(HttpRestClient httpRestClient)
        {
            _httpRestClient = httpRestClient;
            AddMemoCom = new(AddMemo);
            GetConditionQueryMemoListCom = new(GetConditionQueryMemoList);
            DeleteMemoCom = new(DeleteMemo);


            InitialData();
        }

        private async void AddMemo(MemoDto dto)
        {
            //判断AddToDoInfo是否为空 
            if (dto == null)
                return;

            //API请求实例
            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Post,
                Route = "Memo/AddMemo",
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
            GetAllMemoList();
            IsLeftDrawerOpen = false;
        }

        //初始化数据
        private void InitialData()
        {
            #region 假数据
            ////初始化备忘录Dto列表
            //_memoDtoList =
            //[
            //    new MemoDto
            //    {
            //        MemoId = 1,
            //        Title = "备忘录1",
            //        Content = "备忘录1内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 2,
            //        Title = "备忘录2",
            //        Content = "备忘录2内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 3,
            //        Title = "备忘录3",
            //        Content = "备忘录3内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 4,
            //        Title = "备忘录4",
            //        Content = "备忘录4内容",
            //        Status = 4
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 5,
            //        Title = "备忘录5",
            //        Content = "备忘录5内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 6,
            //        Title = "备忘录6",
            //        Content = "备忘录6内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 7,
            //        Title = "备忘录7",
            //        Content = "备忘录7内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 8,
            //        Title = "备忘录8",
            //        Content = "备忘录8内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 9,
            //        Title = "备忘录9",
            //        Content = "备忘录9内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 10,
            //        Title = "备忘录10",
            //        Content = "备忘录10内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 11,
            //        Title = "备忘录11",
            //        Content = "备忘录11内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 12,
            //        Title = "备忘录12",
            //        Content = "备忘录12内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 13,
            //        Title = "备忘录13",
            //        Content = "备忘录13内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 14,
            //        Title = "备忘录14",
            //        Content = "备忘录14内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 15,
            //        Title = "备忘录15",
            //        Content = "备忘录15内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 16,
            //        Title = "备忘录16",
            //        Content = "备忘录16内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 17,
            //        Title = "备忘录17",
            //        Content = "备忘录17内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 18,
            //        Title = "备忘录18",
            //        Content = "备忘录18内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 19,
            //        Title = "备忘录19",
            //        Content = "备忘录19内容",
            //        Status = 1
            //    },
            //    new MemoDto
            //    {
            //        MemoId = 20,
            //        Title = "备忘录20",
            //        Content = "备忘录20内容",
            //        Status = 1
            //    }

            //]; 
            #endregion
            GetAllMemoList();
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

            return;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            GetAllMemoList();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        private async void GetConditionQueryMemoList()
        {
            if (string.IsNullOrEmpty(QueryText))
            {
                GetAllMemoList();
                return;
            }

            ApiRequest apiRequest = new()
            {
                Method = HttpMethod.Get,
                //http://localhost:5297/api/Memo/GetConditionQueryToDoList?queryText=%E4%BA%94
                Route = $"Memo/GetConditionQueryMemoList?searchText={QueryText}"
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

            var DeserRes = JsonConvert.DeserializeObject<List<MemoDto>>(apiResponse?.ResultData?.ToString() ?? string.Empty);

            MemoDtoList = DeserRes.Adapt<ObservableCollection<MemoDto>>();

            MsgQueue.Enqueue(apiResponse?.Msg ?? string.Empty);

            return;
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
                Method = HttpMethod.Delete,
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
    }
}
