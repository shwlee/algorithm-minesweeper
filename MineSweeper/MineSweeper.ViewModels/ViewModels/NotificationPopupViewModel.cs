using Microsoft.Toolkit.Mvvm.ComponentModel;
using MineSweeper.Defines.Games;

namespace MineSweeper.ViewModels;

public partial class NotificationPopupViewModel : ObservableRecipient, INotificationPopup
{
    [ObservableProperty]
    public bool _isPopup;

    [ObservableProperty]
    public IPopupContent? _content;
}
