using System.Collections;

namespace Textwriter;

public class BuiltText : IEnumerable
{
    public ShapedText[] Texts { get; }

    public BuiltText(IEnumerable<ShapedText> texts)
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