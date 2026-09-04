namespace PhotoEditor.Models;

public class ImageTask
{
    public Guid Uuid { get; set; }
    public ImageTaskStatus Status { get; set; }
    public string? ResultPath { get; set; }
    public string? Error { get; set; }
}