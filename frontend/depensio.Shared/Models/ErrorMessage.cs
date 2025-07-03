using System.Text.Json.Serialization;

namespace depensio.Shared.Models;

public class ErrorMessage
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("status")]
    public int Status { get; set; } 
    [JsonPropertyName("ValidationErrors")]
    public List<ValidationError> ValidationErrors { get; set; } = new List<ValidationError>();
}
