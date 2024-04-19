using Prism.Mvvm;

namespace Daily.WPF.Dtos
{
    /// <summary>
    /// 待办事项Dto
    /// </summary>
    public class ToDoInfoDto : BindableBase
    {
        /// <summary>
        /// 待办事项Id
        /// </summary>
        private int _toDoId;

        public int ToDoId
        {
            get { return _toDoId; }
            set { _toDoId = value; RaisePropertyChanged(); }
        }



        /// <summary>
        /// 待办事项标题
        /// </summary>
        private string _title = string.Empty;

        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 待办事项内容
        /// </summary>
        private string _content = string.Empty;

        public string Content
        {
            get { return _content; }
            set { _content = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 待办事项状态
        /// </summary>
        private int _status;

        public int Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged(); }
        }

        private string _background = string.Empty;

        public string Background
        {
            get { return _background; }
            set { _background = value; RaisePropertyChanged(); }
        }



    }
}
