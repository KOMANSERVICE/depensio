namespace depensio.Application.Interfaces;

public interface IKeyManagementService
{
    string GetKey(string keyVersion);
    string GetLastKeyVerdion();
}
