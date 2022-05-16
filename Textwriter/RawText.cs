using System.Numerics;

namespace Textwriter;

public class RawText
{
    public string Text { get; set; }
    public Style Style { get; set; }
    public Vector2 Offset { get; set; }
    public bool Break { get; set; }
}