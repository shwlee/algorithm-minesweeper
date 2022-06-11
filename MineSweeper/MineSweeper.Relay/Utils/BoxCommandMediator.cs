using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using MineSweeper.Models;
using MineSweeper.Models.Messages;

namespace MineSweeper.ViewModels.Utils;

public class BoxCommandMediator : ObservableRecipient
{
    public void OpenBox(Box box)
    {
        Messenger.Send(new OpenBoxMessage(box));
    }

    public void MarkBox(Box box)
    {
        Messenger.Send(new MarkBoxMessage(box));
    }
}
