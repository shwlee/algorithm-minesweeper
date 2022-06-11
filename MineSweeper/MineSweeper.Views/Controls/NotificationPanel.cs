using System.Windows;
using System.Windows.Controls;

namespace MineSweeper.Views.Controls;

/// <summary>
/// </summary>
public class NotificationPanel : ContentControl
{
    public static DependencyProperty IsShowProperty = DependencyProperty.Register(
        nameof(IsShow),
        typeof(bool),
        typeof(NotificationPanel),
        new PropertyMetadata(false, IsShowPropertyChanged));

    public bool IsShow
    {
        get => (bool)GetValue(IsShowProperty);
        set => SetValue(IsShowProperty, value);
    }

    static NotificationPanel()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationPanel), new FrameworkPropertyMetadata(typeof(NotificationPanel)));
    }

    private static void IsShowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NotificationPanel panel)
        {
            return;
        }

        var value = (bool)e.NewValue;
        var visibility = value ? Visibility.Visible : Visibility.Collapsed;
        panel.Visibility = visibility;
    }
}
