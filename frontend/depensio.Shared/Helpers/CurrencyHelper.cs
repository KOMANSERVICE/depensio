
using System.Globalization;

namespace depensio.Shared.Helper;

public static class CurrencyHelper
{
    public static string FormatFcfa(decimal amount)
    {
        return string.Format(new CultureInfo("fr-FR"), "{0:N0} FCFA", amount);
    }
}
