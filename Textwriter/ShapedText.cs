namespace Textwriter;

public class ShapedText
{
    public GlyphInfo[] Glyphs { get; set; }
    public Style Style { get; set; }
    public Font Font { get; set; }
    public int TextureIndex { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public bool Break { get; set; }
}