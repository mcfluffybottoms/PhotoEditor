using System.Drawing;

namespace PhotoEditor.Services.ImageProcessing;

public class ImageProcessing : IImageProcessing
{
    public Task<byte[]> ProcessPixelization(Stream image, int width, int height)
    {
        using var img = new Bitmap(image);
        throw new NotImplementedException();
    }

    public Task<byte[]> ProcessResize(Stream image, int width, int height)
    {
        throw new NotImplementedException();
    }
}