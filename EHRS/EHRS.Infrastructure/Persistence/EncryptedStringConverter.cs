using EHRS.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EHRS.Infrastructure.Persistence;

public class EncryptedStringConverter : ValueConverter<string, string>
{
    public EncryptedStringConverter(IEncryptionService encryption)
        : base(
            v => EncryptSafe(encryption, v),
            v => DecryptSafe(encryption, v))
    {
    }

    private static string EncryptSafe(IEncryptionService encryption, string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return encryption.Encrypt(value);
    }

    private static string DecryptSafe(IEncryptionService encryption, string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return encryption.Decrypt(value);
    }
}