using depensio.Shared.Models;

namespace depensio.Shared.Extensions;

public static class ErrorMessageExtension
{
    public static string GetErrorMessage(this ErrorMessage errorMessage, string propertyName)
    {
        if (errorMessage == null || errorMessage.ValidationErrors == null || !errorMessage.ValidationErrors.Any())
        {
            return string.Empty;
        }
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return string.Empty;
        }

        var result = errorMessage.ValidationErrors.FirstOrDefault(x => x.PropertyName.ToLower() == propertyName.ToLower());
        return result != null ? result.ErrorMessage : "";
    }
}
