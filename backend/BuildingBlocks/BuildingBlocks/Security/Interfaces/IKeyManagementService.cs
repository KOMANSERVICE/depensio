namespace BuildingBlocks.Security.Interfaces;

public interface IKeyManagementService
{
    string GetKey(string keyVersion);
    string GetLastKeyVerdion();
}
