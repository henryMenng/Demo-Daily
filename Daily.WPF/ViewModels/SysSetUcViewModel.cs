using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Daily.WPF.ViewModels;
public class SysSetUcViewModel : BindableBase
{
    public DelegateCommand ChangeAvatarCommand { get; }

    private ImageSource _avatar;
    public ImageSource Avatar
    {
        get { return _avatar; }
        set { SetProperty(ref _avatar, value); }
    }

    public SysSetUcViewModel()
    {
        ChangeAvatarCommand = new DelegateCommand(ChangeAvatar);
        _avatar = new BitmapImage(new Uri("pack://application:,,,/Daily.WPF;component/Resource/Images/user.jpg"));
    }
    private void ChangeAvatar()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "图片文件(*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            Avatar = new BitmapImage(new Uri(openFileDialog.FileName));
        }
    }
}


