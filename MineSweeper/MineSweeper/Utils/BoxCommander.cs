using Microsoft.Toolkit.Mvvm.Input;
using MineSweeper.Models;
using System;
using System.Windows.Input;

namespace MineSweeper.Utils;

public static class BoxCommander
{
    private static Lazy<BoxCommandMediator> _commandBag = new Lazy<BoxCommandMediator>(() => new BoxCommandMediator());

    public static BoxCommandMediator CommandBag => _commandBag.Value;

    private static RelayCommand<Box>? _openCommand;

    public static ICommand OpenCommand => _openCommand ??= new RelayCommand<Box>(OpenBox);

    private static void OpenBox(Box? box)
    {
        if (box is null)
        {
            return;
        }

        CommandBag.OpenBox(box);
    }
}
