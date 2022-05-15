namespace Textwriter;

public class TextMeshGenerator
{
    private readonly Font font;

    public TextMeshGenerator(Font font)
    {
        this.font = font;
    }

    public TextVertex[] GenerateVertices(StyledText styledText)
    {
        List<TextVertex> vertices = new List<TextVertex>();
        
        GlyphInfo[] glyphInfos = font.ShapeText(styledText.Text);
        
        int advanceX = 0;
        int advanceY = 0;
        foreach (GlyphInfo glyphInfo in glyphInfos)
        {
            Glyph glyph = font.GetGlyph(glyphInfo.Index);

            float minX = (advanceX + glyphInfo.OffsetX) / (float) font.Size;
            float minY = (advanceY + glyphInfo.OffsetY) / (float) font.Size;
            float maxX = (advanceX + glyph.HorizontalBearingX + glyphInfo.OffsetX) / (float) font.Size;
            float maxY = (advanceY + glyph.Height - glyph.VerticalBearingY + glyphInfo.OffsetY) / (float) font.Size;
            float minU = glyph.Uv.Min.X;
            float minV = glyph.Uv.Min.Y;
            float maxU = glyph.Uv.Max.X;
            float maxV = glyph.Uv.Max.Y;
            
            vertices.Add(new TextVertex(minX, minY, minU, maxV, 1.0f, 1.0f, 1.0f, 1.0f));
            vertices.Add(new TextVertex(maxX, maxY, maxU, minV, 1.0f, 1.0f, 1.0f, 1.0f));
            vertices.Add(new TextVertex(minX, maxY, minU, minV, 1.0f, 1.0f, 1.0f, 1.0f));
            vertices.Add(new TextVertex(minX, minY, minU, maxV, 1.0f, 1.0f, 1.0f, 1.0f));
            vertices.Add(new TextVertex(maxX, minY, maxU, maxV, 1.0f, 1.0f, 1.0f, 1.0f));
            vertices.Add(new TextVertex(maxX, maxY, maxU, minV, 1.0f, 1.0f, 1.0f, 1.0f));
            
            advanceX += glyphInfo.AdvanceX;
            advanceY += glyphInfo.AdvanceY;
        }

        return vertices.ToArray();
    }
}