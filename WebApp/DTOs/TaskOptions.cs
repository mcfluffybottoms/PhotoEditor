namespace PhotoEditor.DTOs;

public class TaskOptions
{
    public required string Uuid { get; set; }
    public required string ImagePath { get; set; }
    public string? Filename { get; set; }
    public string? Name { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = [];
}