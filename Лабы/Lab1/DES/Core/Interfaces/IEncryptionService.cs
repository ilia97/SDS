using Core.Misc;

namespace Core.Interfaces
{
    public interface IEncryptionService
    {
        string EncryptWithDES(string text, string key, EncodingType type);
    }
}
