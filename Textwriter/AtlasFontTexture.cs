using System.Drawing;
using System.Numerics;

namespace Textwriter;

public class AtlasFontTexture
{
    public ClientFontTexture Texture { get; }

    private readonly List<Rectangle> previouslyAddedGlyphs = new List<Rectangle>();

    public AtlasFontTexture(int width, int height)
    {
        Texture = new ClientFontTexture(width, height);
    }

    public UvInfo AddGlyphTexture(ClientFontTexture texture)
    {
        for (int y = 0; y < Texture.Height; y++)
        {
            for (int x = 0; x < Texture.Width; x++)
            {
                Rectangle glyphRect = new Rectangle(x, y, texture.Width, texture.Height);
                if (CanTextureFit(glyphRect))
                {
                    Texture.WritePartial(texture, x, y);
                    UvInfo uvInfo = new UvInfo
                    {
                        Min = new Vector2(x / (float)Texture.Width, y / (float)Texture.Height),
                        Max = new Vector2((x + texture.Width) / (float)Texture.Width, (y + texture.Height) / (float)Texture.Height)
                    };
                    return uvInfo;
                }
            }
        }

        throw new Exception("Could not fit texture onto atlas!");
    }

    private bool CanTextureFit(Rectangle rect)
    {
        Rectangle globalRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
        if (!globalRect.Contains(rect))
        {
            return false;
        }
        
        foreach (Rectangle rectangle in previouslyAddedGlyphs)
        {
            if (rect.IntersectsWith(rectangle))
            {
                return false;
            }
        }

        return true;
    }
}