using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Media;

namespace Daily.WPF.ViewModels;
public class PersonalUcViewModel : BindableBase
{
    #region 仿照写的
    private readonly PaletteHelper _paletteHelper = new();

    public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

    //主题背景色
    private bool _isDarkTheme;
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
            {
                ModifyTheme(theme => theme.SetBaseTheme(value ? BaseTheme.Dark : BaseTheme.Light));
            }
        }
    }


    public DelegateCommand<object> ChangeHueCommand { get; }

    public PersonalUcViewModel()
    {
        ChangeHueCommand = new DelegateCommand<object>(ChangeHue);
    }

    //主题色更改
    private static void ModifyTheme(Action<Theme> modificationAction)
    {
        var paletteHelper = new PaletteHelper();
        Theme theme = paletteHelper.GetTheme();

        modificationAction?.Invoke(theme);

        paletteHelper.SetTheme(theme);
    }

    private void ChangeHue(object? obj)
    {
        Theme theme = _paletteHelper.GetTheme();

        if (obj == null)
            return;
        var color = (Color)obj;

        theme.PrimaryLight = new ColorPair(color.Lighten());
        theme.PrimaryMid = new ColorPair(color);
        theme.PrimaryDark = new ColorPair(color.Darken());

        _paletteHelper.SetTheme(theme);
    }
    #endregion

    #region 源码

    //#region 设置主题背景
    //private bool _isDarkTheme;
    //public bool IsDarkTheme
    //{
    //    get => _isDarkTheme;
    //    set
    //    {
    //        if (SetProperty(ref _isDarkTheme, value))
    //        {
    //            ModifyTheme(theme => theme.SetBaseTheme(value ? Theme.Dark : Theme.Light));
    //        }
    //    }
    //}

    //private static void ModifyTheme(Action<ITheme> modificationAction)
    //{
    //    var paletteHelper = new PaletteHelper();
    //    ITheme theme = paletteHelper.GetTheme();

    //    modificationAction?.Invoke(theme);

    //    paletteHelper.SetTheme(theme);
    //}
    //#endregion

    //public PersonalUcViewModel()
    //{
    //    ChangeHueCommand = new DelegateCommand<object>(ChangeHue);
    //}

    //#region 顶部背景颜色

    //private readonly PaletteHelper paletteHelper = new PaletteHelper();

    //public DelegateCommand<object> ChangeHueCommand { get; }

    //public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

    //private void ChangeHue(object? obj)
    //{
    //    ITheme theme = paletteHelper.GetTheme();

    //    var color = (Color)obj;
    //    theme.PrimaryLight = new ColorPair(color.Lighten());
    //    theme.PrimaryMid = new ColorPair(color);
    //    theme.PrimaryDark = new ColorPair(color.Darken());

    //    paletteHelper.SetTheme(theme);
    //}
    //#endregion 

    #endregion

}
