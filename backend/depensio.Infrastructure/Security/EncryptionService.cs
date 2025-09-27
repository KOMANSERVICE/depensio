using depensio.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace depensio.Infrastructure.Security;

public class EncryptionService(IKeyManagementService _keyManagementService, ILogger<EncryptionService> logger) : IEncryptionService
{
    public string Encrypt(string plainText)
    {
        string keyVersion = _keyManagementService.GetLastKeyVerdion();
        if (string.IsNullOrEmpty(plainText)) return "";
        if (string.IsNullOrEmpty(keyVersion)) throw new ArgumentNullException(nameof(keyVersion));

        string key = _keyManagementService.GetKey(keyVersion);
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException($"La clé pour la version '{keyVersion}' est introuvable ou vide.");

        byte[] keyBytes = Convert.FromBase64String(key);

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.GenerateIV();

            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length); // Stocke l'IV au début du flux
                ms.Write(BitConverter.GetBytes(keyVersion.Length), 0, sizeof(int)); // Stocke la longueur de la version
                ms.Write(Encoding.UTF8.GetBytes(keyVersion), 0, keyVersion.Length); // Stocke la version

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return "";
        try
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            using (var ms = new MemoryStream(fullCipher))
            using (Aes aes = Aes.Create())
            {
                int ivSize = aes.BlockSize / 8; // Taille dynamique du IV
                byte[] iv = new byte[ivSize];
                ms.Read(iv, 0, iv.Length);

                byte[] keyVersionLengthBytes = new byte[sizeof(int)];
                int readKeyVersionLengthBytes = ms.Read(keyVersionLengthBytes, 0, sizeof(int));
                if (readKeyVersionLengthBytes != sizeof(int))
                {
                    logger.LogWarning("Impossible de lire la longueur de la version de clé.");
                    return cipherText;
                }
                int keyVersionLength = BitConverter.ToInt32(keyVersionLengthBytes, 0);

                byte[] keyVersionBytes = new byte[keyVersionLength];
                int readKeyVersionBytes = ms.Read(keyVersionBytes, 0, keyVersionLength);
                if (readKeyVersionBytes != keyVersionLength)
                {
                    logger.LogWarning("Impossible de lire la version de clé, données corrompues ou format inattendu.");
                    return cipherText;
                }
                string keyVersion = Encoding.UTF8.GetString(keyVersionBytes);

                string key = _keyManagementService.GetKey(keyVersion);
                if (string.IsNullOrEmpty(key))
                {
                   
                    return cipherText;
                }
                   
                byte[] keyBytes = Convert.FromBase64String(key);

                aes.IV = iv;
                aes.Key = keyBytes;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "Erreur lors du déchiffrement");
            return cipherText;
        }
    }
}
