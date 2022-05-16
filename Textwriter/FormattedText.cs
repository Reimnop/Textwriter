using System.Collections;

namespace Textwriter;

public class FormattedText : IEnumerable
{
    public readonly RawText[] Texts;
    public readonly Font Font;

    public FormattedText(ICollection<RawText> texts, Font font)
    {
        Texts = texts.ToArray();
        Font = font;
    }
    
    public IEnumerator GetEnumerator()
    {
        return Texts.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}