using System.Drawing;

namespace Textwriter;

public struct Style
{
    public bool Strikethrough { get; set; }
    public bool Underline { get; set; }
    public Color Color { get; set; }
}