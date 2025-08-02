using depensio.Domain.Constants;
using depensio.Domain.Enums;

namespace depensio.Application.Services;

public class SettingService : ISettingService
{

    public string GetSetting(string key)
    {
        var settings = GetAllSettings();
        var setting = settings.FirstOrDefault(s => s.Key == key) ?? new SettingDTO();

        return setting.Value;
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
                "Value": {{BarcodeGenerationMode.Auto}},
                "LabelText": "Préfixe Code Bar.",
                "Text": "",
                "Description": "Rendre la génération de code bar automatique, Ajouter egalement le préfixe."
            },
        ]
        """ }
        };
    }
}
