namespace PhotoEditor.DTOs;

public class FileUploadDto
{
    public required string NewFilename { get; set; }
    public IFormFile Image { get; set; } = default!;
    public Dictionary<string, string> Parameters { get; set; } = [];
}