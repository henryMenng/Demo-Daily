using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace Daily.WPF.Extensions;
/// <summary>
/// PasswordBox的扩展属性
/// </summary>
public class PasswordBoxEx
{
    //密码框的附加属性
    public static string GetPwd(DependencyObject obj)
    {
        return (string)obj.GetValue(PwdProperty);
    }

    public static void SetPwd(DependencyObject obj, string value)
    {
        obj.SetValue(PwdProperty, value);
    }

    // Using a DependencyProperty as the backing store for Pwd.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PwdProperty =
        DependencyProperty.RegisterAttached("Pwd", typeof(string), typeof(PasswordBoxEx), new PropertyMetadata("", OnPwdChanged));

    /// <summary>
    /// 自定义的附加属性发生变化，改变Password
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void OnPwdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        PasswordBox? passwordBox = d as PasswordBox;

        string newPwd = (string)e.NewValue;//新的值

        if (passwordBox != null && passwordBox.Password != newPwd)
        {
            passwordBox.Password = newPwd;
        }
    }
}

/// <summary>
/// 添加 PasswordBox的行为，当PasswordBox的password改变，那么自定义的附加属性也要改变
/// </summary>
public class PasswordBoxBehavior : Behavior<PasswordBox>
{
    /// <summary>
    /// 附加s属性 注入事件
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PasswordChanged += OnPasswordChanged;
    }

    /// <summary>
    /// 当PasswordBox的password改变，那么自定义的附加属性也要改变
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        PasswordBox? passwordBox = sender as PasswordBox;//获得PasswordBox的密码值


        if (passwordBox != null)
        {
            string password = PasswordBoxEx.GetPwd(passwordBox);//获得拥有附加属性的PasswordBox的附加属性的值

            if (passwordBox.Password != password)//如果PaswordBox的值与拥有附加属性的PasswordBox的附加属性的值不相等
            {
                PasswordBoxEx.SetPwd(passwordBox, passwordBox.Password);//把PasswordBox的值给password
            }

        }

    }

    /// <summary>
    /// 销毁 移出事件 防止泄露
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PasswordChanged -= OnPasswordChanged;
    }
}
