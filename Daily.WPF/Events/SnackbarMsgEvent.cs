using Prism.Events;

namespace Daily.WPF.Events;
/// <summary>
/// 发布订阅 - Snackbar消息事件
/// </summary>
public class SnackbarMsgEvent : PubSubEvent<string>
{
}
