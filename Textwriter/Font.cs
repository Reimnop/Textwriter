using System.Numerics;
using HardFuzz.HarfBuzz;
using SharpFont;

using Buffer = HardFuzz.HarfBuzz.Buffer;
using FtFace = SharpFont.Face;
using HbFont = HardFuzz.HarfBuzz.Font;
using HbGlyphInfo = HardFuzz.HarfBuzz.GlyphInfo;
using FtGlyph = SharpFont.Glyph;

namespace Textwriter;

public class Font : IDisposable
{
    public int Size { get; }
    public AtlasFontTexture Atlas { get; }

    private readonly FtFace ftFace;
    private readonly HbFont hbFont;

    private readonly Glyph[] glyphs;

    public Font(Library library, string path, int size, int atlasWidth, int atlasHeight)
    {
        Size = size;
        
        ftFace = new FtFace(library, path);
        ftFace.SetPixelSizes(0, (uint)size);

        Atlas = new AtlasFontTexture(atlasWidth, atlasHeight);
        glyphs = new Glyph[ftFace.GlyphCount];
        
        for (uint i = 0; i < glyphs.Length; i++)
        {
            ftFace.LoadGlyph(i, LoadFlags.Render, LoadTarget.Normal);
            
            GlyphSlot gs = ftFace.Glyph;
            GlyphMetrics metrics = gs.Metrics;
            
            ClientFontTexture glyphTexture = new ClientFontTexture(gs.Bitmap.Width, gs.Bitmap.Rows);
            if (gs.Bitmap.Buffer != IntPtr.Zero)
            {
                glyphTexture.WritePartial(gs.Bitmap.BufferData, gs.Bitmap.Width, gs.Bitmap.Rows, 0, 0);
            }

            glyphs[i].Width = metrics.Width.Value;
            glyphs[i].Height = metrics.Height.Value;
            glyphs[i].AdvanceX = metrics.HorizontalAdvance.Value;
            glyphs[i].AdvanceY = metrics.VerticalAdvance.Value;
            glyphs[i].HorizontalBearingX = metrics.HorizontalBearingX.Value;
            glyphs[i].HorizontalBearingY = metrics.HorizontalBearingY.Value;
            glyphs[i].VerticalBearingX = metrics.VerticalBearingX.Value;
            glyphs[i].VerticalBearingY = metrics.VerticalBearingY.Value;
            glyphs[i].Uv = Atlas.AddGlyphTexture(glyphTexture);
        }

        hbFont = HbFont.FromFreeType(ftFace.Reference);
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