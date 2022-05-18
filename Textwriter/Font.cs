using System.Numerics;
using System.Runtime.CompilerServices;
using HardFuzz.HarfBuzz;
using SharpFont;
using SharpFont.TrueType;
using Buffer = HardFuzz.HarfBuzz.Buffer;
using FtFace = SharpFont.Face;
using HbFont = HardFuzz.HarfBuzz.Font;
using HbGlyphInfo = HardFuzz.HarfBuzz.GlyphInfo;
using FtGlyph = SharpFont.Glyph;

namespace Textwriter;

public class Font : IDisposable
{
    public int Size { get; }
    public int Height { get; }
    public int Ascender { get; }
    public int Descender { get; }
    public int UnderlinePosition { get; }
    public int UnderlineThickness { get; }
    public int StrikethroughPosition { get; }
    public int StrikethroughThickness { get; }

    public AtlasTexture? GrayscaleAtlas { get; }
    public AtlasTexture? ColoredAtlas { get; }
    

    private readonly FtFace ftFace;
    private readonly HbFont hbFont;

    private readonly Glyph[] glyphs;

    public Font(Library library, string path, int size, int atlasWidth)
    {
        Size = size;
        
        ftFace = new FtFace(library, path);
        ftFace.SetPixelSizes(0, (uint)size);

        Height = ftFace.Size.Metrics.Height.Value;
        Ascender = ftFace.Size.Metrics.Ascender.Value;
        Descender = ftFace.Size.Metrics.Descender.Value;
        UnderlinePosition = ftFace.UnderlinePosition;
        UnderlineThickness = ftFace.UnderlineThickness;

        StrikethroughPosition = Ascender / 2;
        StrikethroughThickness = UnderlineThickness;
        
        List<ClientTexture> grayscaleGlyphTextures = new List<ClientTexture>();
        List<ClientTexture> coloredGlyphTextures = new List<ClientTexture>();
        
        glyphs = new Glyph[ftFace.GlyphCount];
        for (uint i = 0; i < glyphs.Length; i++)
        {
            ftFace.LoadGlyph(i, LoadFlags.Render | LoadFlags.Color, LoadTarget.Normal);

            GlyphSlot gs = ftFace.Glyph;
            GlyphMetrics metrics = gs.Metrics;

            ClientTexture glyphTexture = new ClientTexture(gs.Bitmap.Width, gs.Bitmap.Rows, 
                gs.Bitmap.PixelMode == PixelMode.Gray ? 1 : 
                gs.Bitmap.PixelMode == PixelMode.Bgra ? 4 : 
                throw new NotImplementedException());
            
            if (gs.Bitmap.Buffer != IntPtr.Zero)
            {
                glyphTexture.WritePartial(gs.Bitmap.BufferData, gs.Bitmap.Width, gs.Bitmap.Rows, 0, 0);
            }

            switch (gs.Bitmap.PixelMode)
            {
                case PixelMode.Gray:
                    grayscaleGlyphTextures.Add(glyphTexture);
                    break;
                case PixelMode.Bgra:
                    coloredGlyphTextures.Add(glyphTexture);
                    break;
                default:
                    throw new NotImplementedException();
            }

            glyphs[i].Width = metrics.Width.Value;
            glyphs[i].Height = metrics.Height.Value;
            glyphs[i].AdvanceX = metrics.HorizontalAdvance.Value;
            glyphs[i].AdvanceY = metrics.VerticalAdvance.Value;
            glyphs[i].HorizontalBearingX = metrics.HorizontalBearingX.Value;
            glyphs[i].HorizontalBearingY = metrics.HorizontalBearingY.Value;
            glyphs[i].VerticalBearingX = metrics.VerticalBearingX.Value;
            glyphs[i].VerticalBearingY = metrics.VerticalBearingY.Value;
            glyphs[i].Colored =
                gs.Bitmap.PixelMode != PixelMode.Gray && (gs.Bitmap.PixelMode == PixelMode.Bgra ? true :
                    throw new NotImplementedException()); // if it's gray, false, it's it's bgra, true, if it's neither, throw
        }

        GrayscaleAtlas = null;
        ColoredAtlas = null;

        if (grayscaleGlyphTextures.Count > 0)
        {
            int atlasHeight = CalculateAtlasHeight(grayscaleGlyphTextures, atlasWidth);
            GrayscaleAtlas = new AtlasTexture(atlasWidth, atlasHeight, 1);
        }
        
        if (coloredGlyphTextures.Count > 0)
        {
            int atlasHeight = CalculateAtlasHeight(coloredGlyphTextures, atlasWidth);
            ColoredAtlas = new AtlasTexture(atlasWidth, atlasHeight, 4);
        }

        int gIndex = 0;
        int cIndex = 0;
        for (int i = 0; i < glyphs.Length; i++)
        {
            if (!glyphs[i].Colored)
            {
                glyphs[i].Uv = GrayscaleAtlas.AddGlyphTexture(grayscaleGlyphTextures[gIndex]);
                gIndex++;
            }
            else
            {
                glyphs[i].Uv = ColoredAtlas.AddGlyphTexture(coloredGlyphTextures[cIndex]);
                cIndex++;
            }
        }

        hbFont = HbFont.FromFreeType(ftFace.Reference);
    }

    private int CalculateAtlasHeight(IEnumerable<ClientTexture> textures, int atlasWidth)
    {
        int ptrX = 0;
        int ptrY = 0;
        int maxY = 0;
        foreach (ClientTexture texture in textures)
        {
            if (ptrX + texture.Width > atlasWidth)
            {
                ptrY += maxY + 4;
                maxY = 0;
                ptrX = 0;
            }
            
            ptrX += texture.Width + 4;
            maxY = Math.Max(maxY, texture.Height);
        }

        return ptrY + maxY;
    }

    public GlyphInfo[] ShapeText(string text)
    {
        using Buffer buffer = new Buffer();
        buffer.AddUtf(text);
        buffer.GuessSegmentProperties();
        buffer.Shape(hbFont);

        GlyphPosition[] glyphPositions = buffer.GlyphPositions.ToArray();
        HbGlyphInfo[] glyphInfos = buffer.GlyphInfos.ToArray();

        GlyphInfo[] glyphs = new GlyphInfo[buffer.Length];
        for (int i = 0; i < buffer.Length; i++)
        {
            glyphs[i] = new GlyphInfo
            {
                AdvanceX = glyphPositions[i].XAdvance,
                AdvanceY = glyphPositions[i].YAdvance,
                OffsetX = glyphPositions[i].XOffset,
                OffsetY = glyphPositions[i].YOffset,
                Colored = this.glyphs[glyphInfos[i].Codepoint].Colored,
                Index = (int)glyphInfos[i].Codepoint
            };
        }

        return glyphs;
    }

    public Glyph GetGlyph(int index)
    {
        return glyphs[index];
    }

    public void Dispose()
    {
        hbFont.Dispose();
        ftFace.Dispose();
    }
}