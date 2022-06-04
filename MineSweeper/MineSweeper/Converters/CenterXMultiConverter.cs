using System;
using System.Globalization;
using System.Windows.Data;

namespace MineSweeper.Converters;

public class CenterXMultiConverter : MultiConverterMarkupExtension<CenterXMultiConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            var width = (double)values[0];
            var height = (double)values[1];

            var centerWidth = width / 2;
            var centerHeight = height / 2;

            return centerWidth - centerHeight;
        }
        catch 
        {
            return Binding.DoNothing;
        }
    }

    public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
