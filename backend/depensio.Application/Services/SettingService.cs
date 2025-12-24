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
                "Value": "false",
                "LabelText": "Préfixe Code Bar.",
                "Text": "",
                "Description": "Lors de la vente, à la caisse, vous ou vos employer pourrons selectionner un produit en rupture de stock"
            },{
                "Id": "{{BoutiqueSettingKeys.VENTE_ENVOI_AUTOMATIQUE_TRESORERIE}}",
                "LabelValue": "Envoi automatique des ventes à la trésorerie",
                "Value": "false",
                "LabelText": "",
                "Text": "",
                "Description": "Coché pour rendre l'envoi des ventes approuvées à la trésorerie."
            }
        ]
        """ },
             new SettingDTO { Key = BoutiqueSettingKeys.ACHAT_KEY, Value = $$"""
        [
            {
                "Id": "{{BoutiqueSettingKeys.ACHAT_ENVOI_AUTOMATIQUE_TRESORERIE}}",
                "LabelValue": "Envoi automatique vers la trésorerie",
                "Value": "false",
                "LabelText": "",
                "Text": "",
                "Description": "Si activé, les achats validés seront automatiquement envoyés à la trésorerie. Les champs Mode de paiement, Compte et Catégorie seront obligatoires lors de la création ou modification d'un achat."
            }
        ]
        """ }
        };
    }
}
