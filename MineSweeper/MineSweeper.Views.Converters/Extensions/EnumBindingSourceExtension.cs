using System.Windows.Markup;

namespace MineSweeper.Views.Converters.Extensions;

public class EnumBindingSourceExtension : MarkupExtension
{
    private Type? _enumType;
    public Type? EnumType
    {
        get => this._enumType;
        set
        {
            if (value == this._enumType)
            {
                return;
            }

            if (value != null)
            {
                if (value.IsEnum == false)
                {
                    throw new ArgumentException($"It's allowed enum type. Target:{value}");
                }
            }

            this._enumType = value;
        }
    }

    public EnumBindingSourceExtension(Type enumType) => _enumType = enumType;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (this._enumType == null)
        {
            throw new InvalidOperationException("The target is not set.");
        }

        return Enum.GetValues(this._enumType);
    }
}
