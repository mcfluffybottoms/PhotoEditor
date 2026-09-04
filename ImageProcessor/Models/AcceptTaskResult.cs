namespace ImageProcessor.Models;

public class AcceptTaskResult
{
    public string? Uuid { get; set; }
    public bool Accepted { get; set; }
    public string? ResultPath { get; set; }
    public string? Error { get; set; }
}