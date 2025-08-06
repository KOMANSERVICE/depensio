namespace depensio.Shared.Helpers;

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
}
