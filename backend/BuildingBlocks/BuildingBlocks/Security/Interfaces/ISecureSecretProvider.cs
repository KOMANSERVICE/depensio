namespace BuildingBlocks.Security.Interfaces;

public interface ISecureSecretProvider
{
    Task<string> GetSecretAsync(string key);
}
