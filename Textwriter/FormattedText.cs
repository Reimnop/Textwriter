using System.Collections;

namespace Textwriter;

public class FormattedText : IEnumerable
{
    public RawText[] Texts { get; }

    public FormattedText(ICollection<RawText> texts)
    {
        Texts = texts.ToArray();
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