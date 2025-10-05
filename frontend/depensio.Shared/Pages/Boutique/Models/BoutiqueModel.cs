namespace depensio.Shared.Pages.Boutique.Model;
public record CreateBoutiqueResponse(Guid Id);
public record CreateBoutiqueRequest(BoutiqueDTO Boutique);

public record GetBoutiqueByUserResponse(IEnumerable<BoutiqueDTO> Boutiques);

public record BoutiqueDTO
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly CreatedAt { get; set; }
    public string FirstUrl { get; set; } = string.Empty;

}

    
