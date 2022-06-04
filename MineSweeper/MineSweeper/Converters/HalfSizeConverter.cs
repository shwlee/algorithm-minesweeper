using System;
using System.Globalization;
using System.Windows.Data;

namespace MineSweeper.Converters;

public class HalfSizeConverter : ConverterMarkupExtension<HalfSizeConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            var size = (double)value;

            return size / 2;
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
