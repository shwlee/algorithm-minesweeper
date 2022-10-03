using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MineSweeper.Models;
using MineSweeper.Models.Messages;

namespace MineSweeper.Utils;

public class BoxCommandMediator : ObservableRecipient
{
    public void OpenBox(Box box)
    {
        WeakReferenceMessenger.Default.Send(new OpenBoxMessage(box));
    }

    public void MarkBox(Box box)
    {
        WeakReferenceMessenger.Default.Send(new MarkBoxMessage(box));
    }
}
