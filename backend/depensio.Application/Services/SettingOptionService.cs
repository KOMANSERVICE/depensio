using depensio.Application.Interfaces;
using depensio.Application.Models;
using System.Text.Json;

namespace depensio.Application.Services;

public class SettingOptionService(
    IBoutiqueSettingService _settingService)
{

    public async Task<BarcodeGenerationMode> GetBarcodeConfigAsync(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync(
            boutiqueId,
            BoutiqueSettingKeys.PRODUCT_KEY
        );

        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);

        var configBarcode = result?.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.PRODUCT_BARCODE_GENERATION_MODE);
        return EnumHelper.ParseOrDefault<BarcodeGenerationMode>(configBarcode.Value.ToString(), BarcodeGenerationMode.Auto);
    }

    public async Task<bool> AutoriserLesProduitAvecStockZero(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync(
                  boutiqueId,
                  BoutiqueSettingKeys.PRODUCT_KEY
              );

        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);

        var stockAuto = result?.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.PRODUCT_STOCK_AUTOMATIQUE);

        return BoolHelper.ToBool(stockAuto?.Value.ToString());
    }
}
