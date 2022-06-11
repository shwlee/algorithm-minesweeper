using System.Diagnostics;
using System.Windows;

namespace MineSweeper.Views.Behaviours;

public class TurnProperty
{
    public static DependencyProperty TurnCountUpProperty =
        DependencyProperty.RegisterAttached(
            "TurnCountUp",
            typeof(int),
            typeof(TurnProperty),
            new PropertyMetadata(new PropertyChangedCallback(TurnCountUpPropertyChanged)));

    public static void SetTurnCountUp(UIElement element, double value)
    {
        element.SetValue(TurnCountUpProperty, value);
    }

    public static int GetTurnCountUp(UIElement element)
    {
        return (int)element.GetValue(TurnCountUpProperty);
    }

    private static void TurnCountUpPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        try
        {
            var value = (int)e.NewValue;

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
