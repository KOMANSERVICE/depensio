using depensio.Domain.Constants;
using depensio.Domain.Enums;
using depensio.Domain.Settings;
using System.Text.Json;

namespace depensio.Application.Services;

public class BarcodeService(IBoutiqueSettingService _settingService) : IBarcodeService
{
    public async Task<string> GenerateBarcodeAsync(Guid boutiqueId, string? manualBarcode = null)
    {
        var config = await GetBarcodeConfigAsync(boutiqueId);

        switch (config.GenerationMode)
        {
            case BarcodeGenerationMode.Manual:
                if (string.IsNullOrEmpty(manualBarcode))
                    throw new BadRequestException("Code-barre manuel requis");
                return manualBarcode;

            case BarcodeGenerationMode.Auto:
                return await GenerateAutoBarcodeAsync(boutiqueId, config);

            case BarcodeGenerationMode.Hybrid:
                return !string.IsNullOrEmpty(manualBarcode)
                    ? manualBarcode
                    : await GenerateAutoBarcodeAsync(boutiqueId, config);

            default:
                throw new InternalServerException($"Mode de génération non supporté: {config.GenerationMode}");
        }
    }

    private async Task<BoutiqueBarcodeConfig> GetBarcodeConfigAsync(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync<BoutiqueBarcodeConfig>(
            boutiqueId,
            BoutiqueSettingKeys.BARCODE_GENERATION_MODE,
            new BoutiqueBarcodeConfig()
        );

        return config ?? new BoutiqueBarcodeConfig();
    }
  
    private async Task<string> GenerateAutoBarcodeAsync(Guid boutiqueId, BoutiqueBarcodeConfig config)
    {
        var barcode = $"{config.Prefix}{config.NextBarcodeNumber:D8}";

        if (config.AutoIncrement)
        {
            config.NextBarcodeNumber++;
            await _settingService.SetSettingAsync(boutiqueId, BoutiqueSettingKeys.BARCODE_GENERATION_MODE, config);
        }

        return barcode;
    }
}