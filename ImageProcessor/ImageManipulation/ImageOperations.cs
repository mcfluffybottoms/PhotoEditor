using System.Drawing;
using ImageProcessor.Models;

namespace ImageProcessor.ImageManipulation;

public class ImageOperations
{
    public class ImageResult {
        public required bool IsProcessed { get; set; }
        public Bitmap? Image { get; set; }
        public string? Error { get; set; }
        public static implicit operator ImageResult((bool, Bitmap?, string?) result)
        {
            var (isProcessed, image, error) = result;
            return new ImageResult
            {
                IsProcessed = isProcessed,
                Image = image,
                Error = error
            };
        }
    }
    public static async Task<ImageResult> Process(TaskOptions options)
    {
        if(!File.Exists(options.ImagePath))
        {
            return (false, null, "File does not exist.");
        }
        await using FileStream stream = File.OpenRead(options.ImagePath);
        switch (options.Operation)
        {
            case "PIXEL":
                try
                {
                    return await ProcessPixelization(stream, options.Parameters);
                } catch(Exception e)
                {
                    return (false, null, $"Error while processing image: {e.Message}");
                }
            default:
                return (false, null, $"Operation not found.");
        }
    }

    public static async Task<ImageResult> ProcessPixelization(Stream image, Dictionary<string, string> parameters)
    {
        if (!TryGetIntParameter(parameters, "width", out int width, out string? errorW))
        {
            return (false, null, errorW);
        }
        if (!TryGetIntParameter(parameters, "height", out int height, out string? errorH))
        {
            return (false, null, errorH);
        }

        try
        {
            return (true, await Pixelator.Execute(image, width, height), null);
        } catch (Exception e)
        {
            return (false, null, $"Error while processing image: {e.Message}");
        }
    }

    private static bool TryGetIntParameter(Dictionary<string, string> parameters, string name, out int value, out string? error)
    {
        value = 0;
        error = null;

        if (!parameters.TryGetValue(name, out string? valueStr))
        {
            error = $"Parameter not found: {name}.";
            return false;
        }

        if (!int.TryParse(valueStr, out value))
        {
            error = $"Parameter {name} cannot be converted to integer.";
            return false;
        }

        return true;
    }

    public Task<Bitmap> ProcessResize(Stream image, int width, int height)
    {
        throw new NotImplementedException();
    }
}