using System.Runtime.InteropServices;

namespace Textwriter;

public class ClientFontTexture
{
    public int Width { get; }
    public int Height { get; }
    public byte[] Pixels { get; }

    public ClientFontTexture(int width, int height)
    {
        Pixels = new byte[width * height];
        Width = width;
        Height = height;
    }

    public byte this[int x, int y]
    {
        get => GetPixel(x, y);
        set => SetPixel(x, y, value);
    }

    private byte GetPixel(int x, int y)
    {
        return Pixels[y * Width + x];
    }

    private void SetPixel(int x, int y, byte value)
    {
        Pixels[y * Width + x] = value;
    }

    public void WritePartial(ClientFontTexture texture, int offsetX, int offsetY)
    {
        WritePartial(texture.Pixels, texture.Width, texture.Height, offsetX, offsetY);
    }

    public void WritePartial(byte[] pixels, int width, int height, int offsetX, int offsetY)
    {
        if (offsetX + width > Width || offsetY + height > Height ||
            offsetX < 0 || offsetY < 0)
        {
            throw new IndexOutOfRangeException();
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SetPixel(x + offsetX, y + offsetY, pixels[y * width + x]);
            }
        }
    }

    public void WritePartial(IntPtr ptr, int width, int height, int offsetX, int offsetY)
    {
        int offsetIndex = offsetY * Width + offsetX;
        Marshal.Copy(ptr, Pixels, offsetIndex, width * height);
    }
}