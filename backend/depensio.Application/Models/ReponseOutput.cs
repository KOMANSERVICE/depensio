
namespace depensio.Application.Models;
public class ReponseOutput
{
    public OutputData Output { get; set; }
}

public class OutputData
{
    public string Nom { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    public string Question { get; set; }
    public string Contexte { get; set; }
    public string Reponse { get; set; }
}
