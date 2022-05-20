using System.Drawing;

namespace Textwriter;

public static class TextMeshGenerator
{
    public static TextVertex[] GenerateVertices(BuiltText builtText)
    {
        List<TextVertex> vertices = new List<TextVertex>();

        int advanceX = 0;
        int advanceY = 0;
        foreach (ShapedText text in builtText)
        {
            if (text.Break)
            {
                advanceX = 0;
                advanceY = 0;
            }

            int baselineOffsetX = advanceX + text.OffsetX;
            int baselineOffsetY = advanceY + text.OffsetY;

            BuiltGlyph[] glyphInfos = text.Glyphs;

            Style style = text.Style;
            Color color = style.Color;
            
            foreach (BuiltGlyph glyphInfo in glyphInfos)
            {
                Glyph glyph = text.Font.GetGlyph(glyphInfo.Index);

                if (glyph.Width * glyph.Height > 0)
                {
                    float minX = (advanceX + glyph.HorizontalBearingX + glyphInfo.OffsetX + text.OffsetX) / 64.0f;
                    float minY = (advanceY + glyph.HorizontalBearingY - glyph.Height + glyphInfo.OffsetY + text.OffsetY) / 64.0f;
                    float maxX = (advanceX + glyph.HorizontalBearingX + glyph.Width + glyphInfo.OffsetX + text.OffsetX) / 64.0f;
                    float maxY = (advanceY + glyph.HorizontalBearingY + glyphInfo.OffsetY + text.OffsetY) / 64.0f;
                    float minU = glyph.Uv.Min.X;
                    float minV = glyph.Uv.Min.Y;
                    float maxU = glyph.Uv.Max.X;
                    float maxV = glyph.Uv.Max.Y;

                    vertices.Add(new TextVertex(minX, minY, minU, maxV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                        glyphInfo.TextureIndex));
                    vertices.Add(new TextVertex(maxX, maxY, maxU, minV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                        glyphInfo.TextureIndex));
                    vertices.Add(new TextVertex(minX, maxY, minU, minV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                        glyphInfo.TextureIndex));
                    vertices.Add(new TextVertex(minX, minY, minU, maxV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                        glyphInfo.TextureIndex));
                    vertices.Add(new TextVertex(maxX, minY, maxU, maxV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                        glyphInfo.TextureIndex));
                    vertices.Add(new TextVertex(maxX, maxY, maxU, minV, 
                        color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f,
                        glyphInfo.TextureIndex));
                }

                advanceX += glyphInfo.AdvanceX;
                advanceY += glyphInfo.AdvanceY;
            }

            // Underline
            if (text.Style.Underline)
            {
                float beginX = baselineOffsetX / 64.0f;
                float beginY = (baselineOffsetY + text.Font.UnderlinePosition + text.Font.UnderlineThickness / 2) / 64.0f;
                float endX = (advanceX + text.OffsetX) / 64.0f;
                float endY = (baselineOffsetY + text.Font.UnderlinePosition - text.Font.UnderlineThickness / 2) / 64.0f;
                
                vertices.Add(new TextVertex(beginX, endY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                vertices.Add(new TextVertex(endX, endY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                vertices.Add(new TextVertex(beginX, beginY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                vertices.Add(new TextVertex(endX, endY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                vertices.Add(new TextVertex(endX, beginY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                vertices.Add(new TextVertex(beginX, beginY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
            }
            
            // Strikethrough
            if (text.Style.Strikethrough)
            {
                float beginX = baselineOffsetX / 64.0f;
                float beginY = (baselineOffsetY + text.Font.StrikethroughPosition + text.Font.StrikethroughThickness / 2) / 64.0f;
                float endX = (advanceX + text.OffsetX) / 64.0f;
                float endY = (baselineOffsetY + text.Font.StrikethroughPosition - text.Font.StrikethroughThickness / 2) / 64.0f;
                
                vertices.Add(new TextVertex(beginX, endY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                vertices.Add(new TextVertex(endX, endY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                vertices.Add(new TextVertex(beginX, beginY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                vertices.Add(new TextVertex(endX, endY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                vertices.Add(new TextVertex(endX, beginY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
                vertices.Add(new TextVertex(beginX, beginY, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
            }
        }

        return vertices.ToArray();
    }
}