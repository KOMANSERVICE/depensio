namespace BuildingBlocks.Security.Models;

public class VaultSecretsConfig
{    
    public string Vault__Uri { get; set; } = default!;
    public string Vault__RoleId { get; set; } = default!;
    public string Vault__SecretId { get; set; } = default!;
}
