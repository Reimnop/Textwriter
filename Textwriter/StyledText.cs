using System.Drawing;

namespace Textwriter;

public class StyledText
{
    public string Text { get; set; }
    public Style Style { get; set; }

    public StyledText(string text)
    {
        Text = text;
    }

    public StyledText(string text, Style style) : this(text)
    {
        Style = style;
    }

    public StyledText WithStyle(Style style)
    {
        Style = style;
        return this;
    }

    public StyledText WithBold(bool value)
    {
        Style style = Style;
        style.Bold = value;
        return WithStyle(style);
    }
    
    public StyledText WithItalic(bool value)
    {
        Style style = Style;
        style.Italic = value;
        return WithStyle(style);
    }
    
    public StyledText WithStrikethrough(bool value)
    {
        Style style = Style;
        style.Strikethrough = value;
        return WithStyle(style);
    }
    
    public StyledText WithUnderline(bool value)
    {
        Style style = Style;
        style.Underline = value;
        return WithStyle(style);
    }
    
    public StyledText WithColor(Color value)
    {
        Style style = Style;
        style.Color = value;
        return WithStyle(style);
    }
}