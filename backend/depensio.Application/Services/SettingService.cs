using depensio.Domain.Constants;
using depensio.Domain.Enums;

namespace depensio.Application.Services;

public class SettingService : ISettingService
{

    public SettingDTO GetSetting(string key)
    {
        var settings = GetAllSettings();
        return settings.FirstOrDefault(s => s.Key == key) ?? new SettingDTO();
    }

    private List<SettingDTO> GetAllSettings()
    {
        // Simulate fetching all settings from a database or other storage
        // In a real application, this would involve database access logic
        return new List<SettingDTO>
        {
            new SettingDTO { Key = BoutiqueSettingKeys.PRODUCT_KEY, Value = $$"""
        [
            {
                "Id": "{{BoutiqueSettingKeys.PRODUCT_BARCODE_GENERATION_MODE}}",
                "LabelValue": "Code Bar.",
                "Value": "{{BarcodeGenerationMode.Auto}}",
                "LabelText": "Préfixe Code Bar.",
                "Text": "",
                "Description": "Rendre la génération de code bar automatique, Ajouter egalement le préfixe."
            },{
                "Id": "{{BoutiqueSettingKeys.PRODUCT_STOCK_AUTOMATIQUE}}",
                "LabelValue": "Stock automatique",
                "Value": "true",
                "LabelText": "Préfixe Code Bar.",
                "Text": "",
                "Description": "Coché pour rendre la gestion du stock automatique sinon la gestion du stock sera manuel"
            }
        ]
        """ }
        };
    }
}
