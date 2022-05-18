using System.Text.RegularExpressions;

namespace Textwriter;

public class TextBuilder
{
    private readonly List<StyledText> styledTexts = new List<StyledText>();
    private readonly Dictionary<AtlasTexture, int> atlases = new Dictionary<AtlasTexture, int>();
    private int baselineOffset = 0;
    private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
    private VerticalAlignment verticalAlignment = VerticalAlignment.Bottom;

    public TextBuilder(params Font[] fonts)
    {
        int texIndex = 0;
        for (int i = 0; i < fonts.Length; i++)
        {
            if (fonts[i].GrayscaleAtlas != null)
            {
                atlases.Add(fonts[i].GrayscaleAtlas, texIndex);
                texIndex++;
            }

            if (fonts[i].ColoredAtlas != null)
            {
                atlases.Add(fonts[i].ColoredAtlas, texIndex);
                texIndex++;
            }
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

    public TextBuilder WithBaselineOffset(int value)
    {
        baselineOffset = value;
        return this;
    }
    
    public TextBuilder WithHorizontalAlignment(HorizontalAlignment value)
    {
        horizontalAlignment = value;
        return this;
    }
    
    public TextBuilder WithVerticalAlignment(VerticalAlignment value)
    {
        verticalAlignment = value;
        return this;
    }

    public BuiltText Build()
    {
        List<List<ShapedText>> shapedTextLines = new List<List<ShapedText>>();
        shapedTextLines.Add(new List<ShapedText>());

        foreach (StyledText text in styledTexts)
        {
            List<ShapedText> shapedTexts = SplitStyledTextIntoShapedTextLines(text);
            shapedTextLines[^1].Add(shapedTexts[0]);

            for (int i = 1; i < shapedTexts.Count; i++)
            {
                shapedTextLines.Add(new List<ShapedText>());
                shapedTextLines[^1].Add(shapedTexts[i]);
            }
        }
        
        int offsetY = baselineOffset + GetOffsetY(shapedTextLines, verticalAlignment);
        for (int i = 0; i < shapedTextLines.Count; i++)
        {
            List<ShapedText> line = shapedTextLines[i];
            
            int offsetX = GetOffsetX(line, horizontalAlignment);
            line[0].Break = true;
            foreach (ShapedText shapedText in line)
            {
                shapedText.OffsetX = offsetX;
                shapedText.OffsetY = offsetY;
            }

            if (i + 1 < shapedTextLines.Count)
            {
                offsetY -= GetLineHeight(shapedTextLines[i + 1]);
            }
        }

        List<ShapedText> result = new List<ShapedText>();
        foreach (List<ShapedText> shapedTexts in shapedTextLines)
        {
            result.AddRange(shapedTexts);
        }

        return new BuiltText(result);
    }

    private List<ShapedText> SplitStyledTextIntoShapedTextLines(StyledText text)
    {
        List<ShapedText> result = new List<ShapedText>();
        string[] strs = Regex.Split(text.Text, "\n|\r\n");
        foreach (string str in strs)
        {
            GlyphInfo[] glyphs = text.Font.ShapeText(str);
            BuiltGlyph[] builtGlyphs = new BuiltGlyph[glyphs.Length];

            for (int i = 0; i < glyphs.Length; i++)
            {
                builtGlyphs[i] = new BuiltGlyph
                {
                    AdvanceX = glyphs[i].AdvanceX,
                    AdvanceY = glyphs[i].AdvanceY,
                    OffsetX = glyphs[i].OffsetX,
                    OffsetY = glyphs[i].OffsetY,
                    Index = glyphs[i].Index,
                    TextureIndex = glyphs[i].Colored
                        ? -(atlases[text.Font.ColoredAtlas] + 1)
                        : atlases[text.Font.GrayscaleAtlas] + 1
                };
            }

            ShapedText shapedText = new ShapedText();
            shapedText.Glyphs = builtGlyphs;
            shapedText.Style = text.Style;
            shapedText.Font = text.Font;
            result.Add(shapedText);
        }

        return result;
    }

    private int GetOffsetX(List<ShapedText> shapedTexts, HorizontalAlignment horizontalAlignment)
    {
        switch (horizontalAlignment)
        {
            case HorizontalAlignment.Left:
                return 0;
            case HorizontalAlignment.Center:
                return -GetLineSize(shapedTexts) / 2;
            case HorizontalAlignment.Right:
                return -GetLineSize(shapedTexts);
            default:
                throw new ArgumentException();
        }
    }

    private int GetOffsetY(List<List<ShapedText>> paragraph, VerticalAlignment verticalAlignment)
    {
        switch (verticalAlignment)
        {
            case VerticalAlignment.Bottom:
                return 0;
            case VerticalAlignment.Center:
                return GetParagraphHeight(paragraph) / 2;
            case VerticalAlignment.Top:
                return GetParagraphHeight(paragraph);
            default:
                throw new ArgumentException();
        }
    }

    private int GetParagraphHeight(List<List<ShapedText>> paragraph)
    {
        int result = 0;
        foreach (List<ShapedText> line in paragraph)
        {
            result += GetLineHeight(line);
        }

        return result;
    }

    private int GetLineHeight(List<ShapedText> shapedTexts)
    {
        int result = 0;
        foreach (ShapedText shapedText in shapedTexts)
        {
            result = Math.Max(result, shapedText.Font.Height);
        }

        return result;
    }

    private int GetLineSize(List<ShapedText> shapedTexts)
    {
        int result = 0;
        foreach (ShapedText shapedText in shapedTexts)
        {
            foreach (BuiltGlyph glyphInfo in shapedText.Glyphs)
            {
                Glyph glyph = shapedText.Font.GetGlyph(glyphInfo.Index);
                result += glyph.AdvanceX;
            }
        }

        return result;
    }
}