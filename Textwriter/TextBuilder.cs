namespace Textwriter;

public class TextBuilder
{
    private readonly Font font;
    private readonly List<StyledText> styledTexts = new List<StyledText>();

    public TextBuilder(Font font)
    {
        this.font = font;
    }

    public TextBuilder AddText(StyledText text)
    {
        styledTexts.Add(text);
        return this;
    }

    public TextBuilder AddTexts(ICollection<StyledText> texts)
    {
        styledTexts.AddRange(texts);
        return this;
    }

    public FormattedText Build()
    {
        List<RawText> rawTexts = new List<RawText>();
        foreach (StyledText text in styledTexts)
        {
            RawText rawText = new RawText();
            rawText.Text = text.Text;
            rawText.Style = text.Style;
            rawTexts.Add(rawText);
        }

        return new FormattedText(rawTexts, font);
    }
}