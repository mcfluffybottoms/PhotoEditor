namespace PhotoEditor.Data;
public interface IFileStorageRepository
{
    Task<string?> StoreFile(IFormFile file, string filename);
    Task<string?> DeleteFile(string fileName);
    Task<Stream?> GetFile(string fileName);
}