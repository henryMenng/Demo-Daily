using System.Windows;

namespace Daily.WPF.Views;
/// <summary>
/// MainWin.xaml 的交互逻辑
/// </summary>
public partial class MainWin : Window
{
    public MainWin()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 最小化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Min_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// 最大化或正常
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MaxOrNormal_Click(object sender, RoutedEventArgs e)
    {
        if (this.WindowState != WindowState.Maximized)
            this.WindowState = WindowState.Maximized;
        else
            this.WindowState = WindowState.Normal;
    }

    /// <summary>
    /// 关闭程序
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Close_Click(object sender, RoutedEventArgs e)
    {
        //关闭当前窗口
        //this.Close();
        //强制退出
        Environment.Exit(0);
    }

}
