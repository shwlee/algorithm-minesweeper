using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MineSweeper.Views.Converters;

public class ColorAlphaConverter : ConverterMarkupExtension<ColorAlphaConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            var color = (Color)value;
            var alpha = byte.Parse(parameter.ToString());

            color.A = alpha;

            return color;
        }
        catch
        {
            return Binding.DoNothing;
        }
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
