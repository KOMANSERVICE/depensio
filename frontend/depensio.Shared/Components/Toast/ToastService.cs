
namespace depensio.Shared.Components.Toast;

public class ToastService
{
    public event Action<ToastMessage>? OnShow;

    public void ShowToast(string title, string content, string type = "info", int duration = 5000)
    {
        var toast = new ToastMessage
        {
            Title = title,
            Content = content,
            Type = type,
            Duration = duration
        };
        OnShow?.Invoke(toast);
    }
}
