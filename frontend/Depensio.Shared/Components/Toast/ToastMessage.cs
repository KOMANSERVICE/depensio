
namespace depensio.Shared.Components.Toast;

public class ToastMessage
{
    public string Title { get; set; } = "Notification";
    public string Content { get; set; } = "";
    public string Type { get; set; } = "info"; // success, error, warning, info
    public int Duration { get; set; } = 5000;  // ms
    public string Progress { get; set; } = "100%"; // pour la barre
    public string Class { get; set; } = "";
    public bool IsClosing { get; set; } = false; // <- pour l’animation de fermeture
}
