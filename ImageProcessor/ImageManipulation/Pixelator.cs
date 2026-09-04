using System.Data;
using System.Drawing;

namespace ImageProcessor.ImageManipulation;

public class Pixelator
{
    public class Grid {
        public int Width { get; set; }
        public int Height { get; set; }
    }
    class ColorPixel
    {
        public int R { get; set; } = 0;
        public int G { get; set; } = 0;
        public int B { get; set; } = 0;
        public int Count;

        public Color ReturnColor()
        {
            return Color.FromArgb(
                R / Count, 
                G / Count, 
                B / Count
            );
        }
        public void Add(Color color)
        {
            R += color.R;
            G += color.G;
            B += color.B;
            Count++;
        }
    }
    public class Image
    {
        int[][] Pixels;
        Dictionary<int, ColorPixel> Colors;

        public Bitmap FillBitmap(Bitmap bitmapToFill)
        {
            if(bitmapToFill.Height < Pixels.Length || bitmapToFill.Width < Pixels[0].Length)
            {
                throw new InvalidOperationException("bitmapToFill is samller than Image.");
            }

            for(int y = 0; y < bitmapToFill.Height; ++y)
            {
                for(int x = 0; x < bitmapToFill.Width; ++x)
                {
                    int pX = x * Pixels[0].Length / Pixels[0].Length;
                    int pY = y * Pixels.Length / Pixels.Length;
                    bitmapToFill.SetPixel(x, y, Colors[Pixels[pY][pX]].ReturnColor());
                }
            }
            return bitmapToFill;
        }
    }
    public static async Task<Bitmap> Execute(Stream path, int width, int height)
    {
        var image = new Bitmap(path);
        Grid pixelGrid = new(){ Width = width, Height = height, };
        Grid baseGrid = new(){ Width = image.Width, Height = image.Height };
        //var img = await Resize(image, baseGrid);
        return GetPixelColor(image, baseGrid, pixelGrid);
    }
    private static Task<Bitmap> Resize(Bitmap image, Grid grid)
    {
        using var graphics = Graphics.FromImage(image);

        graphics.DrawImage(
            image,
            new Rectangle(0, 0, grid.Width, grid.Height),
            0, 0, image.Width, image.Height,
            GraphicsUnit.Pixel
        );

        return Task.FromResult(image);
    }

    private static Bitmap GetPixelColor(Bitmap image, Grid baseGrid, Grid pixelGrid)
    {
        ColorPixel[][] finalImage = InitColorMatrix(pixelGrid);
        //Dictionary<int, ColorPixel> ColorsDict = [];
        //Dictionary<int, ColorPixel> CollectedColors = [];
        for(int y = 0; y < baseGrid.Height; ++y)
        {
            for(int x = 0; x < baseGrid.Width; ++x)
            {
                int pX = x * pixelGrid.Width / baseGrid.Width;
                int pY = y * pixelGrid.Height / baseGrid.Height;
                finalImage[pY][pX].Add(image.GetPixel(x, y));
            }
        }
        for(int y = 0; y < baseGrid.Height; ++y)
        {
            for(int x = 0; x < baseGrid.Width; ++x)
            {
                int pX = x * pixelGrid.Width / baseGrid.Width;
                int pY = y * pixelGrid.Height / baseGrid.Height;
                image.SetPixel(x, y, finalImage[pY][pX].ReturnColor());
            }
        }
        return image;
    }

    private static ColorPixel[][] InitColorMatrix(Grid pixelGrid)
    {
        ColorPixel[][] result = new ColorPixel[pixelGrid.Height][];

        for (int y = 0; y < pixelGrid.Height; y++)
        {
            result[y] = new ColorPixel[pixelGrid.Width];

            for (int x = 0; x < pixelGrid.Width; x++)
            {
                result[y][x] = new ColorPixel();
            }
        }

        return result;
    }
}