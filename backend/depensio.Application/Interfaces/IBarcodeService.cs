
namespace depensio.Application.Interfaces;

public interface IBarcodeService
{
    Task<string> GenerateBarcodeAsync(Guid boutiqueId, string? manualBarcode = null);
}