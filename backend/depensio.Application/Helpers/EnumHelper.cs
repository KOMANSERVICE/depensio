namespace depensio.Application.Helpers;

public static class EnumHelper
{
    public static TEnum ParseOrDefault<TEnum>(object? value, TEnum defaultValue) where TEnum : struct, Enum
    {
        if (value is string strValue &&
            Enum.TryParse<TEnum>(strValue, ignoreCase: true, out var result))
        {
            return result;
        }

        if (value is TEnum enumValue)
        {
            return enumValue;
        }

        return defaultValue;
    }

}
