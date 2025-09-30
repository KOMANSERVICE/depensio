namespace depensio.Application.Helpers;

public static class BoolHelper
{
    public static bool ToBool(this object? value, bool defaultValue = false)
    {
        if (value is null)
            return defaultValue;

        if (value is bool b)
            return b;

        if (value is string s)
        {
            if (bool.TryParse(s, out var parsedBool))
                return parsedBool;

            // Gérer les cas "1", "0", "yes", "no"
            s = s.Trim().ToLowerInvariant();
            if (s == "1" || s == "yes" || s == "y")
                return true;
            if (s == "0" || s == "no" || s == "n")
                return false;
        }

        // Gérer les entiers
        if (value is int i)
            return i != 0;

        if (value is long l)
            return l != 0;

        if (value is double d)
            return d != 0;

        return defaultValue;
    }

    public static bool IsValidEan13(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode) || barcode.Length != 13 || !barcode.All(char.IsDigit))
            return false;

        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            int digit = barcode[i] - '0';
            sum += (i % 2 == 0) ? digit : digit * 3;
        }
        int checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit == (barcode[12] - '0');
    }
}
