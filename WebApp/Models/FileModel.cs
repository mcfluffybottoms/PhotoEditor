namespace PhotoEditor.Models;

public class FileModel
{
    public Guid Uuid { get; set; }
    public required string Filename { get; set; }
    public required string Path { get; set; }
}