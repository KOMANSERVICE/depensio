namespace depensio.Shared.Pages.Boutique.Model;
//public record CreateBoutiqueResponse(Guid Id);
public record GetSettingByBoutiqueResponse(Setting Settings);

//public record GetBoutiqueByUserResponse(IEnumerable<BoutiqueDTO> Boutiques);

public record Setting
{
    public Guid BoutiqueId { get; set; } = Guid.Empty;
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

}

public class BoutiqueSettingValue
{
    public string Id { get; set; } = string.Empty;
    public string LabelValue { get; set; } = string.Empty;
    public object Value { get; set; }
    public string LabelText { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}


