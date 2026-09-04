using System.Drawing;
using ImageProcessor.Models;

namespace ImageProcessor.ImageManipulation;

public class ImageOperations
{
    public static async Task<Bitmap> Process(TaskOptions options)
    {
        throw new NotImplementedException();
    }

    public static async Task<Bitmap> ProcessPixelization(Stream image, int width, int height)
    {
        return await Pixelator.Execute(image, width, height);
    }

    public Task<Bitmap> ProcessResize(Stream image, int width, int height)
    {
        throw new NotImplementedException();
    }
}