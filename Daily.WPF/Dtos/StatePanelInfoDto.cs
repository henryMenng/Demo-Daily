using CommunityToolkit.Mvvm.ComponentModel;
using Daily.WPF.Models;
using Mapster;

namespace Daily.WPF.Dtos;
public partial class StatePanelInfoDto : ObservableObject
{
    private readonly StatePanelInfo _statePanelInfo;

    public StatePanelInfoDto(StatePanelInfo statePanelInfo)
    {
        _statePanelInfo = statePanelInfo;
        _statePanelInfo.Adapt(this);
    }
    [ObservableProperty]
    private string _icon = string.Empty;

    [ObservableProperty]
    private string _itemName = string.Empty;

    [ObservableProperty]
    private string _backgroundColor = string.Empty;

    [ObservableProperty]
    private string _viewName = string.Empty;

    [ObservableProperty]
    private string _result = string.Empty;

    public string MouseHand
    {
        get
        {
            if (ItemName == "完成比例")
                return "";
            return "Hand";
        }
    }

    public void ApplyChanges() => this.Adapt(_statePanelInfo);

    public void DiscardChanges() => _statePanelInfo.Adapt(this);
}
