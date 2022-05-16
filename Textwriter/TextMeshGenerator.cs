using System.Drawing;

namespace Textwriter;

public static class TextMeshGenerator
{
    public static TextVertex[] GenerateVertices(FormattedText formattedText)
    {
        Font font = formattedText.Font;
        
        List<TextVertex> vertices = new List<TextVertex>();

        int advanceX = 0;
        int advanceY = 0;
        foreach (RawText text in formattedText)
        {
            if (text.Break)
            {
                advanceX = 0;
                advanceY = 0;
            }
            
            GlyphInfo[] glyphInfos = font.ShapeText(text.Text);

            Style style = text.Style;
            Color color = style.Color;
            
            foreach (GlyphInfo glyphInfo in glyphInfos)
            {
                Glyph glyph = font.GetGlyph(glyphInfo.Index);

                if (glyph.Width * glyph.Height > 0)
                {
                    float minX = (advanceX + glyph.HorizontalBearingX + glyphInfo.OffsetX) / 64.0f;
                    float minY = (advanceY + glyph.HorizontalBearingY - glyph.Height + glyphInfo.OffsetY) / 64.0f;
                    float maxX = (advanceX + glyph.HorizontalBearingX + glyph.Width + glyphInfo.OffsetX) / 64.0f;
                    float maxY = (advanceY + glyph.HorizontalBearingY + glyphInfo.OffsetY) / 64.0f;
                    float minU = glyph.Uv.Min.X;
                    float minV = glyph.Uv.Min.Y;
                    float maxU = glyph.Uv.Max.X;
                    float maxV = glyph.Uv.Max.Y;

                    vertices.Add(new TextVertex(minX, minY, minU, maxV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                    vertices.Add(new TextVertex(maxX, maxY, maxU, minV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                    vertices.Add(new TextVertex(minX, maxY, minU, minV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                    vertices.Add(new TextVertex(minX, minY, minU, maxV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                    vertices.Add(new TextVertex(maxX, minY, maxU, maxV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                    vertices.Add(new TextVertex(maxX, maxY, maxU, minV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                }

                advanceX += glyphInfo.AdvanceX;
                advanceY += glyphInfo.AdvanceY;
            }
        }

        return vertices.ToArray();
    }
}