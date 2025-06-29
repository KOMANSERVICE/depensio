namespace depensio.Application.Interfaces;

public interface ISecureSecretProvider
{
    Task<string> GetSecretAsync(string key);
}
