namespace PhotoEditor.DTOs;

public class TaskOptions
{
    public required string Uuid { get; set; }
    public required string ImagePath { get; set; }
    public string? Operation { get; set; }
    public string? Filename { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = [];
}