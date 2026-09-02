namespace PhotoEditor.Services.ImageProcessing;

public interface IImageProcessing
{
    Task<byte[]> ProcessPixelization(Stream image, int width, int height);
    Task<byte[]> ProcessResize(Stream image, int width, int height);
}