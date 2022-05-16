namespace Textwriter;

public class TextBuilder
{
    private readonly List<StyledText> styledTexts = new List<StyledText>();
    private readonly Dictionary<Font, int> fonts = new Dictionary<Font, int>();

    public TextBuilder(params Font[] fonts)
    {
        for (int i = 0; i < fonts.Length; i++)
        {
            this.fonts.Add(fonts[i], i);
        }
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
        int offsetY = 0;
        
        // It works. Don't touch.
        List<RawText> rawTexts = new List<RawText>();
        foreach (StyledText text in styledTexts)
        {
            int lineHeight = text.Font.Size;
            
            string[] strs = text.Text.Split('\n');
            
            RawText rawText = new RawText();
            rawText.Text = strs[0];
            rawText.Style = text.Style;
            rawText.Font = text.Font;
            rawText.TextureIndex = fonts[text.Font];
            rawText.OffsetX = 0;
            rawText.OffsetY = -offsetY;
            rawTexts.Add(rawText);

            for (int i = 1; i < strs.Length; i++)
            {
                offsetY += lineHeight * 64;
                
                RawText rawText2 = new RawText();
                rawText2.Text = strs[i];
                rawText2.Style = text.Style;
                rawText2.Font = text.Font;
                rawText2.TextureIndex = fonts[text.Font];
                rawText2.Break = true;
                rawText2.OffsetX = 0;
                rawText2.OffsetY = -offsetY;
                rawTexts.Add(rawText2);
            }
        }

        return new FormattedText(rawTexts);
    }
}