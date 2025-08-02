using depensio.Domain.Constants;
using depensio.Domain.Enums;

namespace depensio.Domain.Settings;

public class BoutiqueProductConfig : BoutiqueSettingBase
{
    public override string Key { get; set; } = BoutiqueSettingKeys.PRODUCT_KEY;
    public override string Value { get; set; } = $$"""
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
        """;
}
