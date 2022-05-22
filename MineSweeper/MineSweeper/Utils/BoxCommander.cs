using Microsoft.Toolkit.Mvvm.Input;
using MineSweeper.Models;
using System;
using System.Windows;
using System.Windows.Input;

namespace MineSweeper.Utils;

public static class BoxCommander
{
    private static Lazy<BoxCommandMediator> _commandBag = new Lazy<BoxCommandMediator>(() => new BoxCommandMediator());

    public static BoxCommandMediator CommandBag => _commandBag.Value;

    #region Open

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

    #endregion

    #region Mark

    public static readonly DependencyProperty MarkHandleCommandProperty =
       DependencyProperty.RegisterAttached(
           "MarkHandleCommand",
           typeof(ICommand),
           typeof(BoxCommander),
           new PropertyMetadata(null, OnMarkCommandChanged));

    public static ICommand GetMarkHandleCommand(DependencyObject target)
    {
        return (ICommand)target.GetValue(MarkHandleCommandProperty);
    }

    public static void SetMarkHandleCommand(DependencyObject target, ICommand value)
    {
        target.SetValue(MarkHandleCommandProperty, value);
    }

    private static void OnMarkCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is ICommand command == false)
        {
            return;
        }

        if (d is FrameworkElement element == false)
        {
            return;
        }

        element.MouseRightButtonDown += (sender, args) =>
        {
            if (sender is FrameworkElement target == false)
            {
                return;
            }

            if (target.DataContext is Box box == false)
            {
                return;
            }

            command.Execute(box);
        };
    }

    private static RelayCommand<Box>? _markCommand;

    public static ICommand MarkCommand => _markCommand ??= new RelayCommand<Box>(MarkBox);

    private static void MarkBox(Box? box)
    {
        if (box is null)
        {
            return;
        }

        CommandBag.MarkBox(box);
    }

    #endregion
}
