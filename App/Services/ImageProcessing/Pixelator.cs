namespace PhotoEditor.Services.ImageProcessing;

public class Pixelator
{
    public static async Task<byte[]> Execute(Stream image, int width, int height)
    {
        byte[] downsampled = await Downsample(image, width, height);
        return downsampled;
    }

    private static Task<byte[]> Downsample(Stream image, int width, int height)
    {
        throw new NotImplementedException();
    }

    private static Task<byte[]> Upsample(Stream image, int width, int height)
    {
        throw new NotImplementedException();
    }

    private static Task<byte[]> NearestNeighbourAlgorithm(Stream image, int width, int height)
    {
        throw new NotImplementedException();
    }
}