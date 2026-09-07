namespace ImageProcessor.Data;

public interface IFileStorageRepository
{
    Task<string?> StoreFile(Stream file, string filename);
    Task<string?> DeleteFile(string fileName);
    Task<Stream?> GetFile(string fileName);
}