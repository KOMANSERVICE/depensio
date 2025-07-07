namespace depensio.Shared.Extensions;

public static class StringExtension
{
    public static string GetFieldName(this string str)
    {
        if (string.IsNullOrEmpty(str) || !str.Contains("."))
        {
            return str;
        }
        var result = str.Substring(str.LastIndexOf('.') + 1);
        return result;
    }
}
