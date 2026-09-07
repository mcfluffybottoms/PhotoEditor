namespace PhotoEditor.Data;
public class InMemoryFileStorageRepository : IFileStorageRepository
{
    const string TEMP_FOLDER_NAME = "temp_ImageEditorTempFiles";
    public async Task<string?> StoreFile(IFormFile file, string filename)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }
        var ext = Path.GetExtension(file.FileName);
        var filePath = CreateTempDirectory(filename, ext);

        await using var stream = new FileStream(
            filePath, 
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None
        );
        await file.CopyToAsync(stream);
        return filePath;
    }
    private string CreateTempDirectory(string filename, string ext = "")
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), TEMP_FOLDER_NAME);
        Directory.CreateDirectory(tempDirectory);
        var filePath = Path.Combine(tempDirectory, filename, ext);
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