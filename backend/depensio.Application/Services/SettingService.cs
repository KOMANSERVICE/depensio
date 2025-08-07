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
                "LabelValue": "Code Barre",
                "Value": "{{BarcodeGenerationMode.Auto}}",
                "LabelText": "Préfixe Code Barre",
                "Text": "",
                "Description": "Rendre la génération de code bar automatique, Ajouter egalement le préfixe."
            },{
                "Id": "{{BoutiqueSettingKeys.PRODUCT_STOCK_AUTOMATIQUE}}",
                "LabelValue": "Stock automatique",
                "Value": "true",
                "LabelText": "Préfixe Code Barre",
                "Text": "",
                "Description": "Coché pour rendre la gestion du stock automatique sinon la gestion du stock sera manuel"
            }
        ]
        """ },
             new SettingDTO { Key = BoutiqueSettingKeys.VENTE_KEY, Value = $$"""
        [
            {
                "Id": "{{BoutiqueSettingKeys.VENTE_AUTORISER_MODIFICATION_PRIX}}",
                "LabelValue": "Autoriser la modification du prix",
                "Value": "false",
                "LabelText": "Pourcentage de reduction maximun",
                "Text": "100",
                "Description": "Lors de la vente, à la caisse, permettre la modification du prix de vente"
            },{
                "Id": "{{BoutiqueSettingKeys.VENTE_AUTORISER_VENTE_AVEC_STOCK_ZERO}}",
                "LabelValue": "Vendre avec stock zéro (0)",
                "Value": "true",
                "LabelText": "Préfixe Code Bar.",
                "Text": "",
                "Description": "Lors de la vente, à la caisse, vous ou vos employer pourrons selectionner un produit en rupture de stock"
            }
        ]
        """ }
        };
    }
}
