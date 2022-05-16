using System.Drawing;
using System.Net.Mime;
using System.Numerics;

namespace Textwriter;

public class AtlasFontTexture
{
    public ClientFontTexture Texture { get; }

    private int ptrX = 0;
    private int ptrY = 0;
    private int maxY = 0;

    public AtlasFontTexture(int width, int height)
    {
        Texture = new ClientFontTexture(width, height);
    }

    public UvInfo AddGlyphTexture(ClientFontTexture texture)
    {
        // overflow check
        if (ptrX + texture.Width > Texture.Width)
        {
            ptrY += maxY + 2;
            maxY = 0;
            ptrX = 0;
        }

        if (ptrY + texture.Height > Texture.Height)
        {
            throw new Exception("Could not fit texture onto atlas!");
        }
        
        Texture.WritePartial(texture, ptrX, ptrY);
        UvInfo uvInfo = new UvInfo
        {
            Min = new Vector2(ptrX / (float)Texture.Width, ptrY / (float)Texture.Height),
            Max = new Vector2((ptrX + texture.Width) / (float)Texture.Width, (ptrY + texture.Height) / (float)Texture.Height)
        };
        ptrX += texture.Width + 2;
        maxY = Math.Max(maxY, texture.Height);
        return uvInfo;
    }
}