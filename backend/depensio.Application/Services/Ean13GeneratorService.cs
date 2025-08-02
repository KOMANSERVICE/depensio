using depensio.Application.Models;
using depensio.Domain.Constants;
using depensio.Domain.Enums;
using depensio.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace depensio.Application.Services;

public class Ean13GeneratorService(
    IBoutiqueSettingService _settingService,
    IDepensioDbContext _dbContext) : IBarcodeService
{
    public async Task<string> GenerateBarcodeAsync(Guid boutiqueId, string? manualBarcode = null)
    {
        var config = await GetBarcodeConfigAsync(boutiqueId);

        switch (config.Value)
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
                throw new InternalServerException($"Mode de génération non supporté: {config.Value}");
        }
    }

    private async Task<BoutiqueValue> GetBarcodeConfigAsync(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync(
            boutiqueId,
            BoutiqueSettingKeys.PRODUCT_KEY
        );

        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config);

        return result.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.PRODUCT_BARCODE_GENERATION_MODE);
    }
  
    private async Task<string> GenerateAutoBarcodeAsync(Guid boutiqueId, BoutiqueValue config)
    {
        //var barcode = $"{config.Text}{config.NextBarcodeNumber:D8}";

        //if (config.AutoIncrement)
        //{
        //    config.NextBarcodeNumber++;
        //    await _settingService.SetSettingAsync(boutiqueId, BoutiqueSettingKeys.PRODUCT_KEY, config);
        //}

        //return barcode;

        string baseCode;
        string fullCode;

        do
        {
            baseCode = GenerateRandomBase(); // 12 chiffres
            var checksum = CalculateChecksum(baseCode);
            fullCode = baseCode + checksum;

        } while (await CodeExistsInDatabaseAsync(fullCode));

        return fullCode;
    }

    private string GenerateRandomBase()
    {
        var random = new Random();
        var sb = new StringBuilder("613"); // Préfixe fictif

        for (int i = 0; i < 9; i++)
        {
            sb.Append(random.Next(0, 10));
        }

        return sb.ToString();
    }

    private int CalculateChecksum(string ean12)
    {
        int sum = 0;

        for (int i = 0; i < ean12.Length; i++)
        {
            int digit = int.Parse(ean12[i].ToString());
            sum += (i % 2 == 0) ? digit : digit * 3;
        }

        int mod = sum % 10;
        return mod == 0 ? 0 : 10 - mod;
    }

    private async Task<bool> CodeExistsInDatabaseAsync(string code)
    {
        return await _dbContext.Products.AnyAsync(p => p.Barcode == code);
    }
}