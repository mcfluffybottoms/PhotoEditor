namespace ImageProcessor.Data;

public class InMemoryFileStorageRepository : IFileStorageRepository
{
    const string TEMP_FOLDER_NAME = "temp_ImageEditorTempFiles";
    public async Task<string?> StoreFile(Stream file, string filename)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        var filePath = CreateTempDirectory(filename);

        await using var stream = new FileStream(
            filePath, 
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None
        );
        await file.CopyToAsync(stream);
        return filePath;
    }
    private static string CreateTempDirectory(string filename)
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), TEMP_FOLDER_NAME);
        Directory.CreateDirectory(tempDirectory);
        var filePath = Path.Combine(tempDirectory, filename);
        return filePath;
    }
    public async Task<string?> DeleteFile(string fileName)
    {
        var filePath = CreateTempDirectory(fileName);
        if (!File.Exists(filePath))
        {
            return null;
        }
        File.Delete(filePath);
        return filePath;
    }
    public async Task<Stream?> GetFile(string fileName)
    {
        var filePath = CreateTempDirectory(fileName);
        if (!File.Exists(filePath))
        {
            return null;
        }
        return new FileStream(filePath, FileMode.Open);
    }
}