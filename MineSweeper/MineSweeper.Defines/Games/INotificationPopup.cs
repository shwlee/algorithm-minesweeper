namespace MineSweeper.Defines.Games;

public interface INotificationPopup
{
    bool IsPopup { get; set; }

    IPopupContent? Content { get; set; }
}
