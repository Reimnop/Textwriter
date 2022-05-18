using System.Numerics;

namespace Textwriter;

public struct Glyph
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int AdvanceX { get; set; }
    public int AdvanceY { get; set; }
    public int HorizontalBearingX { get; set; }
    public int HorizontalBearingY { get; set; }
    public int VerticalBearingX { get; set; }
    public int VerticalBearingY { get; set; }
    public bool Colored { get; set; }
    public UvInfo Uv { get; set; }
}