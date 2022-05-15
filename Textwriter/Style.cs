using System.Drawing;

namespace Textwriter;

public struct Style
{
    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public bool Strikethrough { get; set; }
    public bool Underline { get; set; }
    public Color Color { get; set; }
}